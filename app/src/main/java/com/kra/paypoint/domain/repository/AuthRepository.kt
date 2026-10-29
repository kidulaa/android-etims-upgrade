package com.kra.paypoint.domain.repository

import com.kra.paypoint.domain.model.auth.User
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.StateFlow

interface AuthRepository {
    val currentUser: StateFlow<User?>
    val isSessionResolved: StateFlow<Boolean>

    suspend fun login(username: String, password: String): Result<User>
    suspend fun logout()
    fun isAuthenticated(): Boolean
    suspend fun getCurrentUser(): User?

    /** True once at least one local operator account has been provisioned on this device. */
    suspend fun hasAnyUsers(): Boolean

    /**
     * Provisions a local operator account. Used by the first-run device setup flow and by
     * admin-only user management — never seeded with placeholder/demo credentials.
     */
    suspend fun registerLocalUser(
        username: String,
        password: String,
        fullName: String,
        authorityCode: String,
        branchId: String,
        tin: String,
        deviceSerial: String,
        telNo: String? = null
    ): Result<Unit>

    /** Every local account, active or not, for admin user management. */
    fun observeUsers(): Flow<List<User>>

    /** Updates profile/role, and the password only when [newPassword] is non-null. */
    suspend fun updateLocalUser(
        username: String,
        fullName: String,
        telNo: String?,
        authorityCode: String,
        newPassword: String?
    ): Result<Unit>

    /**
     * Offline "forgot password": sets a new password when [tin], [branchId] and [deviceSerial]
     * match the account's recorded KRA registration. Failures share one message (no user
     * enumeration) and repeated failures lock resets out for a while.
     */
    suspend fun resetForgottenPassword(
        username: String,
        tin: String,
        branchId: String,
        deviceSerial: String,
        newPassword: String
    ): Result<Unit>

    /** Refuses to deactivate the signed-in operator or the last active admin. */
    suspend fun setUserActive(username: String, active: Boolean): Result<Unit>
}
