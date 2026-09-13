package com.kra.paypoint.security

import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotEquals
import org.junit.Test

/**
 * These test the crypto primitives (HMAC-SHA1 truncation + AES-128-ECB block, both Base32
 * encoded) for correctness and determinism. They cannot verify against a real KRA-issued
 * signature — no test vectors for the live eTIMS spec were available — so this is a
 * self-consistency check, not a conformance check. See FiscalSignature's class doc.
 */
class FiscalSignatureTest {

    // 16 raw bytes, Base32-encoded — a syntactically valid stand-in for a KRA-issued key.
    private val signKey = Base32.encode(ByteArray(16) { it.toByte() })
    private val intrlKey = Base32.encode(ByteArray(16) { (it * 3).toByte() })

    @Test
    fun `receiptSignature is 16 characters, matching a truncated-HMAC Base32 encoding`() {
        val signature = FiscalSignature.receiptSignature("P012345678X|00|1|20260912|116.0", signKey)
        assertEquals(16, signature.length)
    }

    @Test
    fun `internalData is 26 characters, matching one Base32-encoded AES block`() {
        val data = FiscalSignature.internalData("P012345678X|00|1|20260912|116.0", intrlKey)
        assertEquals(26, data.length)
    }

    @Test
    fun `same input and key always produce the same signature`() {
        val receiptData = "P012345678X|00|42|20260912|500.0"
        val first = FiscalSignature.receiptSignature(receiptData, signKey)
        val second = FiscalSignature.receiptSignature(receiptData, signKey)
        assertEquals(first, second)
    }

    @Test
    fun `changing a single field changes the signature`() {
        val a = FiscalSignature.receiptSignature("P012345678X|00|1|20260912|116.0", signKey)
        val b = FiscalSignature.receiptSignature("P012345678X|00|2|20260912|116.0", signKey)
        assertNotEquals(a, b)
    }

    @Test
    fun `different keys produce different signatures for the same data`() {
        val otherKey = Base32.encode(ByteArray(16) { (it + 7).toByte() })
        val a = FiscalSignature.receiptSignature("same-data", signKey)
        val b = FiscalSignature.receiptSignature("same-data", otherKey)
        assertNotEquals(a, b)
    }
}
