package com.kra.paypoint.ui.screens.systemsetting.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.domain.model.device.DeviceRegistration
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.PreferencesRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

data class SystemSettingsUiState(
    val deviceRegistration: DeviceRegistration? = null,
    val email: String = "",
    val phone: String = "",
    val address: String = "",
    val isNonVat: Boolean = false,
    val isSaving: Boolean = false,
    val saveSuccess: Boolean = false
)

@HiltViewModel
class SystemSettingsViewModel @Inject constructor(
    private val deviceRepository: DeviceRepository,
    private val preferencesRepository: PreferencesRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(SystemSettingsUiState())
    val uiState: StateFlow<SystemSettingsUiState> = _uiState.asStateFlow()

    init {
        viewModelScope.launch {
            combine(
                deviceRepository.registration,
                preferencesRepository.customEmail,
                preferencesRepository.customPhone,
                preferencesRepository.customAddress,
                preferencesRepository.nonVatFlag
            ) { reg, email, phone, addr, nonVat ->
                _uiState.update { state ->
                    state.copy(
                        deviceRegistration = reg,
                        email = email ?: reg?.mgrEmail.orEmpty(),
                        phone = phone ?: reg?.mgrTelNo.orEmpty(),
                        address = addr ?: reg?.locDesc.orEmpty(),
                        isNonVat = nonVat
                    )
                }
            }.collect {}
        }
    }

    fun updateEmail(email: String) {
        _uiState.update { it.copy(email = email, saveSuccess = false) }
    }

    fun updatePhone(phone: String) {
        _uiState.update { it.copy(phone = phone, saveSuccess = false) }
    }

    fun updateAddress(address: String) {
        _uiState.update { it.copy(address = address, saveSuccess = false) }
    }

    fun updateNonVat(isNonVat: Boolean) {
        _uiState.update { it.copy(isNonVat = isNonVat, saveSuccess = false) }
    }

    fun saveSettings() {
        viewModelScope.launch {
            _uiState.update { it.copy(isSaving = true, saveSuccess = false) }
            
            val state = _uiState.value
            preferencesRepository.setCustomEmail(state.email)
            preferencesRepository.setCustomPhone(state.phone)
            preferencesRepository.setCustomAddress(state.address)
            preferencesRepository.setNonVatFlag(state.isNonVat)
            
            _uiState.update { it.copy(isSaving = false, saveSuccess = true) }
        }
    }
}
