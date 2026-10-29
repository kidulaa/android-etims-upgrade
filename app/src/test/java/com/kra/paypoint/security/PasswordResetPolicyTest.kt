package com.kra.paypoint.security

import com.kra.paypoint.security.PasswordResetPolicy.Claim
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Test

class PasswordResetPolicyTest {

    private val account = Claim(tin = "P051234567Z", branchId = "00", deviceSerial = "MOBILE001")

    @Test
    fun `matching registration details verify, ignoring case and stray spaces`() {
        assertTrue(PasswordResetPolicy.matches(account, Claim(" p051234567z ", "00", "mobile001")))
    }

    @Test
    fun `any mismatched detail fails`() {
        assertFalse(PasswordResetPolicy.matches(account, Claim("P051234567Z", "01", "MOBILE001")))
        assertFalse(PasswordResetPolicy.matches(account, Claim("P051234567Z", "00", "MOBILE002")))
        assertFalse(PasswordResetPolicy.matches(account, Claim("A012345678Z", "00", "MOBILE001")))
    }

    @Test
    fun `an account without a recorded PIN can never be reset this way`() {
        val blank = Claim("", "", "")
        assertFalse(PasswordResetPolicy.matches(blank, Claim("", "", "")))
    }

    @Test
    fun `lockout starts only at the attempt limit and then expires`() {
        val now = 1_000_000L
        assertEquals(0L, PasswordResetPolicy.lockoutAfterFailure(PasswordResetPolicy.MAX_FAILED_ATTEMPTS - 1, now))
        val until = PasswordResetPolicy.lockoutAfterFailure(PasswordResetPolicy.MAX_FAILED_ATTEMPTS, now)
        assertEquals(now + PasswordResetPolicy.LOCKOUT_MILLIS, until)
        assertTrue(PasswordResetPolicy.remainingLockout(until, now + 60_000) > 0)
        assertEquals(0L, PasswordResetPolicy.remainingLockout(until, until + 1))
    }
}
