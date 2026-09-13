package com.kra.paypoint.domain.repository

import com.kra.paypoint.data.local.entity.ZReportEntity
import kotlinx.coroutines.flow.Flow

data class XReportSummary(
    val reportDate: String,
    val readingTimestamp: Long = System.currentTimeMillis(),
    val firstInvoiceNumber: Long = 0L,
    val lastInvoiceNumber: Long = 0L,
    val totalTransactionCount: Int = 0,
    val grossSalesAmount: Double = 0.0,
    val totalTaxableAmount: Double = 0.0,
    val totalTaxAmount: Double = 0.0,
    val taxableAmountA: Double = 0.0,
    val taxAmountA: Double = 0.0,
    val taxableAmountB: Double = 0.0,
    val taxableAmountC: Double = 0.0,
    val taxableAmountD: Double = 0.0,
    val taxableAmountE: Double = 0.0,
    val taxAmountE: Double = 0.0,
    val cashAmount: Double = 0.0,
    val cardAmount: Double = 0.0,
    val mobileAmount: Double = 0.0
)

interface ZReportRepository {
    fun getAllZReports(): Flow<List<ZReportEntity>>
    suspend fun getLiveShiftSummary(): XReportSummary
    suspend fun closeShiftAndGenerateZReport(operatorId: String, operatorName: String): ZReportEntity
    suspend fun getZReport(zReportNumber: Long): ZReportEntity?
}
