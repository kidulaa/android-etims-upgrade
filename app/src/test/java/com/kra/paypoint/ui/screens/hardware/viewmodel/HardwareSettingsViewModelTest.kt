package com.kra.paypoint.ui.screens.hardware.viewmodel

import android.content.Context
import android.content.SharedPreferences
import com.kra.paypoint.hardware.printer.PrinterService
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
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
import org.mockito.Mockito.anyBoolean
import org.mockito.Mockito.anyString
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify

@OptIn(ExperimentalCoroutinesApi::class)
class HardwareSettingsViewModelTest {

    private lateinit var printerService: PrinterService
    private lateinit var context: Context
    private lateinit var prefs: SharedPreferences
    private lateinit var editor: SharedPreferences.Editor
    private lateinit var viewModel: HardwareSettingsViewModel
    private val testDispatcher = StandardTestDispatcher()

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        printerService = mock(PrinterService::class.java)
        context = mock(Context::class.java)
        prefs = mock(SharedPreferences::class.java)
        editor = mock(SharedPreferences.Editor::class.java)

        `when`(context.getSharedPreferences("etims_hardware_prefs", Context.MODE_PRIVATE)).thenReturn(prefs)
        `when`(prefs.edit()).thenReturn(editor)
        `when`(editor.putString(anyString(), anyString())).thenReturn(editor)
        `when`(editor.remove(anyString())).thenReturn(editor)
        `when`(prefs.getString("selected_printer_address", null)).thenReturn(null)
        `when`(prefs.getString("printer_paper_width", "58mm")).thenReturn("58mm")

        `when`(printerService.getPairedPrinters()).thenReturn(listOf("POS-58 [00:11:22:33:44:55]"))

        viewModel = HardwareSettingsViewModel(printerService, context)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `refreshPrinters updates pairedPrinters list`() = runTest {
        testScheduler.advanceUntilIdle()
        assertEquals(1, viewModel.uiState.value.pairedPrinters.size)
        assertEquals("POS-58 [00:11:22:33:44:55]", viewModel.uiState.value.pairedPrinters.first())
    }

    @Test
    fun `selectPrinter connects and saves preference`() = runTest {
        val printerAddress = "POS-58 [00:11:22:33:44:55]"
        `when`(printerService.connectToPrinter(printerAddress)).thenReturn(true)

        viewModel.selectPrinter(printerAddress)
        testScheduler.advanceUntilIdle()

        verify(printerService).connectToPrinter(printerAddress)
        verify(editor).putString("selected_printer_address", printerAddress)
        assertEquals(printerAddress, viewModel.uiState.value.selectedPrinter)
    }

    @Test
    fun `setPaperWidth updates paper width state and preference`() {
        viewModel.setPaperWidth("80mm")
        verify(editor).putString("printer_paper_width", "80mm")
        assertEquals("80mm", viewModel.uiState.value.paperWidth)
    }

    @Test
    fun `runTestPrint triggers printReceipt on printerService`() = runTest {
        viewModel.runTestPrint()
        testScheduler.advanceUntilIdle()

        verify(printerService).printReceipt(anyString(), org.mockito.Mockito.eq(false))
        assertNotNull(viewModel.uiState.value.statusMessage)
    }
}
