package com.kra.paypoint.ui.screens.receipts.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import android.content.Context
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.hardware.printer.PrinterService
import com.kra.paypoint.ui.export.PdfGenerator
import com.kra.paypoint.ui.export.ReceiptHeader
import com.kra.paypoint.ui.export.ReceiptJournal
import dagger.hilt.android.lifecycle.HiltViewModel
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.File
import javax.inject.Inject

enum class SyncFilter {
    ALL,
    PENDING,
    SYNCED
}

data class ReceiptsUiState(
    val filter: SyncFilter = SyncFilter.ALL,
    val searchQuery: String = "",
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
            val byStatus = when (state.filter) {
                SyncFilter.ALL -> list
                SyncFilter.PENDING -> list.filter { it.syncStatus == "PENDING" || it.syncStatus == "FAILED" }
                SyncFilter.SYNCED -> list.filter { it.syncStatus == "SYNCED" }
            }
            val q = state.searchQuery.trim().removePrefix("#")
            if (q.isEmpty()) byStatus
            else byStatus.filter {
                it.invoiceNumber.toString() == q ||
                    it.customerName?.contains(q, ignoreCase = true) == true ||
                    it.customerTin?.contains(q, ignoreCase = true) == true
            }
        }.stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    fun setFilter(filter: SyncFilter) {
        _uiState.update { it.copy(filter = filter) }
    }

    /** "Search Receipt": matches an exact invoice number, or part of the buyer's name or PIN. */
    fun setSearchQuery(query: String) {
        _uiState.update { it.copy(searchQuery = query) }
    }

    fun selectTransaction(id: Long) {
        viewModelScope.launch {
            val details = transactionRepository.getTransactionDetails(id)
            _uiState.update { it.copy(selectedTransactionWithItems = details) }
        }
    }

    /** "Mark as paid" for a credit sale; refreshes the open receipt so the badge clears. */
    fun markPaid(transactionId: Long) {
        viewModelScope.launch {
            transactionRepository.markPaid(transactionId)
            val refreshed = transactionRepository.getTransactionDetails(transactionId)
            _uiState.update { it.copy(selectedTransactionWithItems = refreshed, printMessage = "Marked as paid.") }
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
                // iText layout + QR rendering is CPU/IO heavy; keep it off the main thread.
                val pdfFile = withContext(Dispatchers.IO) {
                    PdfGenerator.createInvoicePdf(
                        context = context,
                        transaction = data.transaction,
                        items = data.items,
                        registration = registration
                    )
                }
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
                        printMessage = "PDF error: ${e.toUserMessage()}"
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
                        printMessage = "Print error: ${e.toUserMessage()}"
                    )
                }
            }
        }
    }

    private fun buildFiscalReceiptJournal(data: TransactionWithItems): String {
        val registration = deviceRepository.registration.value
        return ReceiptJournal.build(
            data = data,
            header = ReceiptHeader.from(registration),
            registration = registration,
            isCopy = true
        )
    }
}
