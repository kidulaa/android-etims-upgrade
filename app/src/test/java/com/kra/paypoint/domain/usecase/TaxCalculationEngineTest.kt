package com.kra.paypoint.domain.usecase

import org.junit.Assert.assertEquals
import org.junit.Test
import java.math.BigDecimal

class TaxCalculationEngineTest {

    @Test
    fun `16 percent band B splits a VAT-inclusive total correctly`() {
        val split = TaxCalculationEngine.split(BigDecimal("116.00"), "B")
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
    fun `exempt, zero-rated and non-VAT bands charge no tax`() {
        for (code in listOf("A", "C", "D")) {
            val split = TaxCalculationEngine.split(BigDecimal("250.00"), code)
            assertEquals("band $code taxable", BigDecimal("250.00"), split.taxableAmount)
            assertEquals("band $code tax", BigDecimal("0.00"), split.taxAmount)
        }
    }

    @Test
    fun `taxable plus tax always reconciles to the original line total to the cent`() {
        // A value that doesn't divide evenly by 1.16, to prove rounding doesn't leak a cent.
        val lineTotal = BigDecimal("99.99")
        val split = TaxCalculationEngine.split(lineTotal, "B")
        assertEquals(lineTotal.setScale(2), split.taxableAmount.add(split.taxAmount))
    }

    @Test
    fun `rate percent matches KRA whole-number convention`() {
        assertEquals(0, TaxCalculationEngine.ratePercentFor("A"))
        assertEquals(16, TaxCalculationEngine.ratePercentFor("B"))
        assertEquals(0, TaxCalculationEngine.ratePercentFor("C"))
        assertEquals(0, TaxCalculationEngine.ratePercentFor("D"))
        assertEquals(8, TaxCalculationEngine.ratePercentFor("E"))
    }

    @Test
    fun `band totals sum per band and reject unknown bands`() {
        val totals = TaxCalculationEngine.bandTotals(
            listOf(
                Triple("B", 100.0, 16.0),
                Triple("B", 50.0, 8.0),
                Triple("A", 65.0, 0.0)
            )
        )
        assertEquals(150.0, totals.taxableFor("B"), 0.0)
        assertEquals(24.0, totals.taxFor("B"), 0.0)
        assertEquals(65.0, totals.taxableFor("A"), 0.0)
        assertEquals(215.0, totals.totalTaxable, 0.0)
        assertEquals(24.0, totals.totalTax, 0.0)
    }

    @Test(expected = IllegalArgumentException::class)
    fun `unknown band is rejected rather than mis-filed`() {
        TaxCalculationEngine.bandTotals(listOf(Triple("Z", 1.0, 0.0)))
    }
}
