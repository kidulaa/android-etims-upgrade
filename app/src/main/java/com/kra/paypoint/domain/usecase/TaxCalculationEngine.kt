package com.kra.paypoint.domain.usecase

import java.math.BigDecimal
import java.math.RoundingMode

/**
 * Single source of truth for KRA VAT-band splitting. Previously this math (divide a
 * VAT-inclusive line total by 1 + rate) was duplicated across SalesViewModel,
 * CreditNoteViewModel and TransactionRepositoryImpl using raw Double, with no rounding —
 * each copy could drift from the others by a cent and none of them matched what got sent
 * to KRA. Every caller now goes through here, with BigDecimal HALF_UP rounding to 2dp
 * matching how KES amounts are actually invoiced.
 */
object TaxCalculationEngine {

    /** KRA tax-band VAT rates. B, C and D are 0%-rated/exempt bands. */
    val RATE_A: BigDecimal = BigDecimal("0.16")
    val RATE_E: BigDecimal = BigDecimal("0.08")

    data class Split(
        val taxableAmount: BigDecimal,
        val taxAmount: BigDecimal
    )

    /** Splits a VAT-inclusive [lineTotal] into taxable and tax portions for the given [taxTypeCode]. */
    fun split(lineTotal: BigDecimal, taxTypeCode: String): Split {
        val rate = rateFor(taxTypeCode)
        if (rate.signum() == 0) {
            return Split(taxableAmount = lineTotal.round2(), taxAmount = BigDecimal.ZERO.round2())
        }
        val taxable = lineTotal.divide(BigDecimal.ONE.add(rate), 10, RoundingMode.HALF_UP).round2()
        val tax = lineTotal.round2().subtract(taxable)
        return Split(taxableAmount = taxable, taxAmount = tax)
    }

    fun split(lineTotal: Double, taxTypeCode: String): Split = split(lineTotal.toBigDecimal(), taxTypeCode)

    fun rateFor(taxTypeCode: String): BigDecimal = when (taxTypeCode) {
        "A" -> RATE_A
        "E" -> RATE_E
        else -> BigDecimal.ZERO
    }

    /** KRA sends tax rates as whole-number percentages (16, not 0.16). */
    fun ratePercentFor(taxTypeCode: String): Int = when (taxTypeCode) {
        "A" -> 16
        "E" -> 8
        else -> 0
    }

    private fun BigDecimal.round2(): BigDecimal = setScale(2, RoundingMode.HALF_UP)
}
