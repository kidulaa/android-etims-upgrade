package com.kra.paypoint.security

/**
 * RFC 4648 Base32 (no padding). eTIMS signing keys and the computed receipt signature /
 * internal data are all exchanged Base32-encoded — this has no equivalent in the JDK
 * (only Base64 does), so it's implemented directly rather than pulled in as a dependency
 * for ~30 lines of code.
 */
object Base32 {
    private const val ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"

    fun encode(data: ByteArray): String {
        if (data.isEmpty()) return ""
        val output = StringBuilder((data.size * 8 + 4) / 5)
        var buffer = 0L
        var bitsLeft = 0
        for (b in data) {
            buffer = (buffer shl 8) or (b.toLong() and 0xFF)
            bitsLeft += 8
            while (bitsLeft >= 5) {
                bitsLeft -= 5
                val index = ((buffer shr bitsLeft) and 0x1F).toInt()
                output.append(ALPHABET[index])
            }
        }
        if (bitsLeft > 0) {
            val index = ((buffer shl (5 - bitsLeft)) and 0x1F).toInt()
            output.append(ALPHABET[index])
        }
        return output.toString()
    }

    fun decode(encoded: String): ByteArray {
        val clean = encoded.trim().uppercase().trimEnd('=')
        if (clean.isEmpty()) return ByteArray(0)
        val output = ArrayList<Byte>((clean.length * 5) / 8 + 1)
        var buffer = 0L
        var bitsLeft = 0
        for (c in clean) {
            val index = ALPHABET.indexOf(c)
            require(index >= 0) { "Invalid Base32 character: $c" }
            buffer = (buffer shl 5) or index.toLong()
            bitsLeft += 5
            if (bitsLeft >= 8) {
                bitsLeft -= 8
                output.add(((buffer shr bitsLeft) and 0xFF).toByte())
            }
        }
        return output.toByteArray()
    }
}
