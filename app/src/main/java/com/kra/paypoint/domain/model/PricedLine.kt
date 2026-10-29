package com.kra.paypoint.domain.model

import com.kra.paypoint.domain.usecase.TaxCalculationEngine
import java.math.BigDecimal
import java.math.RoundingMode

/** A line discount, entered either way the eTIMS guide allows: a percentage or a fixed KES amount. */
sealed interface Discount {
    data class Percent(val rate: Double) : Discount
    data class Fixed(val amount: Double) : Discount

    companion object {
        val NONE: Discount = Percent(0.0)
    }
}

/**
 * Money for one line of any document (sale, quotation, LPO, purchase): VAT-inclusive
 * unit price × quantity, less discount, split into taxable and VAT for its band.
 */
data class PricedLine(
    val unitPrice: Double,
    val quantity: Double,
    val discount: Discount,
    val taxTypeCode: String
) {
    /** prc × qty before discount — KRA's splyAmt. */
    val supplyAmount: Double get() = round2(unitPrice * quantity)

    /** Never more than the line itself, so a large fixed discount can't produce a negative line. */
    val discountAmount: Double
        get() = when (discount) {
            is Discount.Percent -> round2(supplyAmount * discount.rate / 100.0)
            is Discount.Fixed -> round2(discount.amount)
        }.coerceIn(0.0, supplyAmount)

    /** Effective discount rate, as KRA's dcRt expects even when the discount was entered as an amount. */
    val discountRate: Double
        get() = if (supplyAmount == 0.0) 0.0 else round2(discountAmount / supplyAmount * 100.0)

    val lineTotal: Double get() = round2(supplyAmount - discountAmount)

    private val split get() = TaxCalculationEngine.split(lineTotal, taxTypeCode)
    val taxableAmount: Double get() = split.taxableAmount.toDouble()
    val taxAmount: Double get() = split.taxAmount.toDouble()

    companion object {
        fun round2(value: Double): Double = BigDecimal.valueOf(value).setScale(2, RoundingMode.HALF_UP).toDouble()

        /** Discount fields are validated against what the UI lets a user type. */
        fun validateDiscount(discount: Discount, supplyAmount: Double): String? = when (discount) {
            is Discount.Percent -> if (discount.rate < 0.0 || discount.rate > 100.0) "Discount must be between 0% and 100%." else null
            is Discount.Fixed -> if (discount.amount < 0.0 || discount.amount > supplyAmount) "Discount cannot exceed the line amount." else null
        }
    }
}

/** An item on a document together with how it's priced on that document. */
data class DocumentLine(
    val itemCode: String,
    val itemName: String,
    val line: PricedLine
)
