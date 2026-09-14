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
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
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
        deviceSerial: String
    ): Result<Unit> = withContext(Dispatchers.IO) {
        val trimmedUsername = username.trim()
        if (trimmedUsername.isBlank() || fullName.isBlank()) {
            return@withContext Result.failure(IllegalArgumentException("Username and full name are required."))
        }
        if (password.length < 6) {
            return@withContext Result.failure(IllegalArgumentException("Password must be at least 6 characters."))
        }

        // Check if user already exists to prevent silent overwrite via upsert
        val existing = userDao.getByUsername(trimmedUsername)
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
                passwordHash = hash,
                passwordSalt = java.util.Base64.getEncoder().encodeToString(salt)
            )
        )
        Result.success(Unit)
    }

    private fun UserEntity.toDomain() = User(
        id = username,
        name = fullName,
        authorityCode = authorityCode,
        branchId = branchId,
        tin = tin,
        deviceSerial = deviceSerial
    )

    private companion object {
        const val KEY_SESSION_USERNAME = "session_username"
    }
}
