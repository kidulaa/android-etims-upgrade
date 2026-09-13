package com.kra.paypoint.data.repository

import android.content.Context
import com.google.gson.Gson
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import com.kra.paypoint.domain.exception.DeviceNotRegisteredException
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.InventoryRepository
import com.kra.paypoint.domain.repository.RefundItemParam
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.runBlocking
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock
import org.mockito.Mockito.never
import org.mockito.Mockito.verify

class TransactionRepositoryTest {

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
        // Unregistered by default: signing is best-effort (see TransactionRepositoryImpl.trySign),
        // so an unsigned PENDING transaction must still be recorded, never blocked.
        `when`(deviceRepository.registration).thenReturn(MutableStateFlow(null))
        runBlocking {
            `when`(deviceRepository.signReceipt(org.mockito.kotlin.any()))
                .thenReturn(Result.failure(DeviceNotRegisteredException()))
        }
        repository = TransactionRepositoryImpl(transactionDao, inventoryRepository, deviceRepository, gson, context)
    }

    private fun dummySaleReq(invcNoPlaceholder: Long = 0L) = TrnsSalesSaveReq(
        tin = "P012345678X",
        bhfId = "00",
        invcNo = invcNoPlaceholder,
        orgInvcNo = 0L,
        custTin = null,
        custNm = "Cash Customer",
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
        regrId = "ADMIN",
        regrNm = "Admin",
        modrId = "ADMIN",
        modrNm = "Admin",
        itemList = emptyList()
    )

    @Test
    fun `processSale assigns the next sequential invoice number and stores it in the payload`() = runTest {
        val items = listOf(
            TransactionItemEntity(
                transactionId = 0L,
                itemSequence = 1,
                itemCode = "ITM001",
                itemClassificationCode = "101",
                itemName = "Soda 300ml",
                quantity = 1.0,
                unitPrice = 116.0,
                supplyAmount = 100.0,
                taxTypeCode = "A",
                taxableAmount = 100.0,
                taxAmount = 16.0,
                totalAmount = 116.0
            )
        )

        val (transactionId, invoiceNumber) = repository.processSale(dummySaleReq(), items)

        assertEquals(1L, invoiceNumber) // first sale on an empty ledger

        val stored = transactionDao.allInserted.single { it.id == transactionId }
        assertEquals("PENDING", stored.syncStatus)
        assertEquals(1L, stored.invoiceNumber)
        assertEquals(116.0, stored.totalAmount, 0.001)

        val restoredReq = gson.fromJson(stored.payloadJson, TrnsSalesSaveReq::class.java)
        assertEquals("P012345678X", restoredReq.tin)
        assertEquals(1L, restoredReq.invcNo) // payload's invcNo matches the assigned number, not the 0L placeholder
    }

    @Test
    fun `two sales in a row never collide on invoice number`() = runTest {
        val (_, first) = repository.processSale(dummySaleReq(), emptyList())
        val (_, second) = repository.processSale(dummySaleReq(), emptyList())

        assertEquals(1L, first)
        assertEquals(2L, second)
    }

    @Test
    fun `issueCreditNote creates KRA-compliant credit note referencing orgInvcNo and calculates taxes`() = runTest {
        val origTx = TransactionEntity(
            id = 500L,
            invoiceNumber = 2000L,
            originalInvoiceNumber = 0L,
            customerTin = "P051234567Z",
            customerName = "Supermarket Ltd",
            salesTypeCode = "N",
            receiptTypeCode = "R",
            paymentTypeCode = "01",
            totalAmount = 232.0,
            salesDate = "20260912100000",
            payloadJson = "{}"
        )
        transactionDao.insertTransaction(origTx)

        val refundItems = listOf(
            RefundItemParam(
                itemCode = "ITM001",
                itemName = "Soda 300ml",
                quantity = 1.0,
                unitPrice = 116.0,
                taxRateClassificationCode = "A",
                totalAmount = 116.0
            )
        )

        val result = repository.issueCreditNote(
            originalTransaction = origTx,
            itemsToRefund = refundItems,
            reasonCode = "01",
            reasonDescription = "Damaged Goods",
            restockInventory = true,
            operatorId = "OP1",
            operatorName = "Jane Cashier"
        )

        assertTrue(result.isSuccess)
        val creditNoteId = result.getOrThrow()
        val capturedTx = transactionDao.allInserted.single { it.id == creditNoteId }

        assertEquals(2001L, capturedTx.invoiceNumber) // next number after the seeded 2000
        assertEquals(2000L, capturedTx.originalInvoiceNumber)
        assertEquals("C", capturedTx.salesTypeCode)
        assertEquals("R", capturedTx.receiptTypeCode)
        assertEquals("PENDING", capturedTx.syncStatus)
        assertEquals(116.0, capturedTx.totalAmount, 0.001)

        // Rate A 16% VAT math, via the shared TaxCalculationEngine
        assertEquals(100.0, capturedTx.taxableAmountA, 0.01)
        assertEquals(16.0, capturedTx.taxAmountA, 0.01)

        val parsedReq = gson.fromJson(capturedTx.payloadJson, TrnsSalesSaveReq::class.java)
        assertEquals("C", parsedReq.salesTyCd)
        assertEquals("R", parsedReq.rcptTyCd)
        assertEquals(2001L, parsedReq.invcNo)
        assertEquals(2000L, parsedReq.orgInvcNo)
        assertEquals("01", parsedReq.rfdRsnCd)
        assertEquals("Jane Cashier", parsedReq.regrNm)

        verify(inventoryRepository).recordStockIn(
            itemCode = "ITM001",
            quantity = 1.0,
            reasonCode = "01",
            reasonDescription = "Refund Return (Inv #2000)",
            remark = "Credit Note #2001 - Damaged Goods",
            operatorId = "OP1"
        )
    }

    @Test
    fun `issueCreditNote skips inventory restocking when restockInventory is false`() = runTest {
        val origTx = TransactionEntity(
            id = 600L,
            invoiceNumber = 3000L,
            totalAmount = 50.0,
            salesDate = "20260912100000",
            payloadJson = "{}"
        )
        transactionDao.insertTransaction(origTx)

        val refundItems = listOf(
            RefundItemParam(
                itemCode = "SVC001",
                itemName = "Service Fee",
                quantity = 1.0,
                unitPrice = 50.0,
                taxRateClassificationCode = "C", // Exempt
                totalAmount = 50.0
            )
        )

        val result = repository.issueCreditNote(
            originalTransaction = origTx,
            itemsToRefund = refundItems,
            reasonCode = "02",
            reasonDescription = "Pricing Error",
            restockInventory = false
        )

        assertTrue(result.isSuccess)
        verify(inventoryRepository, never()).recordStockIn(
            itemCode = "SVC001",
            quantity = 1.0,
            reasonCode = "01",
            reasonDescription = "Refund Return (Inv #3000)",
            remark = "Credit Note #3001 - Pricing Error",
            operatorId = "SYSTEM"
        )
    }
}
