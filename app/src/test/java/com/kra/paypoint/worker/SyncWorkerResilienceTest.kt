package com.kra.paypoint.worker

import com.google.gson.Gson
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.remote.api.TransactionService
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesRes
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import com.kra.paypoint.data.repository.FakeTransactionDao
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.any
import org.mockito.Mockito.mock
import org.mockito.kotlin.whenever
import java.io.IOException

/**
 * Exercises the same result-code validation [SyncWorker.doWork] applies, against a fake DAO
 * (see [FakeTransactionDao]) standing in for Room. [SyncWorker] itself is resolved through a
 * Hilt EntryPoint at runtime, which needs an instrumented/Hilt test environment to construct
 * directly — that instrumented coverage is still open (see the migration report), so this
 * stays a focused unit test of the validation semantics rather than of the Worker class itself.
 */
class SyncWorkerResilienceTest {

    private lateinit var transactionDao: FakeTransactionDao
    private lateinit var transactionService: TransactionService
    private val gson = Gson()

    private val sampleReq = TrnsSalesSaveReq(
        tin = "P051111111A",
        bhfId = "00",
        invcNo = 9001L,
        orgInvcNo = 0L,
        custTin = null,
        custNm = "Retail Customer",
        salesTyCd = "N",
        rcptTyCd = "R",
        pmtTyCd = "01",
        rfdRsnCd = null,
        salesSttsCd = "02",
        cfmDt = "20260912120000",
        salesDt = "20260912",
        stockRlsDt = "20260912120000",
        cnclReqDt = null,
        cnclDt = null,
        rfdDt = null,
        totItemCnt = 1.0,
        taxblAmtA = 100.0,
        taxblAmtB = 0.0,
        taxblAmtC = 0.0,
        taxblAmtD = 0.0,
        taxblAmtE = 0.0,
        taxRtA = 16,
        taxRtB = 0,
        taxRtC = 0,
        taxRtD = 0,
        taxRtE = 8,
        taxAmtA = 16.0,
        taxAmtB = 0.0,
        taxAmtC = 0.0,
        taxAmtD = 0.0,
        taxAmtE = 0.0,
        totTaxblAmt = 100.0,
        totTaxAmt = 16.0,
        totAmt = 116.0,
        prchrAcptcYn = "N",
        remark = null,
        regrId = "OPERATOR",
        regrNm = "Operator",
        modrId = "OPERATOR",
        modrNm = "Operator",
        itemList = emptyList()
    )

    @Before
    fun setup() {
        transactionDao = FakeTransactionDao()
        transactionService = mock(TransactionService::class.java)
    }

    private suspend fun runSyncPass() {
        val pending = transactionDao.getPendingTransactionsList()
        for (item in pending) {
            val attempt = item.syncAttempts + 1
            try {
                val payload = gson.fromJson(item.payloadJson, TrnsSalesSaveReq::class.java)
                val response = transactionService.saveSalesTransaction(payload)
                if (response.resultCd == "000") {
                    transactionDao.updateSyncStatus(item.id, "SYNCED", attempt, null, System.currentTimeMillis())
                } else {
                    transactionDao.updateSyncStatus(
                        item.id, "FAILED", attempt,
                        "Rejected by eTIMS: ${response.resultCd} ${response.resultMsg.orEmpty()}", null
                    )
                }
            } catch (e: Exception) {
                transactionDao.updateSyncStatus(item.id, "FAILED", attempt, e.localizedMessage, null)
            }
        }
    }

    @Test
    fun `resultCd 000 marks status SYNCED`() = runTest {
        val pendingEntity = TransactionEntity(
            id = 42L, invoiceNumber = 9001L, customerName = "Retail Customer",
            totalAmount = 116.0, salesDate = "20260912120000",
            syncStatus = "PENDING", syncAttempts = 0, payloadJson = gson.toJson(sampleReq)
        )
        transactionDao.insertTransaction(pendingEntity)
        whenever(transactionService.saveSalesTransaction(any())).thenReturn(
            TrnsSalesRes(resultCd = "000", resultMsg = "Success", resultDt = "20260912120000", data = null)
        )

        runSyncPass()

        val updated = transactionDao.allInserted.single { it.id == 42L }
        assertEquals("SYNCED", updated.syncStatus)
        assertEquals(1, updated.syncAttempts)
    }

    @Test
    fun `a business rejection resultCd stays FAILED, not SYNCED`() = runTest {
        // This is the exact bug the old TransactionService (typed 'Any') masked: KRA
        // rejected the invoice, HTTP still returned 200, and the old code marked it SYNCED.
        val pendingEntity = TransactionEntity(
            id = 43L, invoiceNumber = 9002L, customerName = "Retail Customer",
            totalAmount = 116.0, salesDate = "20260912120000",
            syncStatus = "PENDING", syncAttempts = 0, payloadJson = gson.toJson(sampleReq)
        )
        transactionDao.insertTransaction(pendingEntity)
        whenever(transactionService.saveSalesTransaction(any())).thenReturn(
            TrnsSalesRes(resultCd = "910", resultMsg = "Invalid item code", resultDt = null, data = null)
        )

        runSyncPass()

        val updated = transactionDao.allInserted.single { it.id == 43L }
        assertEquals("FAILED", updated.syncStatus)
        assertEquals(1, updated.syncAttempts)
    }

    @Test
    fun `network error marks status FAILED and increments attempts for retry`() = runTest {
        val pendingEntity = TransactionEntity(
            id = 44L, invoiceNumber = 9003L, customerName = "Retail Customer",
            totalAmount = 116.0, salesDate = "20260912120000",
            syncStatus = "PENDING", syncAttempts = 1, payloadJson = gson.toJson(sampleReq)
        )
        transactionDao.insertTransaction(pendingEntity)
        `when`(transactionService.saveSalesTransaction(any())).thenThrow(IOException("Unable to resolve host"))

        runSyncPass()

        val updated = transactionDao.allInserted.single { it.id == 44L }
        assertEquals("FAILED", updated.syncStatus)
        assertEquals(2, updated.syncAttempts)
    }
}
