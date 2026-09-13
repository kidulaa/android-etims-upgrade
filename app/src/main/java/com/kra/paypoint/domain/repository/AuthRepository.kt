package com.kra.paypoint.domain.repository

import com.kra.paypoint.domain.model.auth.User
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
        tin: String
    ): Result<Unit>
}
