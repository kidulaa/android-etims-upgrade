package com.kra.paypoint.ui.screens.receipts

import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.kra.paypoint.ui.theme.PayPointTheme
import android.content.Intent
import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.shadow
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.core.content.FileProvider
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.isCreditNote
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.ui.screens.receipts.viewmodel.ReceiptsViewModel
import com.kra.paypoint.ui.screens.receipts.viewmodel.SyncFilter
import java.io.File

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ReceiptsScreen(
    viewModel: ReceiptsViewModel,
    /** Null when Receipts is shown as a top-level tab, which has no back arrow. */
    onNavigateBack: (() -> Unit)? = null,
    onIssueRefund: (Long) -> Unit = {}
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    val transactions by viewModel.filteredTransactions.collectAsStateWithLifecycle()
    val context = LocalContext.current

    // Handle PDF Sharing
    LaunchedEffect(uiState.generatedPdf) {
        uiState.generatedPdf?.let { file ->
            sharePdfFile(context, file)
            viewModel.clearGeneratedPdf()
        }
    }

    // Receipt Detail Modal Dialog
    if (uiState.selectedTransactionWithItems != null) {
        val details = uiState.selectedTransactionWithItems!!
        ReceiptDetailDialog(
            details = details,
            isPrinting = uiState.isPrinting,
            printMessage = uiState.printMessage,
            onDismiss = viewModel::clearSelectedTransaction,
            onReprint = { viewModel.reprintReceipt(details) },
            onSharePdf = { viewModel.generateInvoicePdf(details) },
            onMarkPaid = { viewModel.markPaid(details.transaction.id) },
            onIssueRefund = onIssueRefund
        )
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Receipts", fontWeight = FontWeight.SemiBold) },
                navigationIcon = {
                    if (onNavigateBack != null) {
                        IconButton(onClick = onNavigateBack) {
                            Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                        }
                    }
                },
            )
        }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(16.dp)
        ) {
            OutlinedTextField(
                value = uiState.searchQuery,
                onValueChange = viewModel::setSearchQuery,
                leadingIcon = { Icon(Icons.Default.Search, contentDescription = null) },
                placeholder = { Text("Search invoice no., customer or PIN") },
                singleLine = true,
                modifier = Modifier.fillMaxWidth().padding(bottom = 8.dp)
            )

            // Filter Chips
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 12.dp),
                horizontalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                FilterChip(
                    selected = uiState.filter == SyncFilter.ALL,
                    onClick = { viewModel.setFilter(SyncFilter.ALL) },
                    label = { Text("All") }
                )
                FilterChip(
                    selected = uiState.filter == SyncFilter.PENDING,
                    onClick = { viewModel.setFilter(SyncFilter.PENDING) },
                    label = { Text("Pending Sync") }
                )
                FilterChip(
                    selected = uiState.filter == SyncFilter.SYNCED,
                    onClick = { viewModel.setFilter(SyncFilter.SYNCED) },
                    label = { Text("Synced") }
                )
            }

            if (transactions.isEmpty()) {
                Box(
                    modifier = Modifier.fillMaxSize(),
                    contentAlignment = Alignment.Center
                ) {
                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                        Icon(
                            Icons.Default.ReceiptLong,
                            contentDescription = null,
                            modifier = Modifier.size(64.dp),
                            tint = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.2f)
                        )
                        Spacer(modifier = Modifier.height(8.dp))
                        Text(
                            "No transactions found",
                            style = MaterialTheme.typography.titleMedium,
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                        )
                    }
                }
            } else {
                LazyColumn(
                    modifier = Modifier.fillMaxSize(),
                    verticalArrangement = Arrangement.spacedBy(10.dp)
                ) {
                    items(transactions, key = { it.id }) { transaction ->
                        ReceiptItemCard(
                            transaction = transaction,
                            onClick = { viewModel.selectTransaction(transaction.id) }
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun ReceiptItemCard(
    transaction: TransactionEntity,
    onClick: () -> Unit
) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .clickable(onClick = onClick),
        shape = RoundedCornerShape(10.dp),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(14.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = "Invoice #${transaction.invoiceNumber}",
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold
                )
                Spacer(modifier = Modifier.height(2.dp))
                Text(
                    text = "${transaction.customerName ?: "Walk-in Customer"} • ${transaction.salesDate}",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Spacer(modifier = Modifier.height(4.dp))
                Text(
                    text = String.format("KES %.2f", transaction.totalAmount),
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.primary
                )
            }

            Column(horizontalAlignment = Alignment.End, verticalArrangement = Arrangement.spacedBy(4.dp)) {
                SyncBadge(status = transaction.syncStatus)
                if (!transaction.isPaid) UnpaidBadge()
            }
        }
    }
}

@Composable
fun UnpaidBadge() {
    Surface(color = MaterialTheme.colorScheme.errorContainer, shape = RoundedCornerShape(16.dp)) {
        Text(
            text = "UNPAID",
            color = MaterialTheme.colorScheme.onErrorContainer,
            fontSize = 12.sp,
            fontWeight = FontWeight.Bold,
            modifier = Modifier.padding(horizontal = 10.dp, vertical = 4.dp)
        )
    }
}

@Composable
fun SyncBadge(status: String) {
    val (bgColor, textColor) = when (status) {
        "SYNCED" -> PayPointTheme.status.successContainer to PayPointTheme.status.onSuccessContainer
        "PENDING", "SYNCING" -> PayPointTheme.status.warningContainer to PayPointTheme.status.onWarningContainer
        else -> MaterialTheme.colorScheme.errorContainer to MaterialTheme.colorScheme.onErrorContainer
    }

    Surface(
        color = bgColor,
        shape = RoundedCornerShape(16.dp)
    ) {
        Text(
            text = status,
            color = textColor,
            fontSize = 12.sp,
            fontWeight = FontWeight.Bold,
            modifier = Modifier.padding(horizontal = 10.dp, vertical = 4.dp)
        )
    }
}

@Composable
fun ReceiptDetailDialog(
    details: TransactionWithItems,
    isPrinting: Boolean,
    printMessage: String?,
    onDismiss: () -> Unit,
    onReprint: () -> Unit,
    onSharePdf: () -> Unit,
    onMarkPaid: () -> Unit = {},
    onIssueRefund: (Long) -> Unit = {}
) {
    val t = details.transaction

    AlertDialog(
        onDismissRequest = onDismiss,
        title = {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text("Fiscal Receipt #${t.invoiceNumber}", fontWeight = FontWeight.Bold)
                SyncBadge(status = t.syncStatus)
            }
        },
        text = {
            Column(modifier = Modifier.fillMaxWidth()) {
                Text("Date: ${t.salesDate}", style = MaterialTheme.typography.bodySmall)
                Text("Customer: ${t.customerName ?: "Walk-in Customer"}", style = MaterialTheme.typography.bodySmall)
                if (!t.customerTin.isNullOrBlank()) {
                    Text("Customer PIN: ${t.customerTin}", style = MaterialTheme.typography.bodySmall)
                }
                t.note?.let { Text("Note: $it", style = MaterialTheme.typography.bodySmall) }
                if (!t.isPaid) {
                    Row(
                        modifier = Modifier.fillMaxWidth().padding(top = 6.dp),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Column {
                            UnpaidBadge()
                            t.dueDate?.let { Text("Due ${it.takeLast(2)}/${it.substring(4, 6)}/${it.take(4)}", style = MaterialTheme.typography.bodySmall) }
                        }
                        OutlinedButton(onClick = onMarkPaid) { Text("Mark as paid") }
                    }
                }

                Divider(modifier = Modifier.padding(vertical = 8.dp))

                Text("Line Items:", fontWeight = FontWeight.Bold, style = MaterialTheme.typography.bodyMedium)
                Spacer(modifier = Modifier.height(4.dp))

                LazyColumn(modifier = Modifier.heightIn(max = 180.dp)) {
                    items(details.items) { item ->
                        Row(
                            modifier = Modifier.fillMaxWidth().padding(vertical = 2.dp),
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Text(
                                "${item.itemName} x${item.quantity.toInt()}",
                                style = MaterialTheme.typography.bodySmall,
                                modifier = Modifier.weight(1f)
                            )
                            Text(
                                String.format("KES %.2f", item.totalAmount),
                                style = MaterialTheme.typography.bodySmall,
                                fontWeight = FontWeight.SemiBold
                            )
                        }
                    }
                }

                Divider(modifier = Modifier.padding(vertical = 8.dp))

                Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                    Text("Taxable (Supply):", style = MaterialTheme.typography.bodySmall)
                    Text(String.format("KES %.2f", t.totalTaxableAmount), style = MaterialTheme.typography.bodySmall)
                }
                Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                    Text("Total VAT:", style = MaterialTheme.typography.bodySmall)
                    Text(String.format("KES %.2f", t.totalTaxAmount), style = MaterialTheme.typography.bodySmall)
                }
                Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                    Text("Grand Total:", fontWeight = FontWeight.Bold)
                    Text(String.format("KES %.2f", t.totalAmount), fontWeight = FontWeight.Bold, color = MaterialTheme.colorScheme.primary)
                }

                if (printMessage != null) {
                    Spacer(modifier = Modifier.height(8.dp))
                    Text(
                        text = printMessage,
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.primary
                    )
                }
            }
        },
        confirmButton = {
            Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                Button(
                    onClick = onSharePdf,
                    enabled = !isPrinting,
                    colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.secondary)
                ) {
                    Icon(Icons.Default.Share, contentDescription = null, modifier = Modifier.size(16.dp).padding(end = 4.dp))
                    Text("Share PDF")
                }
                
                Button(
                    onClick = onReprint,
                    enabled = !isPrinting
                ) {
                    if (isPrinting) {
                        CircularProgressIndicator(modifier = Modifier.size(16.dp), color = MaterialTheme.colorScheme.onPrimary)
                        Spacer(modifier = Modifier.width(6.dp))
                        Text("Wait...")
                    } else {
                        Icon(Icons.Default.Print, contentDescription = null, modifier = Modifier.size(16.dp).padding(end = 4.dp))
                        Text("Reprint")
                    }
                }
            }
        },
        dismissButton = {
            Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                if (!t.isCreditNote) {
                    OutlinedButton(
                        onClick = {
                            onDismiss()
                            onIssueRefund(t.invoiceNumber)
                        }
                    ) {
                        Icon(Icons.Default.RemoveCircleOutline, contentDescription = null, modifier = Modifier.size(16.dp))
                        Spacer(modifier = Modifier.width(4.dp))
                        Text("Refund")
                    }
                }
                TextButton(onClick = onDismiss) {
                    Text("Close")
                }
            }
        }
    )
}

private fun sharePdfFile(context: android.content.Context, file: File) {
    val uri = FileProvider.getUriForFile(
        context,
        "${context.packageName}.fileprovider",
        file
    )
    val intent = Intent(Intent.ACTION_SEND).apply {
        type = "application/pdf"
        putExtra(Intent.EXTRA_STREAM, uri)
        addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION)
    }
    context.startActivity(Intent.createChooser(intent, "Share Invoice"))
}
