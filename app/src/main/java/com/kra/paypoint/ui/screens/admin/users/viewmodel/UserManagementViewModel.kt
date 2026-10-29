package com.kra.paypoint.ui.screens.admin.users.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

/** [editingUserId] null means the form creates a new user. */
data class UserForm(
    val editingUserId: String? = null,
    val userCode: String = "",
    val userName: String = "",
    val password: String = "",
    val telNo: String = "",
    val isAdmin: Boolean = false
)

data class UserManagementUiState(
    val form: UserForm? = null,
    val isSaving: Boolean = false,
    val message: String? = null,
    val errorMessage: String? = null
)

@HiltViewModel
class UserManagementViewModel @Inject constructor(
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(UserManagementUiState())
    val uiState: StateFlow<UserManagementUiState> = _uiState.asStateFlow()

    val users: StateFlow<List<User>> = authRepository.observeUsers()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    val currentUser: StateFlow<User?> = authRepository.currentUser

    fun startNew() {
        _uiState.update { it.copy(form = UserForm(), errorMessage = null, message = null) }
    }

    fun startEdit(user: User) {
        _uiState.update {
            it.copy(
                form = UserForm(
                    editingUserId = user.id,
                    userCode = user.id,
                    userName = user.name,
                    telNo = user.telNo.orEmpty(),
                    isAdmin = user.isAdmin
                ),
                errorMessage = null,
                message = null
            )
        }
    }

    fun dismissForm() {
        _uiState.update { it.copy(form = null, errorMessage = null) }
    }

    fun updateForm(transform: (UserForm) -> UserForm) {
        _uiState.update { state -> state.copy(form = state.form?.let(transform), errorMessage = null) }
    }

    fun save() {
        // Re-entry guard: a double tap can land before the disabled button recomposes.
        if (_uiState.value.isSaving) return
        val form = _uiState.value.form ?: return
        val admin = authRepository.currentUser.value
        if (admin == null || !admin.isAdmin) {
            _uiState.update { it.copy(errorMessage = "Only an admin can manage users.") }
            return
        }
        val role = if (form.isAdmin) User.ROLE_ADMIN else User.ROLE_USER

        viewModelScope.launch {
            _uiState.update { it.copy(isSaving = true, errorMessage = null) }
            val result = if (form.editingUserId == null) {
                authRepository.registerLocalUser(
                    username = form.userCode,
                    password = form.password,
                    fullName = form.userName,
                    authorityCode = role,
                    branchId = admin.branchId,
                    tin = admin.tin,
                    deviceSerial = admin.deviceSerial,
                    telNo = form.telNo
                )
            } else {
                authRepository.updateLocalUser(
                    username = form.editingUserId,
                    fullName = form.userName,
                    telNo = form.telNo,
                    authorityCode = role,
                    newPassword = form.password.takeIf { it.isNotEmpty() }
                )
            }
            _uiState.update { state ->
                result.fold(
                    onSuccess = { state.copy(isSaving = false, form = null, message = "Saved successfully.") },
                    onFailure = { e -> state.copy(isSaving = false, errorMessage = e.toUserMessage()) }
                )
            }
        }
    }

    fun setActive(user: User, active: Boolean) {
        viewModelScope.launch {
            val result = authRepository.setUserActive(user.id, active)
            _uiState.update { state ->
                result.fold(
                    onSuccess = { state.copy(message = "${user.name} ${if (active) "activated" else "deactivated"}.") },
                    onFailure = { e -> state.copy(message = e.toUserMessage()) }
                )
            }
        }
    }

    fun clearMessage() {
        _uiState.update { it.copy(message = null) }
    }
}
