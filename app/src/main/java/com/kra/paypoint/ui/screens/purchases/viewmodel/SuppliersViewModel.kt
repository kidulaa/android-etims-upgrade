package com.kra.paypoint.ui.screens.purchases.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.SupplierEntity
import com.kra.paypoint.domain.repository.PurchaseRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

data class SupplierForm(
    val isNew: Boolean = true,
    val pin: String = "",
    val name: String = "",
    val phone: String = "",
    val email: String = "",
    val isUsed: Boolean = true
)

data class SuppliersUiState(
    val form: SupplierForm? = null,
    val isSaving: Boolean = false,
    val message: String? = null,
    val errorMessage: String? = null
)

@HiltViewModel
class SuppliersViewModel @Inject constructor(
    private val purchaseRepository: PurchaseRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(SuppliersUiState())
    val uiState: StateFlow<SuppliersUiState> = _uiState.asStateFlow()

    val suppliers: StateFlow<List<SupplierEntity>> = purchaseRepository.observeSuppliers()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    fun startNew() = _uiState.update { it.copy(form = SupplierForm(), errorMessage = null) }

    fun startEdit(s: SupplierEntity) = _uiState.update {
        it.copy(form = SupplierForm(false, s.supplierTin, s.supplierName, s.telNo.orEmpty(), s.email.orEmpty(), s.isUsed), errorMessage = null)
    }

    fun dismiss() = _uiState.update { it.copy(form = null, errorMessage = null) }

    fun updateForm(transform: (SupplierForm) -> SupplierForm) =
        _uiState.update { s -> s.copy(form = s.form?.let(transform), errorMessage = null) }

    fun save() {
        val form = _uiState.value.form ?: return
        if (_uiState.value.isSaving) return
        viewModelScope.launch {
            _uiState.update { it.copy(isSaving = true) }
            if (form.isNew && purchaseRepository.observeSuppliersOnce(form.pin.trim().uppercase()) != null) {
                _uiState.update { it.copy(isSaving = false, errorMessage = "A supplier with this PIN already exists.") }
                return@launch
            }
            val result = purchaseRepository.saveSupplier(
                SupplierEntity(supplierTin = form.pin, supplierName = form.name, telNo = form.phone, email = form.email, isUsed = form.isUsed)
            )
            _uiState.update { s ->
                result.fold(
                    onSuccess = { s.copy(isSaving = false, form = null, message = "Supplier saved.") },
                    onFailure = { e -> s.copy(isSaving = false, errorMessage = e.toUserMessage()) }
                )
            }
        }
    }

    fun messageShown() = _uiState.update { it.copy(message = null) }
}
