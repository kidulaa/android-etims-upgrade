package com.kra.paypoint.ui.screens.documents.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import android.content.Context
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.CommercialDocumentEntity
import com.kra.paypoint.data.local.entity.CommercialDocumentWithItems
import com.kra.paypoint.data.local.entity.DocumentType
import com.kra.paypoint.data.local.entity.PurchaseEntity
import com.kra.paypoint.data.local.entity.PurchaseWithItems
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.DocumentRepository
import com.kra.paypoint.domain.repository.PurchaseRepository
import com.kra.paypoint.ui.export.DocumentPdfGenerator
import dagger.hilt.android.lifecycle.HiltViewModel
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.File
import javax.inject.Inject

data class DocumentsUiState(
    val selectedDocument: CommercialDocumentWithItems? = null,
    val selectedPurchase: PurchaseWithItems? = null,
    val pdf: File? = null,
    val isBusy: Boolean = false,
    val message: String? = null
)

/** Backs both the sales-side (quotations/proformas) and purchase-side (LPOs/purchase invoices) lists. */
@HiltViewModel
class DocumentsViewModel @Inject constructor(
    private val documentRepository: DocumentRepository,
    private val purchaseRepository: PurchaseRepository,
    private val deviceRepository: DeviceRepository,
    @ApplicationContext private val context: Context
) : ViewModel() {

    private val _uiState = MutableStateFlow(DocumentsUiState())
    val uiState: StateFlow<DocumentsUiState> = _uiState.asStateFlow()

    val quotations = documents(DocumentType.QUOTATION)
    val proformas = documents(DocumentType.PROFORMA)
    val lpos = documents(DocumentType.LPO)

    val purchases: StateFlow<List<PurchaseEntity>> = purchaseRepository.observePurchases()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    private fun documents(type: String): StateFlow<List<CommercialDocumentEntity>> =
        documentRepository.observe(type).stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    fun openDocument(id: Long) {
        viewModelScope.launch { _uiState.update { it.copy(selectedDocument = documentRepository.get(id)) } }
    }

    fun openPurchase(id: Long) {
        viewModelScope.launch { _uiState.update { it.copy(selectedPurchase = purchaseRepository.getPurchase(id)) } }
    }

    fun closeDetail() {
        _uiState.update { it.copy(selectedDocument = null, selectedPurchase = null) }
    }

    fun cancelDocument(id: Long) {
        viewModelScope.launch {
            documentRepository.cancel(id)
            _uiState.update { it.copy(selectedDocument = null, message = "Document cancelled.") }
        }
    }

    /** "Download": renders off the main thread, then the screen hands the file to the share sheet. */
    fun downloadSelected() {
        val doc = _uiState.value.selectedDocument
        val purchase = _uiState.value.selectedPurchase
        if (_uiState.value.isBusy || (doc == null && purchase == null)) return
        viewModelScope.launch {
            _uiState.update { it.copy(isBusy = true) }
            val result = runCatching {
                withContext(Dispatchers.IO) {
                    val reg = deviceRepository.registration.value
                    if (doc != null) DocumentPdfGenerator.createForDocument(context, doc, reg)
                    else DocumentPdfGenerator.createForPurchase(context, purchase!!, reg)
                }
            }
            _uiState.update {
                result.fold(
                    onSuccess = { file -> it.copy(isBusy = false, pdf = file) },
                    onFailure = { e -> it.copy(isBusy = false, message = "Could not create PDF: ${e.toUserMessage()}") }
                )
            }
        }
    }

    fun pdfShown() {
        _uiState.update { it.copy(pdf = null) }
    }

    fun messageShown() {
        _uiState.update { it.copy(message = null) }
    }
}
