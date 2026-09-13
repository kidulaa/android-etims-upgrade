package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.ZReportDao
import com.kra.paypoint.data.local.entity.ZReportEntity
import kotlinx.coroutines.flow.MutableStateFlow

/** In-memory [ZReportDao] — see FakeTransactionDao for why this beats mocking here: a mock
 *  skips the real `insertWithNextZReportNumber` default-method sequencing logic entirely. */
class FakeZReportDao : ZReportDao {
    private val reportsFlow = MutableStateFlow<List<ZReportEntity>>(emptyList())

    override suspend fun insertZReport(report: ZReportEntity): Long {
        if (reportsFlow.value.any { it.zReportNumber == report.zReportNumber }) {
            throw IllegalStateException("UNIQUE constraint failed: z_reports.zReportNumber")
        }
        reportsFlow.value = reportsFlow.value + report
        return report.id
    }

    override fun getAllZReports(): MutableStateFlow<List<ZReportEntity>> = reportsFlow

    override suspend fun getLastZReport(): ZReportEntity? =
        reportsFlow.value.maxByOrNull { it.zReportNumber }

    override suspend fun getZReportByNumber(number: Long): ZReportEntity? =
        reportsFlow.value.find { it.zReportNumber == number }

    override suspend fun getLastZReportNumber(): Long =
        reportsFlow.value.maxOfOrNull { it.zReportNumber } ?: 0L
}
