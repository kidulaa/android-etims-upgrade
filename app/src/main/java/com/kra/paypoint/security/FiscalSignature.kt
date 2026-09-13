package com.kra.paypoint.security

import java.nio.charset.StandardCharsets
import javax.crypto.Cipher
import javax.crypto.Mac
import javax.crypto.spec.SecretKeySpec

/**
 * Client-side eTIMS receipt signing — the "software SDC" pattern the legacy EBM2x till used
 * (no physical Sales Data Controller device; the till itself signs, using keys issued once
 * at device registration). See [com.kra.paypoint.domain.repository.DeviceRepository].
 *
 * Algorithm shape (confirmed against the legacy implementation):
 *  - rcptSign:  HMAC-SHA1(receiptData, signKey), truncated to 10 bytes, Base32-encoded (16 chars).
 *  - intrlData: AES-128-ECB(no padding) of one 16-byte block derived from receiptData, keyed by
 *               intrlKey, Base32-encoded (26 chars).
 * Both keys arrive Base32-encoded from the device-init response and are decoded to raw bytes
 * before use.
 *
 * The exact receiptData concatenation this signs over (field order/format) should be verified
 * against KRA's current eTIMS technical specification before this goes live against production —
 * the legacy source established the crypto primitives, not a field-for-field spec we could
 * independently confirm.
 */
object FiscalSignature {

    fun receiptSignature(receiptData: String, signKeyBase32: String): String {
        val keyBytes = Base32.decode(signKeyBase32)
        val mac = Mac.getInstance("HmacSHA1")
        mac.init(SecretKeySpec(keyBytes, "HmacSHA1"))
        val fullHmac = mac.doFinal(receiptData.toByteArray(StandardCharsets.UTF_8))
        val truncated = fullHmac.copyOf(10)
        return Base32.encode(truncated)
    }

    fun internalData(receiptData: String, intrlKeyBase32: String): String {
        val keyBytes = Base32.decode(intrlKeyBase32).copyOf(16) // AES-128 key
        val cipher = Cipher.getInstance("AES/ECB/NoPadding")
        cipher.init(Cipher.ENCRYPT_MODE, SecretKeySpec(keyBytes, "AES"))
        val block = fixedBlock(receiptData)
        return Base32.encode(cipher.doFinal(block))
    }

    /** Pads or truncates to exactly one AES block (16 bytes) — ECB/NoPadding requires it. */
    private fun fixedBlock(input: String): ByteArray {
        val bytes = input.toByteArray(StandardCharsets.UTF_8)
        if (bytes.size == 16) return bytes
        val block = ByteArray(16)
        System.arraycopy(bytes, 0, block, 0, minOf(bytes.size, 16))
        return block
    }
}
