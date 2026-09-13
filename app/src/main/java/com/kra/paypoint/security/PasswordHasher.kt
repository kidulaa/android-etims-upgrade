package com.kra.paypoint.security

import java.security.MessageDigest
import java.security.SecureRandom
import java.util.Base64
import javax.crypto.SecretKeyFactory
import javax.crypto.spec.PBEKeySpec

/**
 * PBKDF2-HMAC-SHA256 password hashing for locally-provisioned operator accounts.
 * eTIMS has no cloud login endpoint to verify against, so this is the actual security
 * boundary for who can operate the till — it must never be a bypassable placeholder.
 */
object PasswordHasher {
    private const val ITERATIONS = 120_000
    private const val KEY_LENGTH_BITS = 256
    private const val SALT_LENGTH_BYTES = 16

    fun generateSalt(): ByteArray {
        val salt = ByteArray(SALT_LENGTH_BYTES)
        SecureRandom().nextBytes(salt)
        return salt
    }

    fun hash(password: CharArray, salt: ByteArray): String {
        val spec = PBEKeySpec(password, salt, ITERATIONS, KEY_LENGTH_BITS)
        val factory = SecretKeyFactory.getInstance("PBKDF2WithHmacSHA256")
        val hashBytes = try {
            factory.generateSecret(spec).encoded
        } finally {
            spec.clearPassword()
        }
        return Base64.getEncoder().encodeToString(hashBytes)
    }

    fun verify(password: CharArray, saltBase64: String, expectedHashBase64: String): Boolean {
        val salt = Base64.getDecoder().decode(saltBase64)
        val actualHash = Base64.getDecoder().decode(hash(password, salt))
        val expectedHash = Base64.getDecoder().decode(expectedHashBase64)
        return MessageDigest.isEqual(actualHash, expectedHash)
    }
}
