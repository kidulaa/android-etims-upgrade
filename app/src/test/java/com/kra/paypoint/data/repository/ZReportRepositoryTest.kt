package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.TransactionDao
import com.kra.paypoint.data.local.dao.ZReportDao
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.ZReportEntity
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Before
import org.junit.Test
import org.mockito.ArgumentCaptor
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify

class ZReportRepositoryTest {

    private lateinit var transactionDao: TransactionDao
    private lateinit var zReportDao: ZReportDao
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
        zReportDao = mock(ZReportDao::class.java)

        `when`(transactionDao.getAllTransactions()).thenReturn(flowOf(sampleTransactions))
        `when`(zReportDao.getLastZReport()).thenReturn(null)
        `when`(zReportDao.getNextZReportNumber()).thenReturn(1L)

        repository = ZReportRepositoryImpl(transactionDao, zReportDao)
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
        val captor = ArgumentCaptor.forClass(ZReportEntity::class.java)
        `when`(zReportDao.insertZReport(captor.capture())).thenReturn(1L)

        val report = repository.closeShiftAndGenerateZReport("CASHIER01", "Jane Doe")

        verify(zReportDao).insertZReport(any())
        assertEquals(1L, report.zReportNumber)
        assertEquals("CASHIER01", report.operatorId)
        assertEquals(332.0, report.grossSalesAmount, 0.001)
        assertEquals(1001L, report.firstInvoiceNumber)
        assertEquals(1002L, report.lastInvoiceNumber)
    }
}
