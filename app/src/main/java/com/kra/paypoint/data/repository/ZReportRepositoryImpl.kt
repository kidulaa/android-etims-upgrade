package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.TransactionDao
import com.kra.paypoint.data.local.dao.ZReportDao
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.ZReportEntity
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.XReportSummary
import com.kra.paypoint.domain.repository.ZReportRepository
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.first
import java.time.LocalDate
import java.time.format.DateTimeFormatter
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class ZReportRepositoryImpl @Inject constructor(
    private val transactionDao: TransactionDao,
    private val zReportDao: ZReportDao,
    private val deviceRepository: DeviceRepository
) : ZReportRepository {

    override fun getAllZReports(): Flow<List<ZReportEntity>> {
        return zReportDao.getAllZReports()
    }

    override suspend fun getLiveShiftSummary(): XReportSummary {
        val lastZ = zReportDao.getLastZReport()
        val shiftStart = lastZ?.closeoutTimestamp ?: 0L

        // Retrieve transactions committed since the last Z-Closeout
        val allTx = transactionDao.getAllTransactions().first()
        val shiftTransactions = allTx.filter { it.createdAt > shiftStart }

        val todayStr = LocalDate.now().format(DateTimeFormatter.ofPattern("yyyyMMdd"))

        if (shiftTransactions.isEmpty()) {
            return XReportSummary(reportDate = todayStr)
        }

        val sorted = shiftTransactions.sortedBy { it.createdAt }

        return XReportSummary(
            reportDate = todayStr,
            readingTimestamp = System.currentTimeMillis(),
            firstInvoiceNumber = sorted.first().invoiceNumber,
            lastInvoiceNumber = sorted.last().invoiceNumber,
            totalTransactionCount = sorted.size,
            grossSalesAmount = sorted.sumOf { it.totalAmount },
            totalTaxableAmount = sorted.sumOf { it.totalTaxableAmount },
            totalTaxAmount = sorted.sumOf { it.totalTaxAmount },
            taxableAmountA = sorted.sumOf { it.taxableAmountA },
            taxAmountA = sorted.sumOf { it.taxAmountA },
            taxableAmountB = sorted.sumOf { it.taxableAmountB },
            taxableAmountC = sorted.sumOf { it.taxableAmountC },
            taxableAmountD = sorted.sumOf { it.taxableAmountD },
            taxableAmountE = sorted.sumOf { it.taxableAmountE },
            taxAmountE = sorted.sumOf { it.taxAmountE },
            cashAmount = sorted.filter { it.paymentTypeCode == "01" }.sumOf { it.totalAmount },
            cardAmount = sorted.filter { it.paymentTypeCode == "02" }.sumOf { it.totalAmount },
            mobileAmount = sorted.filter { it.paymentTypeCode == "03" }.sumOf { it.totalAmount }
        )
    }

    override suspend fun closeShiftAndGenerateZReport(
        operatorId: String,
        operatorName: String
    ): ZReportEntity {
        val stillPending = transactionDao.getPendingTransactionsList()
        if (stillPending.isNotEmpty()) {
            throw IllegalStateException(
                "Cannot close the shift: ${stillPending.size} transaction(s) have not yet " +
                    "synced to eTIMS. Closing now would exclude them from this Z-report. " +
                    "Ensure connectivity and retry sync before closing."
            )
        }

        val summary = getLiveShiftSummary()
        val floor = deviceRepository.registration.value?.lastZReportNumber ?: 0L

        return zReportDao.insertWithNextZReportNumber(floorZReportNumber = floor) { zReportNumber ->
            ZReportEntity(
                zReportNumber = zReportNumber,
                reportDate = summary.reportDate,
                closeoutTimestamp = System.currentTimeMillis(),
                operatorId = operatorId,
                operatorName = operatorName,
                firstInvoiceNumber = summary.firstInvoiceNumber,
                lastInvoiceNumber = summary.lastInvoiceNumber,
                totalTransactionCount = summary.totalTransactionCount,
                grossSalesAmount = summary.grossSalesAmount,
                taxableAmountA = summary.taxableAmountA,
                taxAmountA = summary.taxAmountA,
                taxableAmountB = summary.taxableAmountB,
                taxableAmountC = summary.taxableAmountC,
                taxableAmountD = summary.taxableAmountD,
                taxableAmountE = summary.taxableAmountE,
                taxAmountE = summary.taxAmountE,
                totalTaxableAmount = summary.totalTaxableAmount,
                totalTaxAmount = summary.totalTaxAmount,
                cashAmount = summary.cashAmount,
                cardAmount = summary.cardAmount,
                mobileAmount = summary.mobileAmount,
                // Honest default: there is no `saveReportZ`-equivalent eTIMS submission wired
                // up yet (the KRA Z-report contract needs confirming before building against
                // it), so this must not claim SYNCED for a report that was never actually filed.
                syncStatus = "PENDING"
            )
        }
    }

    override suspend fun getZReport(zReportNumber: Long): ZReportEntity? {
        return zReportDao.getZReportByNumber(zReportNumber)
    }
}
