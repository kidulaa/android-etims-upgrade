package com.kra.paypoint.ui.screens.creditnote.viewmodel

import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.hardware.printer.PrinterService
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.runBlocking
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.advanceUntilIdle
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify
import org.mockito.kotlin.any
import org.mockito.kotlin.argThat
import org.mockito.kotlin.eq

@OptIn(ExperimentalCoroutinesApi::class)
class CreditNoteViewModelTest {

    private val testDispatcher = StandardTestDispatcher()

    private lateinit var transactionRepository: TransactionRepository
    private lateinit var authRepository: AuthRepository
    private lateinit var printerService: PrinterService
    private lateinit var viewModel: CreditNoteViewModel

    private val testOperator = User(
        id = "OP-10",
        name = "Grace Hopper",
        authorityCode = "ROLE_CASHIER",
        branchId = "00",
        tin = "P012345678X"
    )

    private val testTx = TransactionEntity(
        id = 10L,
        invoiceNumber = 5001L,
        originalInvoiceNumber = 0L,
        customerTin = "P051234567Z",
        customerName = "Nakumatt Holdings",
        salesTypeCode = "N",
        receiptTypeCode = "R",
        paymentTypeCode = "01",
        totalAmount = 232.0,
        salesDate = "20260912140000",
        payloadJson = "{}"
    )

    private val testItems = listOf(
        TransactionItemEntity(
            id = 1L,
            transactionId = 10L,
            itemSequence = 1,
            itemCode = "ITM-01",
            itemClassificationCode = "5020",
            itemName = "Milk 500ml",
            quantity = 2.0,
            unitPrice = 116.0,
            supplyAmount = 200.0,
            taxTypeCode = "A",
            taxableAmount = 200.0,
            taxAmount = 32.0,
            totalAmount = 232.0
        )
    )

    private val testTxWithItems = TransactionWithItems(
        transaction = testTx,
        items = testItems
    )

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        transactionRepository = mock(TransactionRepository::class.java)
        authRepository = mock(AuthRepository::class.java)
        printerService = mock(PrinterService::class.java)

        runBlocking { `when`(authRepository.getCurrentUser()).thenReturn(testOperator) }

        viewModel = CreditNoteViewModel(
            transactionRepository = transactionRepository,
            authRepository = authRepository,
            printerService = printerService
        )
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `lookupInvoice finds transaction and populates refund items list`() = runTest {
        `when`(transactionRepository.getTransactionByInvoiceNumber(5001L)).thenReturn(testTxWithItems)

        viewModel.lookupInvoice(5001L)
        advanceUntilIdle()

        val state = viewModel.uiState.value
        assertNotNull(state.originalTransactionWithItems)
        assertEquals(1, state.refundItems.size)
        assertEquals("Milk 500ml", state.refundItems[0].originalItem.itemName)
        assertEquals(0.0, state.refundItems[0].refundQuantity, 0.001)
        assertEquals(false, state.refundItems[0].isSelected)
    }

    @Test
    fun `lookupInvoice rejects refund if original invoice is already a Credit Note`() = runTest {
        val creditNoteTx = testTx.copy(salesTypeCode = "C", receiptTypeCode = "R")
        val cnWithItems = TransactionWithItems(creditNoteTx, testItems)
        `when`(transactionRepository.getTransactionByInvoiceNumber(5002L)).thenReturn(cnWithItems)

        viewModel.lookupInvoice(5002L)
        advanceUntilIdle()

        val state = viewModel.uiState.value
        assertTrue(state.errorMessage?.contains("already a Credit Note") == true)
        assertTrue(state.refundItems.isEmpty())
    }

    @Test
    fun `selectAllForFullRefund selects all items with full quantity and computes totals`() = runTest {
        `when`(transactionRepository.getTransactionByInvoiceNumber(5001L)).thenReturn(testTxWithItems)

        viewModel.lookupInvoice(5001L)
        advanceUntilIdle()

        viewModel.selectAllForFullRefund()

        val state = viewModel.uiState.value
        assertEquals(2.0, state.totalRefundQuantity, 0.001)
        assertEquals(232.0, state.totalRefundAmount, 0.001)
        // Rate A (16% VAT): 232 / 1.16 = 200 taxable, 32 tax
        assertEquals(32.0, state.totalRefundTax, 0.01)
        assertEquals(200.0, state.totalRefundTaxable, 0.01)
    }

    @Test
    fun `updateRefundQuantity clamps to maximum purchased quantity`() = runTest {
        `when`(transactionRepository.getTransactionByInvoiceNumber(5001L)).thenReturn(testTxWithItems)

        viewModel.lookupInvoice(5001L)
        advanceUntilIdle()

        // Try setting 5.0 when only 2.0 were purchased
        viewModel.updateRefundQuantity(itemSequence = 1, quantity = 5.0)

        val state = viewModel.uiState.value
        assertEquals(2.0, state.refundItems[0].refundQuantity, 0.001)
        assertEquals(232.0, state.totalRefundAmount, 0.001)
    }

    @Test
    fun `issueCreditNote executes repository under the signed-in operator's identity and dispatches thermal print`() = runTest {
        `when`(transactionRepository.getTransactionByInvoiceNumber(5001L)).thenReturn(testTxWithItems)

        viewModel.lookupInvoice(5001L)
        advanceUntilIdle()

        viewModel.selectAllForFullRefund()

        `when`(
            transactionRepository.issueCreditNote(
                originalTransaction = eq(testTx),
                itemsToRefund = any(),
                reasonCode = eq("01"),
                reasonDescription = any(),
                restockInventory = eq(true),
                operatorId = eq("OP-10"),
                operatorName = eq("Grace Hopper")
            )
        ).thenReturn(Result.success(99L))

        val cnTx = testTx.copy(id = 99L, invoiceNumber = 5002L, originalInvoiceNumber = 5001L, salesTypeCode = "C", receiptTypeCode = "R")
        val cnWithItems = TransactionWithItems(cnTx, testItems)
        `when`(transactionRepository.getTransactionDetails(99L)).thenReturn(cnWithItems)

        viewModel.issueCreditNote()
        advanceUntilIdle()

        val state = viewModel.uiState.value
        assertEquals(5002L, state.successCreditNoteNumber)
        assertTrue(state.feedbackMessage?.contains("Credit Note #5002") == true)

        verify(printerService).printReceipt(
            argThat { contains("KRA FISCAL CREDIT NOTE") },
            eq(false)
        )
    }
}
