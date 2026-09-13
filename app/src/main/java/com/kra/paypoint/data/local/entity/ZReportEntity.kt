package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.Index
import androidx.room.PrimaryKey

@Entity(
    tableName = "z_reports",
    indices = [
        Index(value = ["zReportNumber"], unique = true),
        Index(value = ["reportDate"])
    ]
)
data class ZReportEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val zReportNumber: Long, // Strictly monotonic counter (1, 2, 3...)
    val reportDate: String,  // yyyyMMdd
    val closeoutTimestamp: Long = System.currentTimeMillis(),
    val operatorId: String,
    val operatorName: String,
    val firstInvoiceNumber: Long,
    val lastInvoiceNumber: Long,
    val totalTransactionCount: Int,
    val grossSalesAmount: Double,
    val taxableAmountA: Double = 0.0,
    val taxableAmountB: Double = 0.0,
    val taxableAmountC: Double = 0.0,
    val taxableAmountD: Double = 0.0,
    val taxableAmountE: Double = 0.0,
    val taxAmountA: Double = 0.0,
    val taxAmountB: Double = 0.0,
    val taxAmountC: Double = 0.0,
    val taxAmountD: Double = 0.0,
    val taxAmountE: Double = 0.0,
    val totalTaxableAmount: Double,
    val totalTaxAmount: Double,
    val cashAmount: Double = 0.0,
    val cardAmount: Double = 0.0,
    val mobileAmount: Double = 0.0,
    val syncStatus: String = "SYNCED"
)
