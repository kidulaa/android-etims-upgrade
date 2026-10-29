package com.kra.paypoint.ui.screens.documents.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import androidx.lifecycle.SavedStateHandle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.DocumentType
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.SupplierEntity
import com.kra.paypoint.domain.model.Discount
import com.kra.paypoint.domain.model.DocumentLine
import com.kra.paypoint.domain.model.EtimsCodes.PaymentType
import com.kra.paypoint.domain.model.EtimsCodes.TaxType
import com.kra.paypoint.domain.model.PricedLine
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.DocumentParty
import com.kra.paypoint.domain.repository.DocumentRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import com.kra.paypoint.domain.repository.PreferencesRepository
import com.kra.paypoint.domain.repository.PurchaseRepository
import com.kra.paypoint.domain.repository.PurchaseRequest
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.flatMapLatest
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import java.time.LocalDate
import java.time.format.DateTimeFormatter
import javax.inject.Inject

/** What the editor is producing. Purchase invoices are fiscal; the other three are not. */
enum class EditorKind(val title: String, val docType: String?) {
    QUOTATION("Quotation", DocumentType.QUOTATION),
    PROFORMA("Proforma Invoice", DocumentType.PROFORMA),
    LPO("Local Purchase Order", DocumentType.LPO),
    PURCHASE_INVOICE("Purchase Invoice", null);

    /** LPOs and purchase invoices are addressed to a supplier; quotations/proformas to a customer. */
    val isPurchaseSide: Boolean get() = this == LPO || this == PURCHASE_INVOICE
}

data class EditorLine(
    val item: ItemEntity,
    val quantity: Double,
    val unitPrice: Double,
    val discount: Discount,
    val taxTypeCode: String
) {
    val priced: PricedLine get() = PricedLine(unitPrice, quantity, discount, taxTypeCode)
}

data class DocumentEditorUiState(
    val kind: EditorKind = EditorKind.QUOTATION,
    val existingId: Long? = null,
    val sourceLpoId: Long? = null,
    val partyTin: String? = null,
    val partyName: String = "",
    val partyPhone: String? = null,
    val selectedSupplier: SupplierEntity? = null,
    val dueInDays: Int? = 30,
    val note: String = "",
    val supplierInvoiceNumber: String = "",
    val paymentTypeCode: String = PaymentType.CASH,
    val restockInventory: Boolean = true,
    val lines: List<EditorLine> = emptyList(),
    val isSaving: Boolean = false,
    val savedId: Long? = null,
    val errorMessage: String? = null
) {
    val total: Double get() = PricedLine.round2(lines.sumOf { it.priced.lineTotal })
    val tax: Double get() = PricedLine.round2(lines.sumOf { it.priced.taxAmount })
    val discount: Double get() = PricedLine.round2(lines.sumOf { it.priced.discountAmount })
}

@HiltViewModel
class DocumentEditorViewModel @Inject constructor(
    savedStateHandle: SavedStateHandle,
    private val documentRepository: DocumentRepository,
    private val purchaseRepository: PurchaseRepository,
    private val masterDataRepository: MasterDataRepository,
    private val authRepository: AuthRepository,
    private val preferencesRepository: PreferencesRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(
        DocumentEditorUiState(
            kind = savedStateHandle.get<String>(ARG_KIND)?.let { runCatching { EditorKind.valueOf(it) }.getOrNull() } ?: EditorKind.QUOTATION
        )
    )
    val uiState: StateFlow<DocumentEditorUiState> = _uiState.asStateFlow()

    val items: StateFlow<List<ItemEntity>> = masterDataRepository.getAllItems()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    private val partyQuery = MutableStateFlow("")

    @OptIn(ExperimentalCoroutinesApi::class)
    val customers: StateFlow<List<CustomerEntity>> = partyQuery
        .flatMapLatest { q -> if (q.isBlank()) masterDataRepository.getAllCustomers() else masterDataRepository.searchCustomers(q) }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    @OptIn(ExperimentalCoroutinesApi::class)
    val suppliers: StateFlow<List<SupplierEntity>> = partyQuery
        .flatMapLatest { q -> if (q.isBlank()) purchaseRepository.observeSuppliers() else purchaseRepository.searchSuppliers(q) }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    init {
        val existingId = savedStateHandle.get<Long>(ARG_ID)?.takeIf { it > 0 }
        val fromLpo = savedStateHandle.get<Long>(ARG_FROM_LPO)?.takeIf { it > 0 }
        (existingId ?: fromLpo)?.let { loadDocument(it, asNewPurchase = fromLpo != null) }
        if (_uiState.value.kind.isPurchaseSide) _uiState.update { it.copy(dueInDays = null) }
    }

    private fun loadDocument(id: Long, asNewPurchase: Boolean) {
        viewModelScope.launch {
            val doc = documentRepository.get(id) ?: return@launch
            val lines = doc.items.sortedBy { it.itemSequence }.mapNotNull { line ->
                masterDataRepository.getItemByCode(line.itemCode)?.let { item ->
                    EditorLine(
                        item = item,
                        quantity = line.quantity,
                        unitPrice = line.unitPrice,
                        discount = if (line.discountAmount > 0) Discount.Fixed(line.discountAmount) else Discount.NONE,
                        taxTypeCode = line.taxTypeCode
                    )
                }
            }
            val supplier = if (_uiState.value.kind.isPurchaseSide) {
                doc.document.partyTin?.let { tin -> purchaseRepository.observeSuppliersOnce(tin) }
            } else null
            val days = doc.document.dueDate?.let { due ->
                runCatching {
                    java.time.temporal.ChronoUnit.DAYS.between(LocalDate.now(), LocalDate.parse(due, DateTimeFormatter.BASIC_ISO_DATE)).toInt().coerceAtLeast(0)
                }.getOrNull()
            }
            _uiState.update {
                it.copy(
                    existingId = if (asNewPurchase) null else id,
                    sourceLpoId = if (asNewPurchase) id else null,
                    partyTin = doc.document.partyTin,
                    partyName = doc.document.partyName,
                    partyPhone = doc.document.partyPhone,
                    selectedSupplier = supplier,
                    dueInDays = if (it.kind.isPurchaseSide) null else days,
                    note = doc.document.note.orEmpty(),
                    lines = lines
                )
            }
        }
    }

    fun onPartyQueryChanged(query: String) {
        partyQuery.value = query
    }

    fun selectCustomer(customer: CustomerEntity) {
        _uiState.update { it.copy(partyTin = customer.customerTin, partyName = customer.customerName, partyPhone = customer.telNo, errorMessage = null) }
    }

    /** Quotations/proformas may go to someone not yet registered — the guide lets you just key in a name. */
    fun setPartyName(name: String) {
        _uiState.update { it.copy(partyTin = null, partyName = name, partyPhone = null, errorMessage = null) }
    }

    fun selectSupplier(supplier: SupplierEntity) {
        _uiState.update {
            it.copy(selectedSupplier = supplier, partyTin = supplier.supplierTin, partyName = supplier.supplierName, partyPhone = supplier.telNo, errorMessage = null)
        }
    }

    fun update(transform: (DocumentEditorUiState) -> DocumentEditorUiState) {
        _uiState.update { transform(it).copy(errorMessage = null) }
    }

    /** Adds or replaces (same item) a line; validates the discount against the line amount. */
    fun upsertLine(item: ItemEntity, quantity: Double, unitPrice: Double, discount: Discount) {
        val band = if (!_uiState.value.kind.isPurchaseSide && preferencesRepository.nonVatFlag.value) TaxType.NON_VAT else item.taxTypeCode
        val line = EditorLine(item, quantity, unitPrice, discount, band)
        val error = when {
            quantity <= 0.0 -> "Quantity must be greater than zero."
            unitPrice < 0.0 -> "Unit cost cannot be negative."
            else -> PricedLine.validateDiscount(discount, line.priced.supplyAmount)
        }
        if (error != null) {
            _uiState.update { it.copy(errorMessage = error) }
            return
        }
        _uiState.update { state ->
            val idx = state.lines.indexOfFirst { it.item.itemCode == item.itemCode }
            val lines = if (idx >= 0) state.lines.toMutableList().apply { this[idx] = line } else state.lines + line
            state.copy(lines = lines, errorMessage = null)
        }
    }

    fun removeLine(itemCode: String) {
        _uiState.update { it.copy(lines = it.lines.filterNot { l -> l.item.itemCode == itemCode }) }
    }

    fun save() {
        // Re-entry guard: a double tap can land before the disabled button recomposes.
        if (_uiState.value.isSaving) return
        val state = _uiState.value
        val operator = authRepository.currentUser.value
        if (operator == null) {
            _uiState.update { it.copy(errorMessage = "Sign in again to save.") }
            return
        }
        if (state.kind.isPurchaseSide && !operator.isAdmin) {
            _uiState.update { it.copy(errorMessage = "Only an admin can record purchases.") }
            return
        }
        val docLines = state.lines.map { DocumentLine(it.item.itemCode, it.item.itemName, it.priced) }

        viewModelScope.launch {
            _uiState.update { it.copy(isSaving = true, errorMessage = null) }
            val result: Result<Long> = when (state.kind) {
                EditorKind.PURCHASE_INVOICE -> {
                    val supplier = state.selectedSupplier
                    if (supplier == null) {
                        Result.failure(IllegalArgumentException("Select the supplier."))
                    } else {
                        purchaseRepository.recordPurchase(
                            PurchaseRequest(
                                supplier = supplier,
                                supplierInvoiceNumber = state.supplierInvoiceNumber,
                                purchaseDate = LocalDate.now().format(DateTimeFormatter.BASIC_ISO_DATE),
                                paymentTypeCode = state.paymentTypeCode,
                                note = state.note,
                                lines = docLines,
                                restockInventory = state.restockInventory,
                                sourceLpoId = state.sourceLpoId
                            ),
                            operator
                        ).onSuccess { purchaseId ->
                            state.sourceLpoId?.let { lpo ->
                                val number = purchaseRepository.getPurchase(purchaseId)?.purchase?.purchaseNumber
                                documentRepository.markConverted(lpo, "PUR ${number ?: purchaseId}")
                            }
                        }
                    }
                }
                else -> {
                    if (state.kind == EditorKind.LPO && state.selectedSupplier == null) {
                        Result.failure(IllegalArgumentException("Select the supplier."))
                    } else {
                        documentRepository.save(
                            docType = state.kind.docType!!,
                            existingId = state.existingId,
                            party = DocumentParty(state.partyTin, state.partyName, state.partyPhone),
                            dueDate = state.dueInDays?.let { LocalDate.now().plusDays(it.toLong()).format(DateTimeFormatter.BASIC_ISO_DATE) },
                            note = state.note,
                            lines = docLines,
                            operator = operator
                        )
                    }
                }
            }
            _uiState.update { s ->
                result.fold(
                    onSuccess = { id -> s.copy(isSaving = false, savedId = id) },
                    onFailure = { e -> s.copy(isSaving = false, errorMessage = e.toUserMessage()) }
                )
            }
        }
    }

    companion object {
        const val ARG_KIND = "kind"
        const val ARG_ID = "id"
        const val ARG_FROM_LPO = "fromLpo"
    }
}
