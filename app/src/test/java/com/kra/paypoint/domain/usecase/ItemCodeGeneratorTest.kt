package com.kra.paypoint.domain.usecase

import org.junit.Assert.assertEquals
import org.junit.Test

class ItemCodeGeneratorTest {

    @Test
    fun `prefix is origin, item type, packaging unit and quantity unit`() {
        assertEquals("KE2NTU", ItemCodeGenerator.prefix("ke", "2", "NT", "u"))
    }

    @Test
    fun `first code under a prefix is sequence 1 padded to 7 digits`() {
        assertEquals("KE2NTU0000001", ItemCodeGenerator.next("KE2NTU", emptyList()))
    }

    @Test
    fun `next code follows the highest existing sequence, not the count`() {
        val existing = listOf("KE2NTU0000001", "KE2NTU0000007", "KE2NTU0000003")
        assertEquals("KE2NTU0000008", ItemCodeGenerator.next("KE2NTU", existing))
    }

    @Test
    fun `codes that only share a longer prefix or have a non-numeric tail are ignored`() {
        // "KE2NTUKG..." belongs to the KG quantity unit; "KE2NTU-CUSTOM" was hand-entered.
        val existing = listOf("KE2NTUKG00009", "KE2NTU-CUSTOM", "KE2NTU0000002")
        assertEquals("KE2NTU0000003", ItemCodeGenerator.next("KE2NTU", existing))
    }
}
