package com.kra.paypoint.domain.model

import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertNull
import org.junit.Test

class PricedLineTest {

    @Test
    fun `percent discount on a 16 percent line`() {
        val line = PricedLine(unitPrice = 116.0, quantity = 2.0, discount = Discount.Percent(10.0), taxTypeCode = "B")
        assertEquals(232.0, line.supplyAmount, 0.0)
        assertEquals(23.2, line.discountAmount, 0.001)
        assertEquals(208.8, line.lineTotal, 0.001)
        assertEquals(28.8, line.taxAmount, 0.001)
        assertEquals(180.0, line.taxableAmount, 0.001)
    }

    @Test
    fun `fixed discount converts to an effective rate and never exceeds the line`() {
        val line = PricedLine(1000.0, 1.0, Discount.Fixed(250.0), "A")
        assertEquals(25.0, line.discountRate, 0.0)
        assertEquals(750.0, line.lineTotal, 0.0)
        assertEquals(0.0, line.taxAmount, 0.0)

        val over = PricedLine(100.0, 1.0, Discount.Fixed(500.0), "A")
        assertEquals(0.0, over.lineTotal, 0.0)
    }

    @Test
    fun `discount validation`() {
        assertNull(PricedLine.validateDiscount(Discount.Percent(50.0), 100.0))
        assertNotNull(PricedLine.validateDiscount(Discount.Percent(101.0), 100.0))
        assertNotNull(PricedLine.validateDiscount(Discount.Fixed(-1.0), 100.0))
        assertNotNull(PricedLine.validateDiscount(Discount.Fixed(100.01), 100.0))
    }
}
