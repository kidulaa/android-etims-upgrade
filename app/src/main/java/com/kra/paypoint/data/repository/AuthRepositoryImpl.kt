package com.kra.paypoint.data.repository

import android.content.Context
import android.content.SharedPreferences
import androidx.security.crypto.EncryptedSharedPreferences
import androidx.security.crypto.MasterKey
import com.kra.paypoint.data.local.dao.UserDao
import com.kra.paypoint.data.local.entity.UserEntity
import com.kra.paypoint.domain.exception.InvalidCredentialsException
import com.kra.paypoint.domain.exception.UserAlreadyExistsException
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.security.PasswordHasher
import com.kra.paypoint.security.PasswordResetPolicy
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject
import javax.inject.Singleton

/**
 * eTIMS has no cloud "login" endpoint for cashiers — the taxpayer's device/branch users are
 * provisioned once (KRA `saveBhfUser`) and thereafter authenticate locally, including fully
 * offline. So the local, salted/hashed [UserEntity] table is the actual security boundary,
 * not a remote call — there is nothing to "verify against the server" on every login.
 */
@Singleton
class AuthRepositoryImpl @Inject constructor(
    private val userDao: UserDao,
    @ApplicationContext context: Context
) : AuthRepository {

    private val securePrefs: SharedPreferences by lazy {
        val masterKey = MasterKey.Builder(context)
            .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
            .build()
        EncryptedSharedPreferences.create(
            context,
            "etims_secure_session",
            masterKey,
            EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
            EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
        )
    }

    private val repoScope = CoroutineScope(SupervisorJob() + Dispatchers.IO)

    private val _currentUser = MutableStateFlow<User?>(null)
    override val currentUser: StateFlow<User?> = _currentUser.asStateFlow()

    private val _isSessionResolved = MutableStateFlow(false)
    override val isSessionResolved: StateFlow<Boolean> = _isSessionResolved.asStateFlow()

    init {
        repoScope.launch {
            val savedUsername = securePrefs.getString(KEY_SESSION_USERNAME, null)
            if (!savedUsername.isNullOrBlank()) {
                _currentUser.value = userDao.getByUsername(savedUsername)?.toDomain()
            }
            _isSessionResolved.value = true
        }
    }

    override suspend fun login(username: String, password: String): Result<User> =
        withContext(Dispatchers.IO) {
            if (username.isBlank() || password.isBlank()) {
                return@withContext Result.failure(
                    IllegalArgumentException("Username and password cannot be empty.")
                )
            }

            val entity = userDao.getByUsername(username.trim())
                ?: return@withContext Result.failure(InvalidCredentialsException())

            val verified = PasswordHasher.verify(
                password = password.toCharArray(),
                saltBase64 = entity.passwordSalt,
                expectedHashBase64 = entity.passwordHash
            )
            if (!verified) {
                return@withContext Result.failure(InvalidCredentialsException())
            }

            securePrefs.edit().putString(KEY_SESSION_USERNAME, entity.username).apply()
            val user = entity.toDomain()
            _currentUser.value = user
            Result.success(user)
        }

    override suspend fun logout() {
        securePrefs.edit().remove(KEY_SESSION_USERNAME).apply()
        _currentUser.value = null
    }

    override fun isAuthenticated(): Boolean = _currentUser.value != null

    override suspend fun getCurrentUser(): User? = _currentUser.value

    override suspend fun hasAnyUsers(): Boolean = withContext(Dispatchers.IO) {
        userDao.countUsers() > 0
    }

    override suspend fun registerLocalUser(
        username: String,
        password: String,
        fullName: String,
        authorityCode: String,
        branchId: String,
        tin: String,
        deviceSerial: String,
        telNo: String?
    ): Result<Unit> = withContext(Dispatchers.IO) {
        val trimmedUsername = username.trim()
        if (trimmedUsername.isBlank() || fullName.isBlank()) {
            return@withContext Result.failure(IllegalArgumentException("Username and full name are required."))
        }
        if (password.length < 6) {
            return@withContext Result.failure(IllegalArgumentException("Password must be at least 6 characters."))
        }

        // Check if user already exists (active or not) to prevent silent overwrite via upsert
        val existing = userDao.findAny(trimmedUsername)
        if (existing != null) {
            return@withContext Result.failure(UserAlreadyExistsException(trimmedUsername))
        }

        val salt = PasswordHasher.generateSalt()
        val hash = PasswordHasher.hash(password.toCharArray(), salt)

        userDao.upsert(
            UserEntity(
                username = trimmedUsername,
                fullName = fullName.trim(),
                authorityCode = authorityCode,
                branchId = branchId.trim(),
                tin = tin.trim(),
                deviceSerial = deviceSerial.trim(),
                telNo = telNo?.trim()?.takeIf { it.isNotEmpty() },
                passwordHash = hash,
                passwordSalt = java.util.Base64.getEncoder().encodeToString(salt)
            )
        )
        Result.success(Unit)
    }

    override fun observeUsers(): Flow<List<User>> = userDao.observeAll().map { list -> list.map { it.toDomain() } }

    override suspend fun updateLocalUser(
        username: String,
        fullName: String,
        telNo: String?,
        authorityCode: String,
        newPassword: String?
    ): Result<Unit> = withContext(Dispatchers.IO) {
        val existing = userDao.findAny(username)
            ?: return@withContext Result.failure(IllegalArgumentException("User '$username' not found."))
        if (fullName.isBlank()) {
            return@withContext Result.failure(IllegalArgumentException("User name is required."))
        }
        if (newPassword != null && newPassword.length < 6) {
            return@withContext Result.failure(IllegalArgumentException("Password must be at least 6 characters."))
        }
        val demotingAdmin = existing.authorityCode == User.ROLE_ADMIN && authorityCode != User.ROLE_ADMIN
        if (demotingAdmin && existing.isActive && userDao.countActiveWithAuthority(User.ROLE_ADMIN) <= 1) {
            return@withContext Result.failure(IllegalStateException("At least one active admin is required."))
        }

        var updated = existing.copy(
            fullName = fullName.trim(),
            telNo = telNo?.trim()?.takeIf { it.isNotEmpty() },
            authorityCode = authorityCode
        )
        if (newPassword != null) {
            val salt = PasswordHasher.generateSalt()
            updated = updated.copy(
                passwordHash = PasswordHasher.hash(newPassword.toCharArray(), salt),
                passwordSalt = java.util.Base64.getEncoder().encodeToString(salt)
            )
        }
        userDao.upsert(updated)
        if (_currentUser.value?.id == username) {
            _currentUser.value = updated.toDomain()
        }
        Result.success(Unit)
    }

    override suspend fun resetForgottenPassword(
        username: String,
        tin: String,
        branchId: String,
        deviceSerial: String,
        newPassword: String
    ): Result<Unit> = withContext(Dispatchers.IO) {
        val now = System.currentTimeMillis()
        val remaining = PasswordResetPolicy.remainingLockout(securePrefs.getLong(KEY_RESET_LOCKED_UNTIL, 0L), now)
        if (remaining > 0) {
            val minutes = (remaining + 59_999) / 60_000
            return@withContext Result.failure(
                IllegalStateException("Too many attempts. Try again in $minutes minute(s), or ask an admin to reset it.")
            )
        }
        if (newPassword.length < 6) {
            return@withContext Result.failure(IllegalArgumentException("Password must be at least 6 characters."))
        }

        // Deactivated accounts can't sign in, so they can't reset either.
        val account = userDao.getByUsername(username.trim())
        val verified = account != null && PasswordResetPolicy.matches(
            PasswordResetPolicy.Claim(account.tin, account.branchId, account.deviceSerial),
            PasswordResetPolicy.Claim(tin, branchId, deviceSerial)
        )
        if (!verified) {
            val failures = securePrefs.getInt(KEY_RESET_FAILURES, 0) + 1
            val lockedUntil = PasswordResetPolicy.lockoutAfterFailure(failures, now)
            securePrefs.edit()
                .putInt(KEY_RESET_FAILURES, if (lockedUntil > 0) 0 else failures)
                .putLong(KEY_RESET_LOCKED_UNTIL, lockedUntil)
                .apply()
            return@withContext Result.failure(
                IllegalArgumentException("Those details don't match an active user on this device.")
            )
        }

        val salt = PasswordHasher.generateSalt()
        userDao.upsert(
            account!!.copy(
                passwordHash = PasswordHasher.hash(newPassword.toCharArray(), salt),
                passwordSalt = java.util.Base64.getEncoder().encodeToString(salt)
            )
        )
        securePrefs.edit().remove(KEY_RESET_FAILURES).remove(KEY_RESET_LOCKED_UNTIL).apply()
        Result.success(Unit)
    }

    override suspend fun setUserActive(username: String, active: Boolean): Result<Unit> = withContext(Dispatchers.IO) {
        val existing = userDao.findAny(username)
            ?: return@withContext Result.failure(IllegalArgumentException("User '$username' not found."))
        if (!active) {
            if (_currentUser.value?.id == username) {
                return@withContext Result.failure(IllegalStateException("You cannot deactivate your own account."))
            }
            if (existing.authorityCode == User.ROLE_ADMIN && existing.isActive &&
                userDao.countActiveWithAuthority(User.ROLE_ADMIN) <= 1
            ) {
                return@withContext Result.failure(IllegalStateException("At least one active admin is required."))
            }
        }
        userDao.setActive(username, active)
        Result.success(Unit)
    }

    private fun UserEntity.toDomain() = User(
        id = username,
        name = fullName,
        authorityCode = authorityCode,
        branchId = branchId,
        tin = tin,
        deviceSerial = deviceSerial,
        telNo = telNo,
        isActive = isActive
    )

    private companion object {
        const val KEY_SESSION_USERNAME = "session_username"
        const val KEY_RESET_FAILURES = "reset_failures"
        const val KEY_RESET_LOCKED_UNTIL = "reset_locked_until"
    }
}
