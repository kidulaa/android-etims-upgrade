package com.kra.paypoint.ui.screens.receipts.viewmodel

import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.hardware.printer.PrinterService
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.launch
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify
import org.mockito.kotlin.any
import org.mockito.kotlin.eq

@OptIn(ExperimentalCoroutinesApi::class)
class ReceiptsViewModelTest {

    private lateinit var transactionRepository: TransactionRepository
    private lateinit var printerService: PrinterService
    private lateinit var viewModel: ReceiptsViewModel
    private val testDispatcher = StandardTestDispatcher()

    private val sampleTransactions = listOf(
        TransactionEntity(
            id = 1L,
            invoiceNumber = 1001L,
            customerName = "Walk-in Customer",
            totalAmount = 50.0,
            salesDate = "20260912100000",
            syncStatus = "SYNCED",
            payloadJson = "{}"
        ),
        TransactionEntity(
            id = 2L,
            invoiceNumber = 1002L,
            customerName = "Acme Corp",
            totalAmount = 150.0,
            salesDate = "20260912110000",
            syncStatus = "PENDING",
            payloadJson = "{}"
        )
    )

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        transactionRepository = mock(TransactionRepository::class.java)
        printerService = mock(PrinterService::class.java)

        `when`(transactionRepository.getAllTransactions()).thenReturn(flowOf(sampleTransactions))

        viewModel = ReceiptsViewModel(transactionRepository, printerService)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `filteredTransactions filters by PENDING and SYNCED status`() = runTest {
        // stateIn(WhileSubscribed) only starts producing once collected.
        backgroundScope.launch { viewModel.filteredTransactions.collect {} }
        testScheduler.advanceUntilIdle()
        assertEquals(2, viewModel.filteredTransactions.value.size)

        viewModel.setFilter(SyncFilter.PENDING)
        testScheduler.advanceUntilIdle()
        assertEquals(1, viewModel.filteredTransactions.value.size)
        assertEquals(1002L, viewModel.filteredTransactions.value.first().invoiceNumber)

        viewModel.setFilter(SyncFilter.SYNCED)
        testScheduler.advanceUntilIdle()
        assertEquals(1, viewModel.filteredTransactions.value.size)
        assertEquals(1001L, viewModel.filteredTransactions.value.first().invoiceNumber)
    }

    @Test
    fun `selectTransaction loads details from repository`() = runTest {
        val details = TransactionWithItems(
            transaction = sampleTransactions.first(),
            items = listOf(
                TransactionItemEntity(
                    id = 1L,
                    transactionId = 1L,
                    itemSequence = 1,
                    itemCode = "ITM-01",
                    itemClassificationCode = "5020",
                    itemName = "Water",
                    quantity = 1.0,
                    unitPrice = 50.0,
                    supplyAmount = 50.0,
                    taxTypeCode = "A",
                    taxableAmount = 43.10,
                    taxAmount = 6.90,
                    totalAmount = 50.0
                )
            )
        )
        `when`(transactionRepository.getTransactionDetails(1L)).thenReturn(details)

        viewModel.selectTransaction(1L)
        testScheduler.advanceUntilIdle()

        assertNotNull(viewModel.uiState.value.selectedTransactionWithItems)
        assertEquals(1001L, viewModel.uiState.value.selectedTransactionWithItems?.transaction?.invoiceNumber)
    }

    @Test
    fun `reprintReceipt invokes printerService with reprint flag`() = runTest {
        val details = TransactionWithItems(
            transaction = sampleTransactions.first(),
            items = emptyList()
        )

        viewModel.reprintReceipt(details)
        testScheduler.advanceUntilIdle()

        verify(printerService).printReceipt(any(), eq(true))
        assertNotNull(viewModel.uiState.value.printMessage)
    }
}
