package com.kra.paypoint.ui.screens.closeout.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.ZReportEntity
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.XReportSummary
import com.kra.paypoint.domain.repository.ZReportRepository
import com.kra.paypoint.hardware.printer.PrinterService
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

data class FiscalCloseoutUiState(
    val shiftSummary: XReportSummary? = null,
    val isProcessing: Boolean = false,
    val feedbackMessage: String? = null
)

@HiltViewModel
class FiscalCloseoutViewModel @Inject constructor(
    private val zReportRepository: ZReportRepository,
    private val authRepository: AuthRepository,
    private val printerService: PrinterService
) : ViewModel() {

    private val _uiState = MutableStateFlow(FiscalCloseoutUiState())
    val uiState: StateFlow<FiscalCloseoutUiState> = _uiState.asStateFlow()

    val pastZReports: StateFlow<List<ZReportEntity>> =
        zReportRepository.getAllZReports()
            .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    init {
        refreshShiftSummary()
    }

    fun refreshShiftSummary() {
        viewModelScope.launch {
            _uiState.update { it.copy(isProcessing = true) }
            try {
                val summary = zReportRepository.getLiveShiftSummary()
                _uiState.update {
                    it.copy(shiftSummary = summary, isProcessing = false)
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isProcessing = false,
                        feedbackMessage = "Error loading shift reading: ${e.toUserMessage()}"
                    )
                }
            }
        }
    }

    fun printXReport() {
        val summary = _uiState.value.shiftSummary ?: return
        viewModelScope.launch {
            _uiState.update { it.copy(isProcessing = true, feedbackMessage = null) }
            try {
                val xReportJournal = formatXReportJournal(summary)
                printerService.printReceipt(xReportJournal, isReprint = false)
                _uiState.update {
                    it.copy(
                        isProcessing = false,
                        feedbackMessage = "X-Report reading printed."
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isProcessing = false,
                        feedbackMessage = "Failed to print X-Report: ${e.toUserMessage()}"
                    )
                }
            }
        }
    }

    fun closeShiftAndPrintZReport() {
        // Re-entry guard: a double tap can land before the disabled button recomposes.
        if (_uiState.value.isProcessing) return
        _uiState.update { it.copy(isProcessing = true, feedbackMessage = null) }
        viewModelScope.launch {
            try {
                val user = authRepository.currentUser.value
                val operatorId = user?.id ?: "OPERATOR"
                val operatorName = user?.name ?: "Cashier"

                val zReport = zReportRepository.closeShiftAndGenerateZReport(operatorId, operatorName)
                val zJournal = formatZReportJournal(zReport)
                printerService.printReceipt(zJournal, isReprint = false)

                // Refresh shift reading (which resets to 0)
                val newSummary = zReportRepository.getLiveShiftSummary()

                _uiState.update {
                    it.copy(
                        shiftSummary = newSummary,
                        isProcessing = false,
                        feedbackMessage = "Z-Report #${zReport.zReportNumber} closed and printed successfully."
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isProcessing = false,
                        feedbackMessage = "Closeout error: ${e.toUserMessage()}"
                    )
                }
            }
        }
    }

    fun reprintZReport(report: ZReportEntity) {
        viewModelScope.launch {
            _uiState.update { it.copy(isProcessing = true, feedbackMessage = null) }
            try {
                val zJournal = formatZReportJournal(report, isCopy = true)
                printerService.printReceipt(zJournal, isReprint = true)
                _uiState.update {
                    it.copy(
                        isProcessing = false,
                        feedbackMessage = "Z-Report #${report.zReportNumber} sent to printer (COPY)."
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isProcessing = false,
                        feedbackMessage = "Print error: ${e.toUserMessage()}"
                    )
                }
            }
        }
    }

    private fun formatXReportJournal(s: XReportSummary): String {
        return buildString {
            appendLine("================================")
            appendLine("      eTIMS FISCAL READING      ")
            appendLine("            X-REPORT            ")
            appendLine("================================")
            appendLine("DATE        : ${s.reportDate}")
            appendLine("TX COUNT    : ${s.totalTransactionCount}")
            appendLine("FIRST INVC  : #${s.firstInvoiceNumber}")
            appendLine("LAST INVC   : #${s.lastInvoiceNumber}")
            appendLine("--------------------------------")
            appendLine(String.format("TAXABLE B-16%%   : KES %.2f", s.taxableAmountB))
            appendLine(String.format("VAT B-16%%       : KES %.2f", s.taxAmountB))
            appendLine(String.format("TAXABLE E-8%%    : KES %.2f", s.taxableAmountE))
            appendLine(String.format("VAT E-8%%        : KES %.2f", s.taxAmountE))
            appendLine(String.format("A-EXEMPT        : KES %.2f", s.taxableAmountA))
            appendLine(String.format("C-ZERO RATED    : KES %.2f", s.taxableAmountC))
            appendLine(String.format("D-NON-VAT       : KES %.2f", s.taxableAmountD))
            appendLine("--------------------------------")
            appendLine(String.format("TOTAL TAXABLE   : KES %.2f", s.totalTaxableAmount))
            appendLine(String.format("TOTAL VAT       : KES %.2f", s.totalTaxAmount))
            appendLine(String.format("REFUNDS (%d)     : KES %.2f", s.creditNoteCount, s.creditNoteAmount))
            appendLine(String.format("NET SALES       : KES %.2f", s.grossSalesAmount))
            appendLine("--------------------------------")
            appendLine(String.format("CASH DRAWER     : KES %.2f", s.cashAmount))
            appendLine(String.format("MOBILE MONEY    : KES %.2f", s.mobileAmount))
            appendLine(String.format("CARD            : KES %.2f", s.cardAmount))
            appendLine(String.format("CREDIT (UNPAID) : KES %.2f", s.creditAmount))
            appendLine("================================")
            appendLine("     MID-SHIFT READING ONLY     ")
            appendLine("================================")
        }
    }

    private fun formatZReportJournal(z: ZReportEntity, isCopy: Boolean = false): String {
        return buildString {
            appendLine("================================")
            appendLine("   eTIMS DAILY FISCAL CLOSEOUT  ")
            appendLine(if (isCopy) "        Z-REPORT (COPY)         " else "            Z-REPORT            ")
            appendLine("================================")
            appendLine("Z-REPORT NO : #${z.zReportNumber}")
            appendLine("DATE        : ${z.reportDate}")
            appendLine("OPERATOR    : ${z.operatorName} (${z.operatorId})")
            appendLine("TX COUNT    : ${z.totalTransactionCount}")
            appendLine("INVOICE SEQ : #${z.firstInvoiceNumber} - #${z.lastInvoiceNumber}")
            appendLine("--------------------------------")
            appendLine(String.format("TAXABLE B-16%%   : KES %.2f", z.taxableAmountB))
            appendLine(String.format("VAT B-16%%       : KES %.2f", z.taxAmountB))
            appendLine(String.format("TAXABLE E-8%%    : KES %.2f", z.taxableAmountE))
            appendLine(String.format("VAT E-8%%        : KES %.2f", z.taxAmountE))
            appendLine(String.format("A-EXEMPT        : KES %.2f", z.taxableAmountA))
            appendLine(String.format("C-ZERO RATED    : KES %.2f", z.taxableAmountC))
            appendLine(String.format("D-NON-VAT       : KES %.2f", z.taxableAmountD))
            appendLine("--------------------------------")
            appendLine(String.format("TOTAL TAXABLE   : KES %.2f", z.totalTaxableAmount))
            appendLine(String.format("TOTAL VAT       : KES %.2f", z.totalTaxAmount))
            appendLine(String.format("NET SALES       : KES %.2f", z.grossSalesAmount))
            appendLine("--------------------------------")
            appendLine(String.format("TOTAL CASH      : KES %.2f", z.cashAmount))
            appendLine(String.format("TOTAL MOBILE    : KES %.2f", z.mobileAmount))
            appendLine(String.format("TOTAL CARD      : KES %.2f", z.cardAmount))
            appendLine(String.format("TOTAL CREDIT    : KES %.2f", z.creditAmount))
            appendLine("================================")
            appendLine("     FISCAL DAY CLOSED          ")
            appendLine("================================")
        }
    }
}
