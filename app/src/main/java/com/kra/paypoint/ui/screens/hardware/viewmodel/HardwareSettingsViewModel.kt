package com.kra.paypoint.ui.screens.hardware.viewmodel

import android.content.Context
import android.content.SharedPreferences
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.hardware.printer.PrinterService
import dagger.hilt.android.lifecycle.HiltViewModel
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

data class HardwareSettingsUiState(
    val pairedPrinters: List<String> = emptyList(),
    val selectedPrinter: String? = null,
    val paperWidth: String = "58mm", // "58mm" or "80mm"
    val isTestingPrint: Boolean = false,
    val isConnecting: Boolean = false,
    val statusMessage: String? = null,
    val isDeviceRegistered: Boolean = false,
    val isRegisteringDevice: Boolean = false
)

@HiltViewModel
class HardwareSettingsViewModel @Inject constructor(
    private val printerService: PrinterService,
    private val deviceRepository: DeviceRepository,
    private val authRepository: AuthRepository,
    @ApplicationContext private val context: Context
) : ViewModel() {

    private val prefs: SharedPreferences =
        context.getSharedPreferences("etims_hardware_prefs", Context.MODE_PRIVATE)

    private val _uiState = MutableStateFlow(
        HardwareSettingsUiState(
            selectedPrinter = prefs.getString("selected_printer_address", null),
            paperWidth = prefs.getString("printer_paper_width", "58mm") ?: "58mm"
        )
    )
    val uiState: StateFlow<HardwareSettingsUiState> = _uiState.asStateFlow()

    init {
        refreshPrinters()
        viewModelScope.launch {
            _uiState.update { it.copy(isDeviceRegistered = deviceRepository.isRegistered()) }
        }
    }

    fun retryDeviceRegistration() {
        val user = authRepository.currentUser.value
        if (user == null) {
            _uiState.update { it.copy(statusMessage = "Sign in again before retrying device registration.") }
            return
        }
        viewModelScope.launch {
            _uiState.update { it.copy(isRegisteringDevice = true, statusMessage = null) }
            val result = deviceRepository.registerDevice(user.tin, user.branchId)
            _uiState.update {
                it.copy(
                    isRegisteringDevice = false,
                    isDeviceRegistered = result.isSuccess,
                    statusMessage = result.fold(
                        onSuccess = { "Device eTIMS registration complete." },
                        onFailure = { e -> "Device registration failed: ${e.localizedMessage}" }
                    )
                )
            }
        }
    }

    fun refreshPrinters() {
        val printers = printerService.getPairedPrinters()
        _uiState.update { it.copy(pairedPrinters = printers) }
    }

    fun selectPrinter(printerAddress: String) {
        viewModelScope.launch {
            _uiState.update { it.copy(isConnecting = true, statusMessage = null) }
            val connected = printerService.connectToPrinter(printerAddress)
            if (connected) {
                prefs.edit().putString("selected_printer_address", printerAddress).apply()
                _uiState.update {
                    it.copy(
                        selectedPrinter = printerAddress,
                        isConnecting = false,
                        statusMessage = "Connected to $printerAddress"
                    )
                }
            } else {
                _uiState.update {
                    it.copy(
                        isConnecting = false,
                        statusMessage = "Failed to connect to printer."
                    )
                }
            }
        }
    }

    fun setPaperWidth(width: String) {
        prefs.edit().putString("printer_paper_width", width).apply()
        _uiState.update { it.copy(paperWidth = width) }
    }

    fun runTestPrint() {
        viewModelScope.launch {
            _uiState.update { it.copy(isTestingPrint = true, statusMessage = null) }
            try {
                delay(600)
                val testReceipt = buildString {
                    appendLine("================================")
                    appendLine("     eTIMS PAYPOINT POS         ")
                    appendLine("     HARDWARE TEST PRINT        ")
                    appendLine("================================")
                    appendLine("STATUS     : SUCCESSFUL         ")
                    appendLine("PAPER WIDTH: ${_uiState.value.paperWidth}       ")
                    appendLine("PRINTER    : ${_uiState.value.selectedPrinter ?: "Default"} ")
                    appendLine("TIME       : ${System.currentTimeMillis()}")
                    appendLine("================================")
                    appendLine("  READY FOR FISCAL OPERATIONS   ")
                    appendLine("================================")
                }

                printerService.printReceipt(testReceipt, isReprint = false)
                _uiState.update {
                    it.copy(
                        isTestingPrint = false,
                        statusMessage = "Test print job dispatched successfully."
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isTestingPrint = false,
                        statusMessage = "Test print error: ${e.localizedMessage}"
                    )
                }
            }
        }
    }

    fun disconnectPrinter() {
        viewModelScope.launch {
            printerService.disconnect()
            prefs.edit().remove("selected_printer_address").apply()
            _uiState.update {
                it.copy(
                    selectedPrinter = null,
                    statusMessage = "Printer disconnected."
                )
            }
        }
    }
}
