package com.kra.paypoint.ui.export

import com.google.gson.Gson
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Test

class ReceiptJournalTest {

    private val payload = Gson().toJson(
        mapOf("tin" to "P012345678X", "regrNm" to "Dunford", "regrId" to "00000", "rcptSign" to "ABCDEFGHIJKLMNOP", "intrlData" to "QRSTUVWXYZ")
    )

    private val sale = TransactionWithItems(
        transaction = TransactionEntity(
            invoiceNumber = 2L,
            customerTin = "A009723435X",
            customerName = "Brian",
            receiptTypeCode = "S",
            totalAmount = 35000.0,
            taxableAmountA = 35000.0,
            totalTaxableAmount = 35000.0,
            salesDate = "20230127",
            payloadJson = payload
        ),
        items = listOf(
            TransactionItemEntity(
                transactionId = 1L, itemSequence = 1, itemCode = "KE2CTNL0000001", itemClassificationCode = "1218160100",
                itemName = "Oils", quantity = 5.0, unitPrice = 7000.0, supplyAmount = 35000.0,
                taxTypeCode = "A", taxableAmount = 35000.0, taxAmount = 0.0, totalAmount = 35000.0
            )
        )
    )

    private val header = ReceiptHeader("HU025 TEST", "Gatundu", "0705634938", null)

    @Test
    fun `sale receipt carries seller PIN, cashier, client PIN, exempt band and signature`() {
        val text = ReceiptJournal.build(sale, header, registration = null, isCopy = false, amountReceived = 35000.0)

        assertTrue(text.contains("PIN: P012345678X"))
        assertTrue(text.contains("CASHIER: Dunford(00000)"))
        assertTrue(text.contains("CLIENT PIN : A009723435X"))
        assertTrue(text.contains("TAX INVOICE"))
        assertTrue(text.contains("TOTAL A-Exempt"))
        assertTrue(text.contains("ABCD-EFGH-IJKL-MNOP"))
        assertFalse(text.contains("COPY"))
        assertFalse(text.contains("CHANGE"))
    }

    @Test
    fun `cash over-tender prints change and reprint is marked as copy`() {
        val text = ReceiptJournal.build(sale, header, registration = null, isCopy = true, amountReceived = 40000.0)
        assertTrue(text.contains("CHANGE"))
        assertTrue(text.contains("5,000.00"))
        assertTrue(text.contains("*** COPY ***"))
    }

    @Test
    fun `credit note is titled and references the original invoice`() {
        val cn = sale.copy(transaction = sale.transaction.copy(invoiceNumber = 3L, originalInvoiceNumber = 2L, receiptTypeCode = "R"))
        val text = ReceiptJournal.build(cn, header, registration = null, isCopy = false)
        assertTrue(text.contains("CREDIT NOTE"))
        assertTrue(text.contains("REF INVOICE: #2"))
    }

    @Test
    fun `dates render for both short sale and full credit-note timestamps`() {
        assertEquals("27/01/2023", ReceiptJournal.formatDate("20230127"))
        assertEquals("27/01/2023 19:05:46", ReceiptJournal.formatDate("20230127190546"))
    }
}
