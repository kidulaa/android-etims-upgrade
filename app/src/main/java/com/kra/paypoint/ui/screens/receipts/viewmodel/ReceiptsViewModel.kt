package com.kra.paypoint.ui.screens.receipts.viewmodel

import android.content.Context
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.hardware.printer.PrinterService
import com.kra.paypoint.ui.export.PdfGenerator
import dagger.hilt.android.lifecycle.HiltViewModel
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import java.io.File
import javax.inject.Inject

enum class SyncFilter {
    ALL,
    PENDING,
    SYNCED
}

data class ReceiptsUiState(
    val filter: SyncFilter = SyncFilter.ALL,
    val selectedTransactionWithItems: TransactionWithItems? = null,
    val isPrinting: Boolean = false,
    val printMessage: String? = null,
    val generatedPdf: File? = null
)

@HiltViewModel
class ReceiptsViewModel @Inject constructor(
    private val transactionRepository: TransactionRepository,
    private val deviceRepository: DeviceRepository,
    private val printerService: PrinterService,
    @ApplicationContext private val context: Context
) : ViewModel() {

    private val _uiState = MutableStateFlow(ReceiptsUiState())
    val uiState: StateFlow<ReceiptsUiState> = _uiState.asStateFlow()

    private val allTransactions: StateFlow<List<TransactionEntity>> =
        transactionRepository.getAllTransactions()
            .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    val filteredTransactions: StateFlow<List<TransactionEntity>> =
        combine(allTransactions, _uiState) { list, state ->
            when (state.filter) {
                SyncFilter.ALL -> list
                SyncFilter.PENDING -> list.filter { it.syncStatus == "PENDING" || it.syncStatus == "FAILED" }
                SyncFilter.SYNCED -> list.filter { it.syncStatus == "SYNCED" }
            }
        }.stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    fun setFilter(filter: SyncFilter) {
        _uiState.update { it.copy(filter = filter) }
    }

    fun selectTransaction(id: Long) {
        viewModelScope.launch {
            val details = transactionRepository.getTransactionDetails(id)
            _uiState.update { it.copy(selectedTransactionWithItems = details) }
        }
    }

    fun clearSelectedTransaction() {
        _uiState.update { it.copy(selectedTransactionWithItems = null, printMessage = null, generatedPdf = null) }
    }

    fun generateInvoicePdf(data: TransactionWithItems) {
        viewModelScope.launch {
            _uiState.update { it.copy(isPrinting = true, printMessage = "Generating PDF...") }
            try {
                val registration = deviceRepository.registration.value
                val pdfFile = PdfGenerator.createInvoicePdf(
                    context = context,
                    transaction = data.transaction,
                    items = data.items,
                    registration = registration
                )
                _uiState.update {
                    it.copy(
                        isPrinting = false,
                        printMessage = null,
                        generatedPdf = pdfFile
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isPrinting = false,
                        printMessage = "PDF error: ${e.localizedMessage}"
                    )
                }
            }
        }
    }

    fun clearGeneratedPdf() {
        _uiState.update { it.copy(generatedPdf = null) }
    }

    fun reprintReceipt(data: TransactionWithItems) {
        viewModelScope.launch {
            _uiState.update { it.copy(isPrinting = true, printMessage = null) }
            try {
                val receiptText = buildFiscalReceiptJournal(data)
                printerService.printReceipt(receiptText, isReprint = true)
                _uiState.update {
                    it.copy(
                        isPrinting = false,
                        printMessage = "Receipt #${data.transaction.invoiceNumber} sent to printer (REPRINT)."
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isPrinting = false,
                        printMessage = "Print error: ${e.localizedMessage}"
                    )
                }
            }
        }
    }

    private fun buildFiscalReceiptJournal(data: TransactionWithItems): String {
        val t = data.transaction
        val sb = StringBuilder()
        val isCreditNote = t.salesTypeCode == "C" || t.receiptTypeCode == "R" || t.originalInvoiceNumber > 0
        if (isCreditNote) {
            sb.appendLine("================================")
            sb.appendLine("     KRA FISCAL CREDIT NOTE     ")
            sb.appendLine("             (COPY)             ")
            sb.appendLine("================================")
            sb.appendLine("CREDIT NOTE: #${t.invoiceNumber}")
            sb.appendLine("ORIG INVC  : #${t.originalInvoiceNumber}")
        } else {
            sb.appendLine("================================")
            sb.appendLine("       eTIMS FISCAL RECEIPT     ")
            sb.appendLine("             (COPY)             ")
            sb.appendLine("================================")
            sb.appendLine("INVOICE NO : #${t.invoiceNumber}")
        }
        sb.appendLine("DATE/TIME  : ${t.salesDate}")
        sb.appendLine("CUSTOMER   : ${t.customerName ?: "Walk-in"}")
        if (!t.customerTin.isNullOrBlank()) {
            sb.appendLine("CUST PIN   : ${t.customerTin}")
        }
        sb.appendLine("--------------------------------")
        data.items.forEach { item ->
            sb.appendLine("${item.itemName} x ${item.quantity}")
            sb.appendLine(String.format("  @ KES %.2f = KES %.2f [%s]", item.unitPrice, item.totalAmount, item.taxTypeCode))
        }
        sb.appendLine("--------------------------------")
        sb.appendLine(String.format("TAXABLE AMOUNT : KES %.2f", t.totalTaxableAmount))
        sb.appendLine(String.format("TOTAL VAT      : KES %.2f", t.totalTaxAmount))
        sb.appendLine(String.format("TOTAL PAID     : KES %.2f", t.totalAmount))
        sb.appendLine("--------------------------------")
        sb.appendLine("STATUS : ${t.syncStatus}")
        sb.appendLine("================================")
        return sb.toString()
    }
}
