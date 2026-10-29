package com.kra.paypoint.ui.screens.documents

import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Description
import androidx.compose.material.icons.filled.Search
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.kra.paypoint.data.local.entity.CommercialDocumentEntity
import com.kra.paypoint.data.local.entity.CommercialDocumentWithItems
import com.kra.paypoint.data.local.entity.DocumentType
import com.kra.paypoint.data.local.entity.PurchaseEntity
import com.kra.paypoint.data.local.entity.PurchaseWithItems
import com.kra.paypoint.ui.components.sharePdf
import com.kra.paypoint.ui.export.ReceiptJournal
import com.kra.paypoint.ui.screens.admin.StatusChip
import com.kra.paypoint.ui.screens.documents.viewmodel.DocumentsViewModel
import com.kra.paypoint.ui.screens.documents.viewmodel.EditorKind

/** Which side of the business the list shows, per the guide's Sales and Purchases modules. */
enum class DocumentsMode(val title: String, val tabs: List<EditorKind>) {
    SALES("Quotations & Proforma", listOf(EditorKind.QUOTATION, EditorKind.PROFORMA)),
    PURCHASES("Purchases", listOf(EditorKind.LPO, EditorKind.PURCHASE_INVOICE))
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DocumentsScreen(
    viewModel: DocumentsViewModel,
    mode: DocumentsMode,
    onCreate: (EditorKind) -> Unit,
    onEdit: (EditorKind, Long) -> Unit,
    onCreateInvoiceFrom: (Long) -> Unit,
    onCreatePurchaseFromLpo: (Long) -> Unit,
    onNavigateBack: () -> Unit
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    val quotations by viewModel.quotations.collectAsStateWithLifecycle()
    val proformas by viewModel.proformas.collectAsStateWithLifecycle()
    val lpos by viewModel.lpos.collectAsStateWithLifecycle()
    val purchases by viewModel.purchases.collectAsStateWithLifecycle()
    val context = LocalContext.current
    val snackbar = remember { SnackbarHostState() }

    var tab by rememberSaveable { mutableIntStateOf(0) }
    var query by rememberSaveable { mutableStateOf("") }
    val kind = mode.tabs[tab]

    LaunchedEffect(state.pdf) {
        state.pdf?.let { sharePdf(context, it, "Download / send"); viewModel.pdfShown() }
    }
    LaunchedEffect(state.message) {
        state.message?.let { snackbar.showSnackbar(it); viewModel.messageShown() }
    }

    state.selectedDocument?.let { doc ->
        DocumentDetailDialog(
            doc = doc,
            isBusy = state.isBusy,
            onDownload = viewModel::downloadSelected,
            onConvert = {
                viewModel.closeDetail()
                if (doc.document.docType == DocumentType.LPO) onCreatePurchaseFromLpo(doc.document.id)
                else onCreateInvoiceFrom(doc.document.id)
            },
            onEdit = {
                viewModel.closeDetail()
                onEdit(kind, doc.document.id)
            },
            onCancelDocument = { viewModel.cancelDocument(doc.document.id) },
            onDismiss = viewModel::closeDetail
        )
    }
    state.selectedPurchase?.let { p ->
        PurchaseDetailDialog(p, state.isBusy, viewModel::downloadSelected, viewModel::closeDetail)
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(mode.title, fontWeight = FontWeight.SemiBold) },
                navigationIcon = { IconButton(onClick = onNavigateBack) { Icon(Icons.Default.ArrowBack, contentDescription = "Back") } },
            )
        },
        floatingActionButton = {
            ExtendedFloatingActionButton(
                onClick = { onCreate(kind) },
                icon = { Icon(Icons.Default.Add, contentDescription = null) },
                text = { Text("New ${kind.title}") }
            )
        },
        snackbarHost = { SnackbarHost(snackbar) }
    ) { padding ->
        Column(Modifier.fillMaxSize().padding(padding)) {
            TabRow(selectedTabIndex = tab) {
                mode.tabs.forEachIndexed { i, k ->
                    Tab(selected = tab == i, onClick = { tab = i }, text = { Text(k.title) })
                }
            }
            OutlinedTextField(
                value = query,
                onValueChange = { query = it },
                leadingIcon = { Icon(Icons.Default.Search, contentDescription = null) },
                placeholder = { Text("Search number or name") },
                singleLine = true,
                modifier = Modifier.fillMaxWidth().padding(16.dp)
            )

            if (kind == EditorKind.PURCHASE_INVOICE) {
                val rows = purchases.filter {
                    query.isBlank() || it.supplierName.contains(query, true) || it.supplierInvoiceNumber.contains(query, true) ||
                        it.purchaseNumber.toString() == query.trim()
                }
                DocumentList(isEmpty = rows.isEmpty(), emptyText = "No purchase invoices recorded yet.") {
                    items(rows, key = { it.id }) { p -> PurchaseRow(p) { viewModel.openPurchase(p.id) } }
                }
            } else {
                val source = when (kind) {
                    EditorKind.QUOTATION -> quotations
                    EditorKind.PROFORMA -> proformas
                    else -> lpos
                }
                val rows = source.filter { query.isBlank() || it.partyName.contains(query, true) || it.displayNumber.contains(query, true) }
                DocumentList(isEmpty = rows.isEmpty(), emptyText = "No ${kind.title.lowercase()}s yet. Tap New to create one.") {
                    items(rows, key = { it.id }) { d -> DocumentRow(d) { viewModel.openDocument(d.id) } }
                }
            }
        }
    }
}

@Composable
private fun DocumentList(isEmpty: Boolean, emptyText: String, content: androidx.compose.foundation.lazy.LazyListScope.() -> Unit) {
    if (isEmpty) {
        Column(Modifier.fillMaxSize().padding(32.dp), horizontalAlignment = Alignment.CenterHorizontally, verticalArrangement = Arrangement.Center) {
            Icon(Icons.Default.Description, contentDescription = null, modifier = Modifier.size(56.dp), tint = MaterialTheme.colorScheme.outline)
            Spacer(Modifier.height(8.dp))
            Text(emptyText, textAlign = TextAlign.Center, color = MaterialTheme.colorScheme.onSurfaceVariant)
        }
    } else {
        LazyColumn(
            contentPadding = PaddingValues(start = 16.dp, end = 16.dp, bottom = 88.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp),
            content = content
        )
    }
}

@Composable
private fun DocumentRow(d: CommercialDocumentEntity, onClick: () -> Unit) {
    Card(onClick = onClick, modifier = Modifier.fillMaxWidth()) {
        Row(Modifier.padding(14.dp), verticalAlignment = Alignment.CenterVertically) {
            Column(Modifier.weight(1f)) {
                Text(d.partyName, fontWeight = FontWeight.SemiBold)
                Text("${d.displayNumber} • ${ReceiptJournal.formatDate(d.docDate)}", style = MaterialTheme.typography.bodySmall)
                if (d.status != "OPEN") {
                    Spacer(Modifier.height(4.dp))
                    StatusChip(if (d.status == "CONVERTED") "CONVERTED ${d.convertedReference.orEmpty()}" else d.status, isWarning = d.status == "CANCELLED")
                }
            }
            Text("KES %,.2f".format(d.totalAmount), fontWeight = FontWeight.Bold, color = MaterialTheme.colorScheme.primary)
        }
    }
}

@Composable
private fun PurchaseRow(p: PurchaseEntity, onClick: () -> Unit) {
    Card(onClick = onClick, modifier = Modifier.fillMaxWidth()) {
        Row(Modifier.padding(14.dp), verticalAlignment = Alignment.CenterVertically) {
            Column(Modifier.weight(1f)) {
                Text(p.supplierName, fontWeight = FontWeight.SemiBold)
                Text("PUR${p.purchaseNumber.toString().padStart(6, '0')} • inv ${p.supplierInvoiceNumber}", style = MaterialTheme.typography.bodySmall)
                Spacer(Modifier.height(4.dp))
                StatusChip(if (p.syncStatus == "SYNCED") "SYNCED" else "PENDING KRA", isWarning = p.syncStatus != "SYNCED")
            }
            Text("KES %,.2f".format(p.totalAmount), fontWeight = FontWeight.Bold, color = MaterialTheme.colorScheme.primary)
        }
    }
}

@Composable
private fun DocumentDetailDialog(
    doc: CommercialDocumentWithItems,
    isBusy: Boolean,
    onDownload: () -> Unit,
    onConvert: () -> Unit,
    onEdit: () -> Unit,
    onCancelDocument: () -> Unit,
    onDismiss: () -> Unit
) {
    val d = doc.document
    val isOpen = d.status == "OPEN"
    var confirmCancel by remember { mutableStateOf(false) }
    if (confirmCancel) {
        AlertDialog(
            onDismissRequest = { confirmCancel = false },
            title = { Text("Cancel ${d.displayNumber}?") },
            text = { Text("It will stay on record as cancelled and can no longer be invoiced.") },
            confirmButton = { TextButton(onClick = { confirmCancel = false; onCancelDocument() }) { Text("Cancel document", color = MaterialTheme.colorScheme.error) } },
            dismissButton = { TextButton(onClick = { confirmCancel = false }) { Text("Keep") } }
        )
    }
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("${DocumentType.label(d.docType)} ${d.displayNumber}") },
        text = {
            Column(verticalArrangement = Arrangement.spacedBy(4.dp)) {
                Text(d.partyName + (d.partyTin?.let { " • $it" } ?: ""), fontWeight = FontWeight.SemiBold)
                Text("Date ${ReceiptJournal.formatDate(d.docDate)}" + (d.dueDate?.let { " • Due ${ReceiptJournal.formatDate(it)}" } ?: ""), style = MaterialTheme.typography.bodySmall)
                HorizontalDivider(Modifier.padding(vertical = 6.dp))
                doc.items.sortedBy { it.itemSequence }.forEach {
                    Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                        Text("${it.itemName} × ${it.quantity.let { q -> if (q % 1.0 == 0.0) q.toLong().toString() else "%.2f".format(q) }}", Modifier.weight(1f), style = MaterialTheme.typography.bodySmall)
                        Text("%,.2f".format(it.totalAmount), style = MaterialTheme.typography.bodySmall)
                    }
                }
                HorizontalDivider(Modifier.padding(vertical = 6.dp))
                Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                    Text("Total", fontWeight = FontWeight.Bold)
                    Text("KES %,.2f".format(d.totalAmount), fontWeight = FontWeight.Bold)
                }
                d.note?.let { Text("Note: $it", style = MaterialTheme.typography.bodySmall) }
                if (!isOpen) StatusChip(d.status + (d.convertedReference?.let { " → $it" } ?: ""), isWarning = d.status == "CANCELLED")
                Spacer(Modifier.height(8.dp))
                FlowRowActions(
                    buildList {
                        add("Download" to onDownload)
                        if (isOpen) {
                            add((if (d.docType == DocumentType.LPO) "Create Purchase Invoice" else "Create Invoice") to onConvert)
                            add("Edit" to onEdit)
                            add("Cancel" to { confirmCancel = true })
                        }
                    },
                    enabled = !isBusy
                )
            }
        },
        confirmButton = { TextButton(onClick = onDismiss) { Text("Close") } }
    )
}

@Composable
private fun PurchaseDetailDialog(p: PurchaseWithItems, isBusy: Boolean, onDownload: () -> Unit, onDismiss: () -> Unit) {
    val pu = p.purchase
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("Purchase PUR${pu.purchaseNumber.toString().padStart(6, '0')}") },
        text = {
            Column(verticalArrangement = Arrangement.spacedBy(4.dp)) {
                Text("${pu.supplierName} • ${pu.supplierTin}", fontWeight = FontWeight.SemiBold)
                Text("Supplier invoice ${pu.supplierInvoiceNumber} • ${ReceiptJournal.formatDate(pu.purchaseDate)}", style = MaterialTheme.typography.bodySmall)
                HorizontalDivider(Modifier.padding(vertical = 6.dp))
                p.items.sortedBy { it.itemSequence }.forEach {
                    Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                        Text("${it.itemName} × ${it.quantity}", Modifier.weight(1f), style = MaterialTheme.typography.bodySmall)
                        Text("%,.2f".format(it.totalAmount), style = MaterialTheme.typography.bodySmall)
                    }
                }
                HorizontalDivider(Modifier.padding(vertical = 6.dp))
                Text("Total KES %,.2f (VAT %,.2f)".format(pu.totalAmount, pu.totalTaxAmount), fontWeight = FontWeight.Bold)
                Text(if (pu.restockedInventory) "Goods added to stock" else "Stock not adjusted", style = MaterialTheme.typography.bodySmall)
                StatusChip("KRA: ${pu.syncStatus}", isWarning = pu.syncStatus != "SYNCED")
                pu.lastError?.let { Text(it, color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall) }
                Spacer(Modifier.height(8.dp))
                FlowRowActions(listOf("Download" to onDownload), enabled = !isBusy)
            }
        },
        confirmButton = { TextButton(onClick = onDismiss) { Text("Close") } }
    )
}

@OptIn(ExperimentalLayoutApi::class)
@Composable
private fun FlowRowActions(actions: List<Pair<String, () -> Unit>>, enabled: Boolean) {
    FlowRow(horizontalArrangement = Arrangement.spacedBy(8.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
        actions.forEach { (label, action) ->
            OutlinedButton(onClick = action, enabled = enabled) { Text(label) }
        }
    }
}
