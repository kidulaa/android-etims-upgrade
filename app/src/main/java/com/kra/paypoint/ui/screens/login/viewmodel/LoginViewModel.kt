package com.kra.paypoint.ui.screens.login.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.domain.model.auth.User
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
    val isAuthenticated: Boolean = false,
    /** Shown instead of the sign-in form while resetting a forgotten password. */
    val reset: PasswordResetForm? = null,
    val infoMessage: String? = null
)

data class PasswordResetForm(
    val username: String = "",
    val tin: String = "",
    val branchId: String = "",
    val deviceSerial: String = "",
    val newPassword: String = "",
    val confirmPassword: String = "",
    val isSubmitting: Boolean = false,
    val errorMessage: String? = null
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

    fun onUsernameChanged(value: String) = _uiState.update { it.copy(username = value, errorMessage = null, infoMessage = null) }
    fun onPasswordChanged(value: String) = _uiState.update { it.copy(password = value, errorMessage = null, infoMessage = null) }
    fun onFullNameChanged(value: String) = _uiState.update { it.copy(fullName = value, errorMessage = null) }
    fun onConfirmPasswordChanged(value: String) = _uiState.update { it.copy(confirmPassword = value, errorMessage = null) }
    fun onBranchTinChanged(value: String) = _uiState.update { it.copy(branchTin = value, errorMessage = null) }
    fun onBranchIdChanged(value: String) = _uiState.update { it.copy(branchId = value, errorMessage = null) }
    fun onDeviceSerialChanged(value: String) = _uiState.update { it.copy(deviceSerial = value, errorMessage = null) }

    fun startPasswordReset() {
        _uiState.update {
            it.copy(reset = PasswordResetForm(username = it.username.trim()), errorMessage = null, infoMessage = null)
        }
    }

    fun cancelPasswordReset() {
        _uiState.update { it.copy(reset = null) }
    }

    fun updateReset(transform: (PasswordResetForm) -> PasswordResetForm) {
        _uiState.update { s -> s.copy(reset = s.reset?.let(transform)?.copy(errorMessage = null)) }
    }

    fun submitPasswordReset() {
        val form = _uiState.value.reset ?: return
        if (form.isSubmitting) return
        val error = when {
            form.username.isBlank() -> "Enter your user ID."
            form.tin.isBlank() || form.branchId.isBlank() || form.deviceSerial.isBlank() ->
                "Enter the device's KRA PIN, branch ID and device serial."
            form.newPassword.length < 6 -> "Password must be at least 6 characters."
            form.newPassword != form.confirmPassword -> "Passwords do not match."
            else -> null
        }
        if (error != null) {
            _uiState.update { s -> s.copy(reset = s.reset?.copy(errorMessage = error)) }
            return
        }

        _uiState.update { s -> s.copy(reset = s.reset?.copy(isSubmitting = true, errorMessage = null)) }
        viewModelScope.launch {
            val result = authRepository.resetForgottenPassword(
                username = form.username,
                tin = form.tin,
                branchId = form.branchId,
                deviceSerial = form.deviceSerial,
                newPassword = form.newPassword
            )
            _uiState.update { s ->
                result.fold(
                    onSuccess = {
                        s.copy(
                            reset = null,
                            username = form.username.trim(),
                            password = "",
                            infoMessage = "Password reset. Sign in with your new password."
                        )
                    },
                    onFailure = { e ->
                        s.copy(reset = s.reset?.copy(isSubmitting = false, errorMessage = e.toUserMessage()))
                    }
                )
            }
        }
    }

    fun login() {
        if (_uiState.value.isLoading) return
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
                            errorMessage = error.toUserMessage()
                        )
                    }
                }
            )
        }
    }

    /** First-run device setup: provisions the first (admin) operator account. */
    fun completeFirstRunSetup() {
        if (_uiState.value.isLoading) return
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
                authorityCode = User.ROLE_ADMIN,
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
                                        "Device eTIMS registration did not complete: ${e.toUserMessage()} " +
                                            "Retry it from Hardware Settings before processing real sales."
                                    }
                                )
                            }
                        },
                        onFailure = { error ->
                            _uiState.update {
                                it.copy(isLoading = false, errorMessage = error.toUserMessage())
                            }
                        }
                    )
                },
                onFailure = { error ->
                    _uiState.update {
                        it.copy(isLoading = false, errorMessage = error.toUserMessage())
                    }
                }
            )
        }
    }
}
