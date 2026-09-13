package com.kra.paypoint.ui.screens.receipts

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
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.ui.screens.receipts.viewmodel.ReceiptsViewModel
import com.kra.paypoint.ui.screens.receipts.viewmodel.SyncFilter

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ReceiptsScreen(
    viewModel: ReceiptsViewModel,
    onNavigateBack: () -> Unit,
    onIssueRefund: (Long) -> Unit = {}
) {
    val uiState by viewModel.uiState.collectAsState()
    val transactions by viewModel.filteredTransactions.collectAsState()

    // Receipt Detail Modal Dialog
    if (uiState.selectedTransactionWithItems != null) {
        val details = uiState.selectedTransactionWithItems!!
        ReceiptDetailDialog(
            details = details,
            isPrinting = uiState.isPrinting,
            printMessage = uiState.printMessage,
            onDismiss = viewModel::clearSelectedTransaction,
            onReprint = { viewModel.reprintReceipt(details) },
            onIssueRefund = onIssueRefund
        )
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Receipts & Fiscal History", fontWeight = FontWeight.SemiBold) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primary,
                    titleContentColor = MaterialTheme.colorScheme.onPrimary,
                    navigationIconContentColor = MaterialTheme.colorScheme.onPrimary
                )
            )
        }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(16.dp)
        ) {
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
                    items(transactions) { transaction ->
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

            SyncBadge(status = transaction.syncStatus)
        }
    }
}

@Composable
fun SyncBadge(status: String) {
    val (bgColor, textColor) = when (status) {
        "SYNCED" -> Color(0xFFE8F5E9) to Color(0xFF2E7D32)
        "PENDING" -> Color(0xFFFFF3E0) to Color(0xFFE65100)
        else -> Color(0xFFFFEBEE) to Color(0xFFC62828)
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
            Button(
                onClick = onReprint,
                enabled = !isPrinting
            ) {
                if (isPrinting) {
                    CircularProgressIndicator(modifier = Modifier.size(16.dp), color = MaterialTheme.colorScheme.onPrimary)
                    Spacer(modifier = Modifier.width(6.dp))
                    Text("Printing...")
                } else {
                    Icon(Icons.Default.Print, contentDescription = null, modifier = Modifier.size(16.dp).padding(end = 4.dp))
                    Text("Reprint")
                }
            }
        },
        dismissButton = {
            Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                if (t.salesTypeCode != "C" && t.receiptTypeCode != "R") {
                    OutlinedButton(
                        onClick = {
                            onDismiss()
                            onIssueRefund(t.invoiceNumber)
                        }
                    ) {
                        Icon(Icons.Default.RemoveCircleOutline, contentDescription = null, modifier = Modifier.size(16.dp))
                        Spacer(modifier = Modifier.width(4.dp))
                        Text("Issue Refund")
                    }
                }
                TextButton(onClick = onDismiss) {
                    Text("Close")
                }
            }
        }
    )
}
