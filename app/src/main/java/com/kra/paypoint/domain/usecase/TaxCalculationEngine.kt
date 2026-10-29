package com.kra.paypoint.domain.usecase

import com.kra.paypoint.domain.model.EtimsCodes.TaxType
import java.math.BigDecimal
import java.math.RoundingMode

/**
 * Single source of truth for KRA VAT-band splitting. Previously this math (divide a
 * VAT-inclusive line total by 1 + rate) was duplicated across SalesViewModel,
 * CreditNoteViewModel and TransactionRepositoryImpl using raw Double, with no rounding —
 * each copy could drift from the others by a cent and none of them matched what got sent
 * to KRA. Every caller now goes through here, with BigDecimal HALF_UP rounding to 2dp
 * matching how KES amounts are actually invoiced.
 *
 * KRA bands: A = Exempt, B = 16% standard, C = 0% zero-rated, D = Non-VAT, E = 8%.
 */
object TaxCalculationEngine {

    val RATE_B: BigDecimal = BigDecimal("0.16")
    val RATE_E: BigDecimal = BigDecimal("0.08")

    val BANDS = listOf(TaxType.EXEMPT, TaxType.STANDARD, TaxType.ZERO_RATED, TaxType.NON_VAT, TaxType.REDUCED)

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
        TaxType.STANDARD -> RATE_B
        TaxType.REDUCED -> RATE_E
        else -> BigDecimal.ZERO
    }

    /** KRA sends tax rates as whole-number percentages (16, not 0.16). */
    fun ratePercentFor(taxTypeCode: String): Int = when (taxTypeCode) {
        TaxType.STANDARD -> 16
        TaxType.REDUCED -> 8
        else -> 0
    }

    /** Per-band totals for a set of lines, as KRA's taxblAmtX / taxAmtX fields expect. */
    data class BandTotals(
        val taxable: Map<String, Double>,
        val tax: Map<String, Double>
    ) {
        fun taxableFor(band: String): Double = taxable[band] ?: 0.0
        fun taxFor(band: String): Double = tax[band] ?: 0.0
        val totalTaxable: Double get() = taxable.values.sum()
        val totalTax: Double get() = tax.values.sum()
    }

    /** [lines] are (taxTypeCode, taxableAmount, taxAmount). Unknown bands are rejected rather than silently mis-filed. */
    fun bandTotals(lines: List<Triple<String, Double, Double>>): BandTotals {
        val taxable = BANDS.associateWith { BigDecimal.ZERO }.toMutableMap()
        val tax = BANDS.associateWith { BigDecimal.ZERO }.toMutableMap()
        for ((band, taxableAmt, taxAmt) in lines) {
            require(band in BANDS) { "Unknown KRA tax type '$band'" }
            taxable[band] = taxable.getValue(band).add(taxableAmt.toBigDecimal())
            tax[band] = tax.getValue(band).add(taxAmt.toBigDecimal())
        }
        return BandTotals(
            taxable = taxable.mapValues { it.value.round2().toDouble() },
            tax = tax.mapValues { it.value.round2().toDouble() }
        )
    }

    private fun BigDecimal.round2(): BigDecimal = setScale(2, RoundingMode.HALF_UP)
}
