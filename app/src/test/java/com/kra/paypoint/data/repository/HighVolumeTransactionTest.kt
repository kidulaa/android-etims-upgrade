package com.kra.paypoint.data.repository

import android.content.Context
import com.google.gson.Gson
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import com.kra.paypoint.domain.exception.DeviceNotRegisteredException
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.InventoryRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.runBlocking
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock

class HighVolumeTransactionTest {

    private lateinit var transactionDao: FakeTransactionDao
    private lateinit var inventoryRepository: InventoryRepository
    private lateinit var deviceRepository: DeviceRepository
    private lateinit var context: Context
    private val gson = Gson()
    private lateinit var repository: TransactionRepositoryImpl

    @Before
    fun setup() {
        transactionDao = FakeTransactionDao()
        inventoryRepository = mock(InventoryRepository::class.java)
        deviceRepository = mock(DeviceRepository::class.java)
        context = mock(Context::class.java)
        `when`(context.applicationContext).thenReturn(context)
        `when`(deviceRepository.registration).thenReturn(MutableStateFlow(null))
        runBlocking {
            `when`(deviceRepository.signReceipt(org.mockito.kotlin.any()))
                .thenReturn(Result.failure(DeviceNotRegisteredException()))
        }
        repository = TransactionRepositoryImpl(transactionDao, inventoryRepository, deviceRepository, gson, context)
    }

    @Test
    fun `processSale handles rapid sequential transactions with distinct monotonic invoice sequences`() = runTest {
        val transactionCount = 50

        for (i in 1..transactionCount) {
            val req = TrnsSalesSaveReq(
                tin = "P010000000X",
                bhfId = "00",
                invcNo = 0L, // finalized by the repository, not the caller
                orgInvcNo = 0L,
                custTin = null,
                custNm = "Customer #$i",
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
                taxblAmtA = 100.0 * i,
                taxblAmtB = 0.0,
                taxblAmtC = 0.0,
                taxblAmtD = 0.0,
                taxblAmtE = 0.0,
                taxRtA = 16,
                taxRtB = 0,
                taxRtC = 0,
                taxRtD = 0,
                taxRtE = 8,
                taxAmtA = 16.0 * i,
                taxAmtB = 0.0,
                taxAmtC = 0.0,
                taxAmtD = 0.0,
                taxAmtE = 0.0,
                totTaxblAmt = 100.0 * i,
                totTaxAmt = 16.0 * i,
                totAmt = 116.0 * i,
                prchrAcptcYn = "N",
                remark = null,
                regrId = "OPERATOR",
                regrNm = "Operator",
                modrId = "OPERATOR",
                modrNm = "Operator",
                itemList = emptyList()
            )

            repository.processSale(req, emptyList())
        }

        val capturedEntities = transactionDao.allInserted
        assertEquals(transactionCount, capturedEntities.size)

        // The defect this regression-tests: the old implementation numbered invoices from
        // System.currentTimeMillis() and used OnConflictStrategy.REPLACE, so a collision
        // would silently delete an existing fiscal record instead of throwing.
        val invoiceNumbers = capturedEntities.map { it.invoiceNumber }
        assertEquals(transactionCount, invoiceNumbers.toSet().size)
        assertEquals((1L..transactionCount.toLong()).toList(), invoiceNumbers.sorted())
        assertTrue(invoiceNumbers.zipWithNext().all { (a, b) -> a < b })

        assertTrue(capturedEntities.all { it.syncStatus == "PENDING" })
    }
}
