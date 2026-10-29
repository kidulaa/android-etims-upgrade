package com.kra.paypoint.ui.screens.dashboard.viewmodel

import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.isCreditNote
import java.time.Instant
import java.time.LocalDate
import java.time.ZoneId

/** "Today at a glance" for the home screen, derived from local data so it works offline. */
data class HomeSnapshot(
    val salesToday: Double = 0.0,
    val invoicesToday: Int = 0,
    val vatToday: Double = 0.0,
    val refundsToday: Int = 0,
    val unsynced: Int = 0,
    val lowStock: List<ItemEntity> = emptyList(),
    val recent: List<TransactionEntity> = emptyList()
) {
    companion object {
        /** Goods at or below this quantity are flagged; services carry no stock. */
        const val LOW_STOCK_THRESHOLD = 5.0
        private const val RECENT_COUNT = 5
        private const val SERVICE_ITEM_TYPE = "3"

        fun from(
            transactions: List<TransactionEntity>,
            items: List<ItemEntity>,
            today: LocalDate = LocalDate.now(),
            zone: ZoneId = ZoneId.systemDefault()
        ): HomeSnapshot {
            val todays = transactions.filter { Instant.ofEpochMilli(it.createdAt).atZone(zone).toLocalDate() == today }
            val (refunds, sales) = todays.partition { it.isCreditNote }
            return HomeSnapshot(
                salesToday = sales.sumOf { it.totalAmount } - refunds.sumOf { it.totalAmount },
                invoicesToday = sales.size,
                vatToday = sales.sumOf { it.totalTaxAmount } - refunds.sumOf { it.totalTaxAmount },
                refundsToday = refunds.size,
                unsynced = transactions.count { it.syncStatus != "SYNCED" },
                lowStock = items
                    .filter { it.isUsed && it.itemTypeCode != SERVICE_ITEM_TYPE && it.stockQuantity <= LOW_STOCK_THRESHOLD }
                    .sortedBy { it.stockQuantity },
                recent = transactions.sortedByDescending { it.createdAt }.take(RECENT_COUNT)
            )
        }
    }
}
