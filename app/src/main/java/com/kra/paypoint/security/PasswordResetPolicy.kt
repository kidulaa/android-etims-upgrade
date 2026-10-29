package com.kra.paypoint.security

/**
 * Offline "forgot password" rules. With no email or server to prove identity, the operator
 * proves they belong to this till by repeating the device's KRA registration — PIN, branch and
 * device serial — exactly as recorded on their account. Attempts are rate-limited because those
 * details are not secret in the way a password is.
 */
object PasswordResetPolicy {

    const val MAX_FAILED_ATTEMPTS = 5
    const val LOCKOUT_MILLIS = 15 * 60 * 1000L

    data class Claim(val tin: String, val branchId: String, val deviceSerial: String)

    /** Case- and whitespace-insensitive comparison, since these are typed from paperwork. */
    fun matches(account: Claim, entered: Claim): Boolean =
        account.tin.isNotBlank() &&
            normalize(account.tin) == normalize(entered.tin) &&
            normalize(account.branchId) == normalize(entered.branchId) &&
            normalize(account.deviceSerial) == normalize(entered.deviceSerial)

    /** Milliseconds left on an active lockout, or 0 when resets are allowed. */
    fun remainingLockout(lockedUntil: Long, now: Long): Long = (lockedUntil - now).coerceAtLeast(0L)

    /** Lockout end time after a failure, or 0 while still under the attempt limit. */
    fun lockoutAfterFailure(failedAttempts: Int, now: Long): Long =
        if (failedAttempts >= MAX_FAILED_ATTEMPTS) now + LOCKOUT_MILLIS else 0L

    private fun normalize(value: String) = value.trim().uppercase()
}
