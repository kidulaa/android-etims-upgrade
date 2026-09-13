package com.kra.paypoint.ui.screens.creditnote.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.RefundItemParam
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.hardware.printer.PrinterService
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import javax.inject.Inject

data class RefundItemState(
    val originalItem: TransactionItemEntity,
    val refundQuantity: Double = 0.0,
    val isSelected: Boolean = false
) {
    val lineRefundTotal: Double
        get() = refundQuantity * originalItem.unitPrice
}

data class CreditNoteUiState(
    val searchInvoiceInput: String = "",
    val isLoading: Boolean = false,
    val originalTransactionWithItems: TransactionWithItems? = null,
    val refundItems: List<RefundItemState> = emptyList(),
    val selectedReasonCode: String = "01",
    val selectedReasonDescription: String = "Defective / Damaged Goods",
    val restockInventory: Boolean = true,
    val isProcessing: Boolean = false,
    val successCreditNoteNumber: Long? = null,
    val errorMessage: String? = null,
    val feedbackMessage: String? = null
) {
    val totalRefundQuantity: Double
        get() = refundItems.filter { it.isSelected }.sumOf { it.refundQuantity }

    val totalRefundAmount: Double
        get() = refundItems.filter { it.isSelected }.sumOf { it.lineRefundTotal }

    val refundTaxAmountA: Double
        get() = refundItems.filter { it.isSelected && it.originalItem.taxTypeCode == "A" }
            .sumOf { it.lineRefundTotal - (it.lineRefundTotal / 1.16) }

    val refundTaxAmountE: Double
        get() = refundItems.filter { it.isSelected && it.originalItem.taxTypeCode == "E" }
            .sumOf { it.lineRefundTotal - (it.lineRefundTotal / 1.08) }

    val totalRefundTax: Double
        get() = refundTaxAmountA + refundTaxAmountE

    val totalRefundTaxable: Double
        get() = totalRefundAmount - totalRefundTax
}

@HiltViewModel
class CreditNoteViewModel @Inject constructor(
    private val transactionRepository: TransactionRepository,
    private val authRepository: AuthRepository,
    private val printerService: PrinterService
) : ViewModel() {

    private val _uiState = MutableStateFlow(CreditNoteUiState())
    val uiState: StateFlow<CreditNoteUiState> = _uiState.asStateFlow()

    fun onSearchInvoiceChange(query: String) {
        _uiState.update { it.copy(searchInvoiceInput = query, errorMessage = null) }
    }

    fun lookupInvoice(invoiceNumber: Long? = null) {
        val targetInvc = invoiceNumber ?: _uiState.value.searchInvoiceInput.trim().toLongOrNull()
        if (targetInvc == null) {
            _uiState.update { it.copy(errorMessage = "Please enter a valid numeric invoice number.") }
            return
        }

        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, errorMessage = null, successCreditNoteNumber = null) }
            try {
                val transactionWithItems = transactionRepository.getTransactionByInvoiceNumber(targetInvc)
                if (transactionWithItems == null) {
                    _uiState.update {
                        it.copy(
                            isLoading = false,
                            errorMessage = "Invoice #$targetInvc not found. Verify invoice number and try again."
                        )
                    }
                    return@launch
                }

                if (transactionWithItems.transaction.salesTypeCode == "C" ||
                    transactionWithItems.transaction.receiptTypeCode == "R"
                ) {
                    _uiState.update {
                        it.copy(
                            isLoading = false,
                            errorMessage = "Invoice #$targetInvc is already a Credit Note / Refund. Cannot refund a Credit Note."
                        )
                    }
                    return@launch
                }

                val refundStates = transactionWithItems.items.map { item ->
                    RefundItemState(
                        originalItem = item,
                        refundQuantity = 0.0,
                        isSelected = false
                    )
                }

                _uiState.update {
                    it.copy(
                        isLoading = false,
                        originalTransactionWithItems = transactionWithItems,
                        refundItems = refundStates,
                        searchInvoiceInput = targetInvc.toString(),
                        errorMessage = null
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(isLoading = false, errorMessage = "Error retrieving invoice: ${e.localizedMessage}")
                }
            }
        }
    }

    fun toggleItemSelection(itemSequence: Int) {
        _uiState.update { state ->
            val updated = state.refundItems.map { r ->
                if (r.originalItem.itemSequence == itemSequence) {
                    val newSelection = !r.isSelected
                    val newQty = if (newSelection && r.refundQuantity <= 0.0) r.originalItem.quantity else r.refundQuantity
                    r.copy(isSelected = newSelection, refundQuantity = if (newSelection) newQty else 0.0)
                } else {
                    r
                }
            }
            state.copy(refundItems = updated, errorMessage = null)
        }
    }

    fun updateRefundQuantity(itemSequence: Int, quantity: Double) {
        _uiState.update { state ->
            val updated = state.refundItems.map { r ->
                if (r.originalItem.itemSequence == itemSequence) {
                    val clamped = quantity.coerceIn(0.0, r.originalItem.quantity)
                    r.copy(refundQuantity = clamped, isSelected = clamped > 0.0)
                } else {
                    r
                }
            }
            state.copy(refundItems = updated, errorMessage = null)
        }
    }

    fun selectAllForFullRefund() {
        _uiState.update { state ->
            val updated = state.refundItems.map { r ->
                r.copy(isSelected = true, refundQuantity = r.originalItem.quantity)
            }
            state.copy(refundItems = updated, errorMessage = null)
        }
    }

    fun clearSelection() {
        _uiState.update { state ->
            val updated = state.refundItems.map { r ->
                r.copy(isSelected = false, refundQuantity = 0.0)
            }
            state.copy(refundItems = updated, errorMessage = null)
        }
    }

    fun setRefundReason(code: String, description: String) {
        _uiState.update { it.copy(selectedReasonCode = code, selectedReasonDescription = description) }
    }

    fun setRestockInventory(enabled: Boolean) {
        _uiState.update { it.copy(restockInventory = enabled) }
    }

    fun issueCreditNote() {
        val state = _uiState.value
        val originalTx = state.originalTransactionWithItems?.transaction ?: run {
            _uiState.update { it.copy(errorMessage = "No original invoice loaded.") }
            return
        }

        val itemsToRefund = state.refundItems
            .filter { it.isSelected && it.refundQuantity > 0.0 }
            .map { r ->
                RefundItemParam(
                    itemCode = r.originalItem.itemCode,
                    itemClassificationCode = r.originalItem.itemClassificationCode,
                    itemName = r.originalItem.itemName,
                    barcode = r.originalItem.barcode,
                    packagingUnitCode = r.originalItem.packagingUnitCode,
                    packageQuantity = r.originalItem.packageQuantity,
                    quantityUnitCode = r.originalItem.quantityUnitCode,
                    quantity = r.refundQuantity,
                    unitPrice = r.originalItem.unitPrice,
                    taxRateClassificationCode = r.originalItem.taxTypeCode,
                    totalAmount = r.lineRefundTotal
                )
            }

        if (itemsToRefund.isEmpty()) {
            _uiState.update { it.copy(errorMessage = "Please select at least one item and quantity to refund.") }
            return
        }

        viewModelScope.launch {
            _uiState.update { it.copy(isProcessing = true, errorMessage = null, feedbackMessage = null) }
            try {
                val user = authRepository.getCurrentUser()
                val operatorId = user?.id ?: "SYSTEM"
                val operatorName = user?.name ?: "Cashier"

                val result = transactionRepository.issueCreditNote(
                    originalTransaction = originalTx,
                    itemsToRefund = itemsToRefund,
                    reasonCode = state.selectedReasonCode,
                    reasonDescription = state.selectedReasonDescription,
                    restockInventory = state.restockInventory,
                    operatorId = operatorId,
                    operatorName = operatorName
                )

                if (result.isSuccess) {
                    val creditNoteId = result.getOrThrow()
                    val creditNoteWithItems = transactionRepository.getTransactionDetails(creditNoteId)
                    val creditNoteNumber = creditNoteWithItems?.transaction?.invoiceNumber ?: (originalTx.invoiceNumber + 1L)

                    // Auto-print Credit Note Receipt
                    if (creditNoteWithItems != null) {
                        try {
                            val journal = formatCreditNoteReceiptJournal(
                                creditNote = creditNoteWithItems,
                                originalInvoiceNumber = originalTx.invoiceNumber,
                                reasonDesc = state.selectedReasonDescription,
                                isReprint = false
                            )
                            printerService.printReceipt(journal, isReprint = false)
                        } catch (pe: Exception) {
                            // Non-fatal thermal print error
                        }
                    }

                    _uiState.update {
                        it.copy(
                            isProcessing = false,
                            successCreditNoteNumber = creditNoteNumber,
                            feedbackMessage = "Fiscal Credit Note #$creditNoteNumber issued successfully for KES ${String.format("%.2f", state.totalRefundAmount)}."
                        )
                    }
                } else {
                    _uiState.update {
                        it.copy(
                            isProcessing = false,
                            errorMessage = "Credit Note failed: ${result.exceptionOrNull()?.localizedMessage}"
                        )
                    }
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(isProcessing = false, errorMessage = "Error processing Credit Note: ${e.localizedMessage}")
                }
            }
        }
    }

    fun reprintSuccessCreditNote() {
        val cnNumber = _uiState.value.successCreditNoteNumber ?: return
        viewModelScope.launch {
            try {
                val cnWithItems = transactionRepository.getTransactionByInvoiceNumber(cnNumber)
                val originalInvc = _uiState.value.originalTransactionWithItems?.transaction?.invoiceNumber ?: 0L
                if (cnWithItems != null) {
                    val journal = formatCreditNoteReceiptJournal(
                        creditNote = cnWithItems,
                        originalInvoiceNumber = originalInvc,
                        reasonDesc = _uiState.value.selectedReasonDescription,
                        isReprint = true
                    )
                    printerService.printReceipt(journal, isReprint = true)
                    _uiState.update { it.copy(feedbackMessage = "Credit Note #$cnNumber sent to printer (COPY).") }
                }
            } catch (e: Exception) {
                _uiState.update { it.copy(errorMessage = "Reprint failed: ${e.localizedMessage}") }
            }
        }
    }

    fun reset() {
        _uiState.update {
            CreditNoteUiState()
        }
    }

    private fun formatCreditNoteReceiptJournal(
        creditNote: TransactionWithItems,
        originalInvoiceNumber: Long,
        reasonDesc: String,
        isReprint: Boolean
    ): String {
        val t = creditNote.transaction
        return buildString {
            appendLine("================================")
            appendLine("     KRA FISCAL CREDIT NOTE     ")
            if (isReprint) {
                appendLine("             (COPY)             ")
            } else {
                appendLine("          (ORIGINAL)            ")
            }
            appendLine("================================")
            appendLine("CREDIT NOTE: #${t.invoiceNumber}")
            appendLine("ORIG INVC  : #$originalInvoiceNumber")
            appendLine("DATE/TIME  : ${t.salesDate}")
            appendLine("CUSTOMER   : ${t.customerName ?: "Walk-in"}")
            if (!t.customerTin.isNullOrBlank()) {
                appendLine("CUST PIN   : ${t.customerTin}")
            }
            appendLine("REASON     : $reasonDesc")
            appendLine("--------------------------------")
            appendLine("REFUNDED ITEMS:")
            creditNote.items.forEach { item ->
                appendLine("${item.itemName} x ${item.quantity}")
                appendLine(String.format("  @ KES %.2f = -KES %.2f [%s]", item.unitPrice, item.totalAmount, item.taxTypeCode))
            }
            appendLine("--------------------------------")
            appendLine(String.format("REFUND TAXABLE : -KES %.2f", t.totalTaxableAmount))
            appendLine(String.format("REFUND VAT     : -KES %.2f", t.totalTaxAmount))
            appendLine(String.format("TOTAL REFUND   : -KES %.2f", t.totalAmount))
            appendLine("--------------------------------")
            appendLine("SYNC STATUS    : ${t.syncStatus}")
            appendLine("================================")
            appendLine("       KRA eTIMS COMPLIANT      ")
            appendLine("================================")
        }
    }
}
