package com.kra.paypoint.ui.screens.admin.items.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.CodeEntity
import com.kra.paypoint.data.local.entity.ItemClsEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.domain.model.EtimsCodes
import com.kra.paypoint.domain.model.EtimsCodes.CodeClass
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import com.kra.paypoint.domain.repository.MasterSaveOutcome
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.flatMapLatest
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

/** [editingItemCode] null means a new item, whose code is generated from origin/type/units. */
data class ItemForm(
    val editingItemCode: String? = null,
    val previewItemCode: String = "",
    val barcode: String = "",
    val itemName: String = "",
    val description: String = "",
    val salePrice: String = "",
    val beginningStock: String = "0",
    val classCode: String = "",
    val className: String = "",
    val originNationCode: String = "KE",
    val packagingUnitCode: String = "NT",
    val itemTypeCode: String = "2",
    val quantityUnitCode: String = "U",
    val taxTypeCode: String = EtimsCodes.TaxType.STANDARD,
    val isUsed: Boolean = true
) {
    val isNew: Boolean get() = editingItemCode == null
    val isService: Boolean get() = itemTypeCode == ITEM_TYPE_SERVICE

    companion object {
        const val ITEM_TYPE_SERVICE = "3"
    }
}

data class ItemManagementUiState(
    val form: ItemForm? = null,
    val isSaving: Boolean = false,
    val message: String? = null,
    val errorMessage: String? = null
)

/** The pick-lists an item needs, preferring codes synced from eTIMS over the built-in fallbacks. */
data class ItemCodeLists(
    val countries: List<EtimsCodes.Code> = EtimsCodes.fallbackCountries,
    val itemTypes: List<EtimsCodes.Code> = EtimsCodes.ItemType.fallback,
    val packagingUnits: List<EtimsCodes.Code> = EtimsCodes.fallbackPackagingUnits,
    val quantityUnits: List<EtimsCodes.Code> = EtimsCodes.fallbackQuantityUnits,
    val taxTypes: List<EtimsCodes.Code> = EtimsCodes.TaxType.all
)

@HiltViewModel
class ItemManagementViewModel @Inject constructor(
    private val masterDataRepository: MasterDataRepository,
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(ItemManagementUiState())
    val uiState: StateFlow<ItemManagementUiState> = _uiState.asStateFlow()

    val items: StateFlow<List<ItemEntity>> = masterDataRepository.getAllItemsIncludingUnused()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    val codeLists: StateFlow<ItemCodeLists> = kotlinx.coroutines.flow.combine(
        codes(CodeClass.COUNTRY, EtimsCodes.fallbackCountries),
        codes(CodeClass.ITEM_TYPE, EtimsCodes.ItemType.fallback),
        codes(CodeClass.PACKAGING_UNIT, EtimsCodes.fallbackPackagingUnits),
        codes(CodeClass.QUANTITY_UNIT, EtimsCodes.fallbackQuantityUnits)
    ) { countries, types, pkg, qty -> ItemCodeLists(countries, types, pkg, qty) }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), ItemCodeLists())

    private val classQuery = MutableStateFlow("")

    @OptIn(ExperimentalCoroutinesApi::class)
    val classResults: StateFlow<List<ItemClsEntity>> = classQuery
        .flatMapLatest { q -> if (q.length < 2) flowOf(emptyList()) else masterDataRepository.searchItemClassifications(q) }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    private fun codes(classCode: String, fallback: List<EtimsCodes.Code>): Flow<List<EtimsCodes.Code>> =
        masterDataRepository.observeCodes(classCode).map { list: List<CodeEntity> ->
            if (list.isEmpty()) fallback else list.map { EtimsCodes.Code(it.detailCode, it.detailName) }
        }

    fun onClassQueryChanged(query: String) {
        classQuery.value = query
    }

    fun startNew() {
        _uiState.update { it.copy(form = ItemForm(), errorMessage = null, message = null) }
        refreshPreviewCode()
    }

    fun startEdit(item: ItemEntity) {
        viewModelScope.launch {
            val cls = masterDataRepository.getItemClassification(item.itemClassificationCode)
            _uiState.update {
                it.copy(
                    form = ItemForm(
                        editingItemCode = item.itemCode,
                        previewItemCode = item.itemCode,
                        barcode = item.barcode.orEmpty(),
                        itemName = item.itemName,
                        description = item.description.orEmpty(),
                        salePrice = plain(item.defaultUnitPrice),
                        beginningStock = plain(item.stockQuantity),
                        classCode = item.itemClassificationCode,
                        className = cls?.name.orEmpty(),
                        originNationCode = item.originNationCode,
                        packagingUnitCode = item.packagingUnitCode,
                        itemTypeCode = item.itemTypeCode,
                        quantityUnitCode = item.quantityUnitCode,
                        taxTypeCode = item.taxTypeCode,
                        isUsed = item.isUsed
                    ),
                    errorMessage = null,
                    message = null
                )
            }
        }
    }

    fun dismissForm() {
        _uiState.update { it.copy(form = null, errorMessage = null) }
    }

    fun updateForm(transform: (ItemForm) -> ItemForm) {
        val before = _uiState.value.form
        _uiState.update { state -> state.copy(form = state.form?.let(transform), errorMessage = null) }
        val after = _uiState.value.form
        if (after != null && after.isNew && before != null && codeInputsChanged(before, after)) refreshPreviewCode()
    }

    fun selectClassification(cls: ItemClsEntity) {
        updateForm { form ->
            form.copy(
                classCode = cls.code,
                className = cls.name,
                // KRA attaches a default tax band to many classifications; adopt it, the admin can still override.
                taxTypeCode = cls.taxTypeCode?.takeIf { it in EtimsCodes.TaxType.all.map { t -> t.code } } ?: form.taxTypeCode
            )
        }
        classQuery.value = ""
    }

    private fun codeInputsChanged(a: ItemForm, b: ItemForm) =
        a.originNationCode != b.originNationCode || a.itemTypeCode != b.itemTypeCode ||
            a.packagingUnitCode != b.packagingUnitCode || a.quantityUnitCode != b.quantityUnitCode

    private fun refreshPreviewCode() {
        val form = _uiState.value.form ?: return
        viewModelScope.launch {
            val code = masterDataRepository.generateItemCode(
                form.originNationCode, form.itemTypeCode, form.packagingUnitCode, form.quantityUnitCode
            )
            _uiState.update { state -> state.copy(form = state.form?.takeIf { it.isNew }?.copy(previewItemCode = code) ?: state.form) }
        }
    }

    fun save() {
        // Re-entry guard: a double tap can land before the disabled button recomposes.
        if (_uiState.value.isSaving) return
        val form = _uiState.value.form ?: return
        val operator = authRepository.currentUser.value
        if (operator == null || !operator.isAdmin) {
            _uiState.update { it.copy(errorMessage = "Only an admin can manage items.") }
            return
        }
        val price = form.salePrice.trim().replace(",", "").toDoubleOrNull()
        val stock = form.beginningStock.trim().ifEmpty { "0" }.replace(",", "").toDoubleOrNull()
        if (price == null) {
            _uiState.update { it.copy(errorMessage = "Enter a valid sale price.") }
            return
        }
        if (form.isNew && stock == null) {
            _uiState.update { it.copy(errorMessage = "Enter a valid beginning stock.") }
            return
        }

        viewModelScope.launch {
            _uiState.update { it.copy(isSaving = true, errorMessage = null) }
            val result: Result<MasterSaveOutcome> = if (form.isNew) {
                // Re-generated at save time so two admins can't both claim the previewed sequence.
                val code = masterDataRepository.generateItemCode(
                    form.originNationCode, form.itemTypeCode, form.packagingUnitCode, form.quantityUnitCode
                )
                // Services carry no stock, so any opening quantity typed before switching type is ignored.
                masterDataRepository.registerItem(form.toEntity(code, price), if (form.isService) 0.0 else stock ?: 0.0, operator)
            } else {
                masterDataRepository.updateItem(form.toEntity(form.editingItemCode!!, price), operator)
            }
            _uiState.update { state ->
                result.fold(
                    onSuccess = { outcome ->
                        state.copy(
                            isSaving = false,
                            form = null,
                            message = outcome.pushError?.let { "Saved on device. eTIMS registration pending: $it" }
                                ?: "Item saved and registered with eTIMS."
                        )
                    },
                    onFailure = { e -> state.copy(isSaving = false, errorMessage = e.toUserMessage()) }
                )
            }
        }
    }

    fun clearMessage() {
        _uiState.update { it.copy(message = null) }
    }

    private fun ItemForm.toEntity(code: String, price: Double) = ItemEntity(
        itemCode = code,
        itemClassificationCode = classCode.trim(),
        itemTypeCode = itemTypeCode,
        itemName = itemName.trim(),
        description = description.trim().ifEmpty { null },
        barcode = barcode.trim().ifEmpty { null },
        packagingUnitCode = packagingUnitCode,
        quantityUnitCode = quantityUnitCode,
        taxTypeCode = taxTypeCode,
        defaultUnitPrice = price,
        originNationCode = originNationCode,
        isUsed = isUsed
    )

    private fun plain(value: Double): String = if (value % 1.0 == 0.0) value.toLong().toString() else value.toString()
}
