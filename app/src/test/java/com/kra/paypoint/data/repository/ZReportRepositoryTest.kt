package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.TransactionDao
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.domain.repository.DeviceRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.runBlocking
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock

class ZReportRepositoryTest {

    private lateinit var transactionDao: TransactionDao
    private lateinit var zReportDao: FakeZReportDao
    private lateinit var deviceRepository: DeviceRepository
    private lateinit var repository: ZReportRepositoryImpl

    private val sampleTransactions = listOf(
        TransactionEntity(
            id = 1L,
            invoiceNumber = 1001L,
            totalAmount = 116.0,
            totalTaxableAmount = 100.0,
            totalTaxAmount = 16.0,
            taxableAmountA = 100.0,
            taxAmountA = 16.0,
            paymentTypeCode = "01", // Cash
            salesDate = "20260912100000",
            syncStatus = "SYNCED",
            payloadJson = "{}",
            createdAt = 1000L
        ),
        TransactionEntity(
            id = 2L,
            invoiceNumber = 1002L,
            totalAmount = 216.0,
            totalTaxableAmount = 200.0,
            totalTaxAmount = 16.0,
            taxableAmountE = 200.0,
            taxAmountE = 16.0,
            paymentTypeCode = "03", // Mobile Money
            salesDate = "20260912110000",
            syncStatus = "SYNCED",
            payloadJson = "{}",
            createdAt = 2000L
        )
    )

    @Before
    fun setup() {
        transactionDao = mock(TransactionDao::class.java)
        zReportDao = FakeZReportDao()
        deviceRepository = mock(DeviceRepository::class.java)

        `when`(transactionDao.getAllTransactions()).thenReturn(flowOf(sampleTransactions))
        `when`(deviceRepository.registration).thenReturn(MutableStateFlow(null))
        runBlocking {
            // No unsynced transactions by default, so closeShiftAndGenerateZReport's
            // pending-transaction guard doesn't block these tests.
            `when`(transactionDao.getPendingTransactionsList()).thenReturn(emptyList())
        }

        repository = ZReportRepositoryImpl(transactionDao, zReportDao, deviceRepository)
    }

    @Test
    fun `getLiveShiftSummary correctly aggregates sales, tax breakdown, and payment methods`() = runTest {
        val summary = repository.getLiveShiftSummary()

        assertEquals(2, summary.totalTransactionCount)
        assertEquals(332.0, summary.grossSalesAmount, 0.001)
        assertEquals(300.0, summary.totalTaxableAmount, 0.001)
        assertEquals(32.0, summary.totalTaxAmount, 0.001)

        // Tax buckets
        assertEquals(100.0, summary.taxableAmountA, 0.001)
        assertEquals(16.0, summary.taxAmountA, 0.001)
        assertEquals(200.0, summary.taxableAmountE, 0.001)
        assertEquals(16.0, summary.taxAmountE, 0.001)

        // Payment buckets
        assertEquals(116.0, summary.cashAmount, 0.001)
        assertEquals(216.0, summary.mobileAmount, 0.001)
        assertEquals(0.0, summary.cardAmount, 0.001)
    }

    @Test
    fun `closeShiftAndGenerateZReport advances Z-number and persists record`() = runTest {
        val report = repository.closeShiftAndGenerateZReport("CASHIER01", "Jane Doe")

        assertEquals(1L, report.zReportNumber)
        assertEquals("CASHIER01", report.operatorId)
        assertEquals(332.0, report.grossSalesAmount, 0.001)
        assertEquals(1001L, report.firstInvoiceNumber)
        assertEquals(1002L, report.lastInvoiceNumber)
        assertEquals(1, zReportDao.getAllZReports().value.size)
    }

    @Test
    fun `closing the shift twice never reuses a Z-report number`() = runTest {
        val first = repository.closeShiftAndGenerateZReport("CASHIER01", "Jane Doe")
        val second = repository.closeShiftAndGenerateZReport("CASHIER01", "Jane Doe")

        assertEquals(1L, first.zReportNumber)
        assertEquals(2L, second.zReportNumber)
    }
}
