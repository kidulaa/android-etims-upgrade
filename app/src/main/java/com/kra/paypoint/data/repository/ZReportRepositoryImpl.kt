package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.TransactionDao
import com.kra.paypoint.data.local.dao.ZReportDao
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.ZReportEntity
import com.kra.paypoint.data.local.entity.isCreditNote
import com.kra.paypoint.domain.model.EtimsCodes.PaymentType
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
        val creditNotes = sorted.filter { it.isCreditNote }

        // Credit notes are stored with positive amounts; they reverse sales, so they net off.
        fun net(amount: (TransactionEntity) -> Double): Double =
            sorted.sumOf { if (it.isCreditNote) -amount(it) else amount(it) }

        fun netPaidBy(vararg codes: String): Double =
            sorted.filter { it.paymentTypeCode in codes }.sumOf { if (it.isCreditNote) -it.totalAmount else it.totalAmount }

        return XReportSummary(
            reportDate = todayStr,
            readingTimestamp = System.currentTimeMillis(),
            firstInvoiceNumber = sorted.first().invoiceNumber,
            lastInvoiceNumber = sorted.last().invoiceNumber,
            totalTransactionCount = sorted.size,
            grossSalesAmount = net { it.totalAmount },
            totalTaxableAmount = net { it.totalTaxableAmount },
            totalTaxAmount = net { it.totalTaxAmount },
            taxableAmountA = net { it.taxableAmountA },
            taxAmountA = net { it.taxAmountA },
            taxableAmountB = net { it.taxableAmountB },
            taxAmountB = net { it.taxAmountB },
            taxableAmountC = net { it.taxableAmountC },
            taxableAmountD = net { it.taxableAmountD },
            taxableAmountE = net { it.taxableAmountE },
            taxAmountE = net { it.taxAmountE },
            creditNoteCount = creditNotes.size,
            creditNoteAmount = creditNotes.sumOf { it.totalAmount },
            // "03" was this app's pre-KRA-code mobile value; the till never offers Cash/Credit, so keep counting it as mobile.
            cashAmount = netPaidBy(PaymentType.CASH),
            cardAmount = netPaidBy(PaymentType.CARD),
            mobileAmount = netPaidBy(PaymentType.MOBILE_MONEY, PaymentType.CASH_CREDIT),
            creditAmount = netPaidBy(PaymentType.CREDIT)
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
                taxAmountB = summary.taxAmountB,
                taxableAmountC = summary.taxableAmountC,
                taxableAmountD = summary.taxableAmountD,
                taxableAmountE = summary.taxableAmountE,
                taxAmountE = summary.taxAmountE,
                totalTaxableAmount = summary.totalTaxableAmount,
                totalTaxAmount = summary.totalTaxAmount,
                cashAmount = summary.cashAmount,
                cardAmount = summary.cardAmount,
                mobileAmount = summary.mobileAmount,
                creditAmount = summary.creditAmount,
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
