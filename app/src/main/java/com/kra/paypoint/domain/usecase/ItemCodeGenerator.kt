package com.kra.paypoint.domain.usecase

/**
 * KRA item codes are `{origin country}{item type}{packaging unit}{quantity unit}{7-digit sequence}`,
 * e.g. `KE2NTU0000001` = Kenyan finished product, net packaging, sold per piece, 1st of its kind.
 */
object ItemCodeGenerator {

    private const val SEQUENCE_DIGITS = 7

    fun prefix(originNationCode: String, itemTypeCode: String, packagingUnitCode: String, quantityUnitCode: String): String =
        listOf(originNationCode, itemTypeCode, packagingUnitCode, quantityUnitCode)
            .joinToString("") { it.trim().uppercase() }

    /** Next code after the highest sequence already used under [prefix]; codes with a non-numeric tail are ignored. */
    fun next(prefix: String, existingCodes: List<String>): String {
        val maxSequence = existingCodes
            .filter { it.length == prefix.length + SEQUENCE_DIGITS && it.startsWith(prefix) }
            .mapNotNull { it.substring(prefix.length).toLongOrNull() }
            .maxOrNull() ?: 0L
        return prefix + (maxSequence + 1).toString().padStart(SEQUENCE_DIGITS, '0')
    }
}
