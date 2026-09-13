package com.kra.paypoint.security

import org.junit.Assert.assertFalse
import org.junit.Assert.assertNotEquals
import org.junit.Assert.assertTrue
import org.junit.Test
import java.util.Base64

class PasswordHasherTest {

    @Test
    fun `correct password verifies successfully`() {
        val salt = PasswordHasher.generateSalt()
        val hash = PasswordHasher.hash("correct-horse-battery-staple".toCharArray(), salt)
        val saltB64 = Base64.getEncoder().encodeToString(salt)

        assertTrue(PasswordHasher.verify("correct-horse-battery-staple".toCharArray(), saltB64, hash))
    }

    @Test
    fun `wrong password is rejected`() {
        val salt = PasswordHasher.generateSalt()
        val hash = PasswordHasher.hash("correct-horse-battery-staple".toCharArray(), salt)
        val saltB64 = Base64.getEncoder().encodeToString(salt)

        assertFalse(PasswordHasher.verify("wrong-password".toCharArray(), saltB64, hash))
    }

    @Test
    fun `same password hashed twice produces different output due to random salt`() {
        val hashOne = PasswordHasher.hash("password123".toCharArray(), PasswordHasher.generateSalt())
        val hashTwo = PasswordHasher.hash("password123".toCharArray(), PasswordHasher.generateSalt())

        assertNotEquals(hashOne, hashTwo)
    }
}
