package com.kra.paypoint.ui.screens.closeout.viewmodel

import com.kra.paypoint.data.local.entity.ZReportEntity
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.XReportSummary
import com.kra.paypoint.domain.repository.ZReportRepository
import com.kra.paypoint.hardware.printer.PrinterService
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.advanceUntilIdle
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
class FiscalCloseoutViewModelTest {

    private lateinit var zReportRepository: ZReportRepository
    private lateinit var authRepository: AuthRepository
    private lateinit var printerService: PrinterService
    private lateinit var viewModel: FiscalCloseoutViewModel
    private val testDispatcher = StandardTestDispatcher()
    private val userFlow = MutableStateFlow<User?>(
        User(id = "CASHIER01", name = "Jane Doe", authorityCode = "ROLE_CASHIER", branchId = "00", tin = "P012345678X")
    )

    private val sampleXSummary = XReportSummary(
        reportDate = "20260912",
        totalTransactionCount = 5,
        grossSalesAmount = 1500.0,
        totalTaxableAmount = 1293.10,
        totalTaxAmount = 206.90
    )

    private val sampleZReport = ZReportEntity(
        id = 1L,
        zReportNumber = 1L,
        reportDate = "20260912",
        operatorId = "CASHIER01",
        operatorName = "Jane Doe",
        firstInvoiceNumber = 101L,
        lastInvoiceNumber = 105L,
        totalTransactionCount = 5,
        grossSalesAmount = 1500.0,
        totalTaxableAmount = 1293.10,
        totalTaxAmount = 206.90
    )

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        zReportRepository = mock(ZReportRepository::class.java)
        authRepository = mock(AuthRepository::class.java)
        printerService = mock(PrinterService::class.java)

        `when`(authRepository.currentUser).thenReturn(userFlow)
        `when`(zReportRepository.getAllZReports()).thenReturn(flowOf(listOf(sampleZReport)))

        viewModel = FiscalCloseoutViewModel(zReportRepository, authRepository, printerService)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `printXReport formats reading and dispatches to printerService`() = runTest {
        `when`(zReportRepository.getLiveShiftSummary()).thenReturn(sampleXSummary)
        viewModel.refreshShiftSummary()
        testScheduler.advanceUntilIdle()

        viewModel.printXReport()
        testScheduler.advanceUntilIdle()

        verify(printerService).printReceipt(any(), eq(false))
        assertNotNull(viewModel.uiState.value.feedbackMessage)
    }

    @Test
    fun `closeShiftAndPrintZReport generates Z-report and dispatches to printer`() = runTest {
        `when`(zReportRepository.closeShiftAndGenerateZReport("CASHIER01", "Jane Doe")).thenReturn(sampleZReport)
        `when`(zReportRepository.getLiveShiftSummary()).thenReturn(XReportSummary(reportDate = "20260912"))

        viewModel.closeShiftAndPrintZReport()
        testScheduler.advanceUntilIdle()

        verify(zReportRepository).closeShiftAndGenerateZReport("CASHIER01", "Jane Doe")
        verify(printerService).printReceipt(any(), eq(false))
        assertNotNull(viewModel.uiState.value.feedbackMessage)
    }
}
