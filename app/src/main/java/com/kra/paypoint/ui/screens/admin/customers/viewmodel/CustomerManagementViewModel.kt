package com.kra.paypoint.ui.screens.admin.customers.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import com.kra.paypoint.domain.usecase.MasterDataValidator
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

/** [existing] null means a new customer; the PIN is the key, so it can't change once saved. */
data class CustomerForm(
    val existing: CustomerEntity? = null,
    val customerType: String = MasterDataValidator.CUSTOMER_INDIVIDUAL,
    val pin: String = "",
    val name: String = "",
    val phone: String = "",
    val email: String = "",
    val isUsed: Boolean = true
) {
    val isNew: Boolean get() = existing == null
}

data class CustomerManagementUiState(
    val form: CustomerForm? = null,
    val isSaving: Boolean = false,
    val message: String? = null,
    val errorMessage: String? = null
)

@HiltViewModel
class CustomerManagementViewModel @Inject constructor(
    private val masterDataRepository: MasterDataRepository,
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(CustomerManagementUiState())
    val uiState: StateFlow<CustomerManagementUiState> = _uiState.asStateFlow()

    val customers: StateFlow<List<CustomerEntity>> = masterDataRepository.getAllCustomersIncludingUnused()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    fun startNew() {
        _uiState.update { it.copy(form = CustomerForm(), errorMessage = null, message = null) }
    }

    fun startEdit(customer: CustomerEntity) {
        _uiState.update {
            it.copy(
                form = CustomerForm(
                    existing = customer,
                    customerType = customer.customerType,
                    pin = customer.customerTin,
                    name = customer.customerName,
                    phone = customer.telNo.orEmpty(),
                    email = customer.email.orEmpty(),
                    isUsed = customer.isUsed
                ),
                errorMessage = null,
                message = null
            )
        }
    }

    fun dismissForm() {
        _uiState.update { it.copy(form = null, errorMessage = null) }
    }

    fun updateForm(transform: (CustomerForm) -> CustomerForm) {
        _uiState.update { state -> state.copy(form = state.form?.let(transform), errorMessage = null) }
    }

    fun save() {
        // Re-entry guard: a double tap can land before the disabled button recomposes.
        if (_uiState.value.isSaving) return
        val form = _uiState.value.form ?: return
        val operator = authRepository.currentUser.value
        if (operator == null || !operator.isAdmin) {
            _uiState.update { it.copy(errorMessage = "Only an admin can manage customers.") }
            return
        }
        viewModelScope.launch {
            if (form.isNew && masterDataRepository.getCustomerByTin(form.pin.trim().uppercase()) != null) {
                _uiState.update { it.copy(errorMessage = "A customer with PIN ${form.pin.trim().uppercase()} already exists.") }
                return@launch
            }
            _uiState.update { it.copy(isSaving = true, errorMessage = null) }
            val base = form.existing ?: CustomerEntity(customerTin = form.pin, customerName = form.name)
            val result = masterDataRepository.registerCustomer(
                base.copy(
                    customerTin = form.pin,
                    customerName = form.name,
                    customerType = form.customerType,
                    telNo = form.phone,
                    email = form.email,
                    isUsed = form.isUsed
                ),
                operator
            )
            _uiState.update { state ->
                result.fold(
                    onSuccess = { outcome ->
                        state.copy(
                            isSaving = false,
                            form = null,
                            message = outcome.pushError?.let { "Saved on device. eTIMS registration pending: $it" }
                                ?: "Customer saved and registered with eTIMS."
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
}
