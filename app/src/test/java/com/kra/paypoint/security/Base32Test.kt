package com.kra.paypoint.security

import org.junit.Assert.assertArrayEquals
import org.junit.Assert.assertEquals
import org.junit.Test
import java.nio.charset.StandardCharsets

class Base32Test {

    @Test
    fun `encode matches known RFC 4648 test vectors`() {
        // https://www.rfc-editor.org/rfc/rfc4648#section-10 (padding characters stripped)
        assertEquals("", Base32.encode("".toByteArray(StandardCharsets.US_ASCII)))
        assertEquals("MY", Base32.encode("f".toByteArray(StandardCharsets.US_ASCII)))
        assertEquals("MZXQ", Base32.encode("fo".toByteArray(StandardCharsets.US_ASCII)))
        assertEquals("MZXW6", Base32.encode("foo".toByteArray(StandardCharsets.US_ASCII)))
        assertEquals("MZXW6YQ", Base32.encode("foob".toByteArray(StandardCharsets.US_ASCII)))
        assertEquals("MZXW6YTB", Base32.encode("fooba".toByteArray(StandardCharsets.US_ASCII)))
        assertEquals("MZXW6YTBOI", Base32.encode("foobar".toByteArray(StandardCharsets.US_ASCII)))
    }

    @Test
    fun `decode is the inverse of encode for arbitrary bytes`() {
        val original = byteArrayOf(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -128, 127)
        val roundTripped = Base32.decode(Base32.encode(original))
        assertArrayEquals(original, roundTripped)
    }

    @Test
    fun `a 16-byte AES block encodes to 26 characters, a 10-byte HMAC truncation to 16`() {
        // These exact lengths are what make rcptSign (16 chars) and intrlData (26 chars)
        // come out the sizes eTIMS expects.
        assertEquals(16, Base32.encode(ByteArray(10)).length)
        assertEquals(26, Base32.encode(ByteArray(16)).length)
    }
}
