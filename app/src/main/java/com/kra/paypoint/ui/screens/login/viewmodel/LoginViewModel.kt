package com.kra.paypoint.ui.screens.login.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.domain.repository.AuthRepository
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
    val isLoading: Boolean = false,
    val errorMessage: String? = null,
    val isAuthenticated: Boolean = false
)

@HiltViewModel
class LoginViewModel @Inject constructor(
    private val authRepository: AuthRepository
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
            state.branchTin.isBlank() || state.branchId.isBlank() -> {
                _uiState.update { it.copy(errorMessage = "Branch TIN and branch ID are required.") }
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
                tin = state.branchTin
            )
            registerResult.fold(
                onSuccess = {
                    val loginResult = authRepository.login(state.username, state.password)
                    loginResult.fold(
                        onSuccess = { _uiState.update { it.copy(isLoading = false, isAuthenticated = true) } },
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
