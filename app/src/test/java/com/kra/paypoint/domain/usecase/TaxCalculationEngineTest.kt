package com.kra.paypoint.domain.usecase

import org.junit.Assert.assertEquals
import org.junit.Test
import java.math.BigDecimal

class TaxCalculationEngineTest {

    @Test
    fun `16 percent band splits a VAT-inclusive total correctly`() {
        val split = TaxCalculationEngine.split(BigDecimal("116.00"), "A")
        assertEquals(BigDecimal("100.00"), split.taxableAmount)
        assertEquals(BigDecimal("16.00"), split.taxAmount)
    }

    @Test
    fun `8 percent band splits a VAT-inclusive total correctly`() {
        val split = TaxCalculationEngine.split(BigDecimal("108.00"), "E")
        assertEquals(BigDecimal("100.00"), split.taxableAmount)
        assertEquals(BigDecimal("8.00"), split.taxAmount)
    }

    @Test
    fun `zero-rated bands charge no tax`() {
        for (code in listOf("B", "C", "D")) {
            val split = TaxCalculationEngine.split(BigDecimal("250.00"), code)
            assertEquals("band $code taxable", BigDecimal("250.00"), split.taxableAmount)
            assertEquals("band $code tax", BigDecimal("0.00"), split.taxAmount)
        }
    }

    @Test
    fun `taxable plus tax always reconciles to the original line total to the cent`() {
        // A value that doesn't divide evenly by 1.16, to prove rounding doesn't leak a cent.
        val lineTotal = BigDecimal("99.99")
        val split = TaxCalculationEngine.split(lineTotal, "A")
        assertEquals(lineTotal.setScale(2), split.taxableAmount.add(split.taxAmount))
    }

    @Test
    fun `rate percent matches KRA whole-number convention`() {
        assertEquals(16, TaxCalculationEngine.ratePercentFor("A"))
        assertEquals(8, TaxCalculationEngine.ratePercentFor("E"))
        assertEquals(0, TaxCalculationEngine.ratePercentFor("B"))
    }
}
