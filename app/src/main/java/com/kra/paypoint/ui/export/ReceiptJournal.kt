package com.kra.paypoint.ui.export

import com.google.gson.JsonParser
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.data.local.entity.isCreditNote
import com.kra.paypoint.domain.model.EtimsCodes.PaymentType
import com.kra.paypoint.domain.model.EtimsCodes.TaxType
import com.kra.paypoint.domain.model.device.DeviceRegistration

/** Seller details printed at the top of every receipt; manual overrides from System Settings win. */
data class ReceiptHeader(
    val businessName: String?,
    val address: String?,
    val phone: String?,
    val email: String?
) {
    companion object {
        fun from(
            registration: DeviceRegistration?,
            customAddress: String? = null,
            customPhone: String? = null,
            customEmail: String? = null
        ) = ReceiptHeader(
            businessName = registration?.taxprNm,
            address = customAddress?.takeIf { it.isNotBlank() } ?: registration?.locDesc,
            phone = customPhone?.takeIf { it.isNotBlank() } ?: registration?.mgrTelNo,
            email = customEmail?.takeIf { it.isNotBlank() } ?: registration?.mgrEmail
        )
    }
}

/**
 * Plain-text fiscal receipt for the 32-column thermal printer and for SMS, laid out like the
 * KRA eTIMS sample receipt: seller, cashier, client PIN, lines with tax band, per-band totals,
 * payment, then the CU / signature block that makes it verifiable.
 */
object ReceiptJournal {

    private const val WIDTH = 32
    private val RULE = "-".repeat(WIDTH)

    fun build(
        data: TransactionWithItems,
        header: ReceiptHeader,
        registration: DeviceRegistration?,
        isCopy: Boolean,
        amountReceived: Double? = null
    ): String {
        val t = data.transaction
        val payload = runCatching { JsonParser.parseString(t.payloadJson).asJsonObject }.getOrNull()
        fun field(name: String): String? =
            payload?.get(name)?.takeUnless { it.isJsonNull }?.asString?.takeIf { it.isNotBlank() }

        return buildString {
            header.businessName?.let { appendLine(center(it.uppercase())) }
            header.address?.let { appendLine(center(it)) }
            header.phone?.let { appendLine("TEL: $it") }
            header.email?.let { appendLine("EMAIL: $it") }
            appendLine("PIN: ${field("tin") ?: registration?.tin ?: "-"}")
            field("regrNm")?.let { name -> appendLine("CASHIER: $name(${field("regrId").orEmpty()})") }
            appendLine(RULE)
            appendLine(center(if (t.isCreditNote) "CREDIT NOTE" else "TAX INVOICE"))
            if (isCopy) appendLine(center("*** COPY ***"))
            if (t.isCreditNote) appendLine("REF INVOICE: #${t.originalInvoiceNumber}")
            appendLine(RULE)
            if (!t.customerTin.isNullOrBlank()) appendLine("CLIENT PIN : ${t.customerTin}")
            appendLine("CLIENT NAME: ${t.customerName ?: "Walk-in Customer"}")
            appendLine(RULE)

            data.items.sortedBy { it.itemSequence }.forEach { item ->
                appendLine(item.itemName)
                appendLine(item.itemCode)
                appendLine(
                    spread(
                        "%,.2f x %s".format(item.unitPrice, qty(item.quantity)),
                        "%,.2f %s".format(item.totalAmount, item.taxTypeCode)
                    )
                )
                if (item.discountAmount > 0.0) {
                    appendLine(spread("  Discount ${qty(item.discountRate)}%", "-%,.2f".format(item.discountAmount)))
                }
            }
            appendLine(RULE)
            appendLine(spread("TOTAL", "%,.2f".format(t.totalAmount)))
            bandLine(TaxType.EXEMPT, t.taxableAmountA, t.taxAmountA)
            bandLine(TaxType.STANDARD, t.taxableAmountB, t.taxAmountB)
            bandLine(TaxType.ZERO_RATED, t.taxableAmountC, t.taxAmountC)
            bandLine(TaxType.NON_VAT, t.taxableAmountD, t.taxAmountD)
            bandLine(TaxType.REDUCED, t.taxableAmountE, t.taxAmountE)
            appendLine(spread("TOTAL TAX", "%,.2f".format(t.totalTaxAmount)))
            appendLine(RULE)
            appendLine(spread(PaymentType.label(t.paymentTypeCode).uppercase(), "%,.2f".format(amountReceived ?: t.totalAmount)))
            if (amountReceived != null && amountReceived > t.totalAmount) {
                appendLine(spread("CHANGE", "%,.2f".format(amountReceived - t.totalAmount)))
            }
            if (!t.isPaid) {
                appendLine(spread("BALANCE DUE", "%,.2f".format(t.totalAmount)))
                t.dueDate?.let { appendLine("DUE DATE: ${formatDate(it)}") }
            }
            t.note?.let { appendLine("NOTE: $it") }
            appendLine(spread("ITEMS NUMBER", data.items.size.toString()))
            appendLine(RULE)
            appendLine(center("SCU INFORMATION"))
            appendLine("DATE: ${formatDate(t.salesDate)}")
            registration?.sdcId?.let { appendLine("SCU ID: $it") }
            registration?.mrcNo?.let { appendLine("MRC NO: $it") }
            appendLine("INVOICE NO: ${t.invoiceNumber}")
            val sign = field("rcptSign")
            val internal = field("intrlData")
            if (sign != null) {
                appendLine("INTERNAL DATA:")
                appendLine(internal?.chunked(4)?.joinToString("-") ?: "-")
                appendLine("RECEIPT SIGNATURE:")
                appendLine(sign.chunked(4).joinToString("-"))
            } else {
                appendLine(center("NOT YET SIGNED"))
            }
            appendLine("KRA STATUS: ${t.syncStatus}")
            appendLine(RULE)
            appendLine(center("THANK YOU"))
        }
    }

    /** Compact summary for an SMS to the buyer — the full signature block doesn't fit a text. */
    fun buildSms(data: TransactionWithItems, header: ReceiptHeader): String {
        val t = data.transaction
        val kind = if (t.isCreditNote) "Credit note" else "eTIMS invoice"
        return buildString {
            append("$kind #${t.invoiceNumber}")
            header.businessName?.let { append(" from $it") }
            append(": KES %,.2f (VAT KES %,.2f), ".format(t.totalAmount, t.totalTaxAmount))
            append("${data.items.size} item(s), ${formatDate(t.salesDate)}.")
            t.customerTin?.let { append(" PIN $it.") }
            append(" Thank you.")
        }
    }

    private fun StringBuilder.bandLine(band: String, taxable: Double, tax: Double) {
        if (taxable == 0.0 && tax == 0.0) return
        appendLine(spread("TOTAL ${TaxType.label(band)}", "%,.2f".format(taxable)))
        if (tax != 0.0) appendLine(spread("TOTAL TAX $band", "%,.2f".format(tax)))
    }

    private fun qty(value: Double): String =
        if (value % 1.0 == 0.0) value.toLong().toString() else "%.2f".format(value)

    /** salesDate is yyyyMMdd for sales and yyyyMMddHHmmss for credit notes. */
    internal fun formatDate(raw: String): String = when (raw.length) {
        8 -> "${raw.substring(6, 8)}/${raw.substring(4, 6)}/${raw.substring(0, 4)}"
        14 -> "${raw.substring(6, 8)}/${raw.substring(4, 6)}/${raw.substring(0, 4)} " +
            "${raw.substring(8, 10)}:${raw.substring(10, 12)}:${raw.substring(12, 14)}"
        else -> raw
    }

    private fun spread(left: String, right: String): String {
        val gap = WIDTH - left.length - right.length
        return if (gap >= 1) left + " ".repeat(gap) + right else "$left $right"
    }

    private fun center(text: String): String {
        if (text.length >= WIDTH) return text
        return " ".repeat((WIDTH - text.length) / 2) + text
    }
}
