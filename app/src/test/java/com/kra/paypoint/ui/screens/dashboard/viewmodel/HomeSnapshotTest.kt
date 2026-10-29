package com.kra.paypoint.ui.screens.dashboard.viewmodel

import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.TransactionEntity
import org.junit.Assert.assertEquals
import org.junit.Test
import java.time.LocalDate
import java.time.ZoneOffset

class HomeSnapshotTest {

    private val zone = ZoneOffset.UTC
    private val today = LocalDate.of(2026, 10, 28)
    private fun at(day: LocalDate, hour: Int) = day.atTime(hour, 0).toInstant(zone).toEpochMilli()

    private fun tx(id: Long, amount: Double, vat: Double, created: Long, original: Long = 0, status: String = "SYNCED") =
        TransactionEntity(
            id = id, invoiceNumber = id, originalInvoiceNumber = original, totalAmount = amount, totalTaxAmount = vat,
            salesDate = "20261028", payloadJson = "{}", createdAt = created, syncStatus = status
        )

    private fun item(code: String, stock: Double, type: String = "2", used: Boolean = true) =
        ItemEntity(itemCode = code, itemClassificationCode = "5020", itemTypeCode = type, itemName = code, stockQuantity = stock, isUsed = used)

    @Test
    fun `today's totals net off credit notes and ignore other days`() {
        val snap = HomeSnapshot.from(
            transactions = listOf(
                tx(1, 1160.0, 160.0, at(today, 9)),
                tx(2, 580.0, 80.0, at(today, 11)),
                tx(3, 116.0, 16.0, at(today, 12), original = 1), // credit note
                tx(4, 9999.0, 999.0, at(today.minusDays(1), 15), status = "PENDING")
            ),
            items = emptyList(), today = today, zone = zone
        )
        assertEquals(1624.0, snap.salesToday, 0.001)
        assertEquals(2, snap.invoicesToday)
        assertEquals(224.0, snap.vatToday, 0.001)
        assertEquals(1, snap.refundsToday)
        assertEquals(1, snap.unsynced) // yesterday's pending invoice still counts
        assertEquals(listOf(3L, 2L, 1L, 4L), snap.recent.map { it.id })
    }

    @Test
    fun `low stock lists active goods only, emptiest first`() {
        val snap = HomeSnapshot.from(
            transactions = emptyList(),
            items = listOf(
                item("plenty", 50.0),
                item("few", 3.0),
                item("none", 0.0),
                item("service", 0.0, type = "3"),
                item("retired", 1.0, used = false)
            ),
            today = today, zone = zone
        )
        assertEquals(listOf("none", "few"), snap.lowStock.map { it.itemCode })
    }
}
