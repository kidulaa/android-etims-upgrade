package com.kra.paypoint.ui.screens.login.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

data class LoginUiState(
    val checkingSetupState: Boolean = true,
    val needsFirstRunSetup: Boolean = false,
    val username: String = "",
    val password: String = "",
    val fullName: String = "",
    val confirmPassword: String = "",
    val branchTin: String = "",
    val branchId: String = "",
    val deviceSerial: String = "",
    val isLoading: Boolean = false,
    val errorMessage: String? = null,
    val deviceWarning: String? = null,
    val isAuthenticated: Boolean = false
)

@HiltViewModel
class LoginViewModel @Inject constructor(
    private val authRepository: AuthRepository,
    private val deviceRepository: DeviceRepository,
    private val masterDataRepository: MasterDataRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(LoginUiState())
    val uiState: StateFlow<LoginUiState> = _uiState.asStateFlow()

    init {
        viewModelScope.launch {
            val needsSetup = !authRepository.hasAnyUsers()
            _uiState.update {
                it.copy(checkingSetupState = false, needsFirstRunSetup = needsSetup)
            }
        }
        viewModelScope.launch {
            authRepository.currentUser.collect { user ->
                if (user != null) {
                    _uiState.update { it.copy(isAuthenticated = true) }
                }
            }
        }
    }

    fun onUsernameChanged(value: String) = _uiState.update { it.copy(username = value, errorMessage = null) }
    fun onPasswordChanged(value: String) = _uiState.update { it.copy(password = value, errorMessage = null) }
    fun onFullNameChanged(value: String) = _uiState.update { it.copy(fullName = value, errorMessage = null) }
    fun onConfirmPasswordChanged(value: String) = _uiState.update { it.copy(confirmPassword = value, errorMessage = null) }
    fun onBranchTinChanged(value: String) = _uiState.update { it.copy(branchTin = value, errorMessage = null) }
    fun onBranchIdChanged(value: String) = _uiState.update { it.copy(branchId = value, errorMessage = null) }
    fun onDeviceSerialChanged(value: String) = _uiState.update { it.copy(deviceSerial = value, errorMessage = null) }

    fun login() {
        val state = _uiState.value
        if (state.username.isBlank() || state.password.isBlank()) {
            _uiState.update { it.copy(errorMessage = "Please enter username and password") }
            return
        }

        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, errorMessage = null) }
            val result = authRepository.login(state.username, state.password)
            result.fold(
                onSuccess = {
                    _uiState.update { it.copy(isLoading = false, isAuthenticated = true) }
                },
                onFailure = { error ->
                    _uiState.update {
                        it.copy(
                            isLoading = false,
                            errorMessage = error.localizedMessage ?: "Authentication failed"
                        )
                    }
                }
            )
        }
    }

    /** First-run device setup: provisions the first (admin) operator account. */
    fun completeFirstRunSetup() {
        val state = _uiState.value
        when {
            state.fullName.isBlank() || state.username.isBlank() -> {
                _uiState.update { it.copy(errorMessage = "Full name and username are required.") }
                return
            }
            state.branchTin.isBlank() || state.branchId.isBlank() || state.deviceSerial.isBlank() -> {
                _uiState.update { it.copy(errorMessage = "Branch TIN, ID and Device Serial are required.") }
                return
            }
            state.password.length < 6 -> {
                _uiState.update { it.copy(errorMessage = "Password must be at least 6 characters.") }
                return
            }
            state.password != state.confirmPassword -> {
                _uiState.update { it.copy(errorMessage = "Passwords do not match.") }
                return
            }
        }

        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, errorMessage = null) }
            val registerResult = authRepository.registerLocalUser(
                username = state.username,
                password = state.password,
                fullName = state.fullName,
                authorityCode = "ROLE_ADMIN",
                branchId = state.branchId,
                tin = state.branchTin,
                deviceSerial = state.deviceSerial
            )
            registerResult.fold(
                onSuccess = {
                    val loginResult = authRepository.login(state.username, state.password)
                    loginResult.fold(
                        onSuccess = {
                            // Best-effort: this needs live connectivity to KRA, which an
                            // offline-first till may not have during setup. Failing here
                            // must not block account creation — it blocks signed sales
                            // later instead (see DeviceRepository.signReceipt), with a
                            // clear warning surfaced now so it isn't a silent gap.
                            val deviceResult = deviceRepository.registerDevice(state.branchTin, state.branchId, state.deviceSerial)
                            
                            // Background background sync of master data if device registration succeeded
                            deviceResult.onSuccess {
                                viewModelScope.launch {
                                    masterDataRepository.syncReferenceCodesFromEtims(state.branchTin, state.branchId)
                                    masterDataRepository.syncItemClassificationsFromEtims(state.branchTin, state.branchId)
                                    masterDataRepository.syncItemsFromEtims(state.branchTin, state.branchId)
                                    masterDataRepository.syncCustomersFromEtims(state.branchTin, state.branchId)
                                }
                            }

                            _uiState.update {
                                it.copy(
                                    isLoading = false,
                                    isAuthenticated = true,
                                    deviceWarning = deviceResult.exceptionOrNull()?.let { e ->
                                        "Device eTIMS registration did not complete (${e.localizedMessage}). " +
                                            "Retry it from Hardware Settings before processing real sales."
                                    }
                                )
                            }
                        },
                        onFailure = { error ->
                            _uiState.update {
                                it.copy(isLoading = false, errorMessage = error.localizedMessage ?: "Setup failed")
                            }
                        }
                    )
                },
                onFailure = { error ->
                    _uiState.update {
                        it.copy(isLoading = false, errorMessage = error.localizedMessage ?: "Setup failed")
                    }
                }
            )
        }
    }
}
