package com.kra.paypoint.ui.screens.creditnote

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardActions
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.shadow
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.ImeAction
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.kra.paypoint.ui.screens.creditnote.viewmodel.CreditNoteViewModel
import com.kra.paypoint.ui.screens.creditnote.viewmodel.RefundItemState

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun CreditNoteScreen(
    viewModel: CreditNoteViewModel,
    initialInvoiceNumber: Long? = null,
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    var showConfirmDialog by remember { mutableStateOf(false) }

    LaunchedEffect(initialInvoiceNumber) {
        if (initialInvoiceNumber != null && initialInvoiceNumber > 0L) {
            viewModel.lookupInvoice(initialInvoiceNumber)
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Credit Note & Refunds", fontWeight = FontWeight.Bold) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                },
                actions = {
                    if (uiState.originalTransactionWithItems != null) {
                        IconButton(onClick = { viewModel.reset() }) {
                            Icon(Icons.Default.Refresh, contentDescription = "Reset")
                        }
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.surfaceVariant
                )
            )
        }
    ) { paddingValues ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
                .padding(horizontal = 16.dp, vertical = 8.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp)
        ) {
            // Error banner
            if (uiState.errorMessage != null) {
                item {
                    Card(
                        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.errorContainer),
                        shape = RoundedCornerShape(12.dp)
                    ) {
                        Row(
                            modifier = Modifier.fillMaxWidth().padding(12.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            Icon(Icons.Default.Warning, contentDescription = "Error", tint = MaterialTheme.colorScheme.error)
                            Spacer(modifier = Modifier.width(8.dp))
                            Text(
                                text = uiState.errorMessage ?: "",
                                color = MaterialTheme.colorScheme.onErrorContainer,
                                fontSize = 13.sp
                            )
                        }
                    }
                }
            }

            // Success Card
            if (uiState.successCreditNoteNumber != null) {
                item {
                    Card(
                        colors = CardDefaults.cardColors(containerColor = Color(0xFFE8F5E9)),
                        shape = RoundedCornerShape(12.dp),
                        modifier = Modifier.shadow(2.dp, RoundedCornerShape(12.dp))
                    ) {
                        Column(modifier = Modifier.fillMaxWidth().padding(16.dp)) {
                            Row(verticalAlignment = Alignment.CenterVertically) {
                                Icon(Icons.Default.CheckCircle, contentDescription = "Success", tint = Color(0xFF2E7D32))
                                Spacer(modifier = Modifier.width(8.dp))
                                Text(
                                    text = "Credit Note #${uiState.successCreditNoteNumber} Issued!",
                                    fontWeight = FontWeight.Bold,
                                    fontSize = 16.sp,
                                    color = Color(0xFF1B5E20)
                                )
                            }
                            Spacer(modifier = Modifier.height(8.dp))
                            Text(
                                text = uiState.feedbackMessage ?: "Fiscal refund successfully registered and queued for KRA synchronization.",
                                fontSize = 13.sp,
                                color = Color(0xFF2E7D32)
                            )
                            Spacer(modifier = Modifier.height(12.dp))
                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.spacedBy(8.dp)
                            ) {
                                Button(
                                    onClick = { viewModel.reprintSuccessCreditNote() },
                                    modifier = Modifier.weight(1f),
                                    colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF2E7D32))
                                ) {
                                    Icon(Icons.Default.Print, contentDescription = "Reprint")
                                    Spacer(modifier = Modifier.width(6.dp))
                                    Text("Print Receipt")
                                }
                                OutlinedButton(
                                    onClick = { viewModel.reset() },
                                    modifier = Modifier.weight(1f)
                                ) {
                                    Text("New Refund")
                                }
                            }
                        }
                    }
                }
            }

            // Invoice Lookup Bar
            item {
                Card(
                    shape = RoundedCornerShape(12.dp),
                    colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface)
                ) {
                    Column(modifier = Modifier.fillMaxWidth().padding(12.dp)) {
                        Text("Original Invoice Lookup", fontWeight = FontWeight.SemiBold, fontSize = 14.sp)
                        Spacer(modifier = Modifier.height(8.dp))
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            verticalAlignment = Alignment.CenterVertically,
                            horizontalArrangement = Arrangement.spacedBy(8.dp)
                        ) {
                            OutlinedTextField(
                                value = uiState.searchInvoiceInput,
                                onValueChange = { viewModel.onSearchInvoiceChange(it) },
                                label = { Text("Invoice Number") },
                                placeholder = { Text("e.g. 1001") },
                                singleLine = true,
                                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number, imeAction = ImeAction.Search),
                                keyboardActions = KeyboardActions(onSearch = { viewModel.lookupInvoice() }),
                                modifier = Modifier.weight(1f)
                            )
                            Button(
                                onClick = { viewModel.lookupInvoice() },
                                enabled = !uiState.isLoading && uiState.searchInvoiceInput.isNotBlank(),
                                shape = RoundedCornerShape(8.dp)
                            ) {
                                if (uiState.isLoading) {
                                    CircularProgressIndicator(modifier = Modifier.size(18.dp), color = Color.White)
                                } else {
                                    Icon(Icons.Default.Search, contentDescription = "Search")
                                    Spacer(modifier = Modifier.width(4.dp))
                                    Text("Find")
                                }
                            }
                        }
                    }
                }
            }

            // Original Transaction Details Card
            val txWithItems = uiState.originalTransactionWithItems
            if (txWithItems != null) {
                val t = txWithItems.transaction
                item {
                    Card(
                        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant),
                        shape = RoundedCornerShape(12.dp)
                    ) {
                        Column(modifier = Modifier.fillMaxWidth().padding(14.dp)) {
                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.SpaceBetween,
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                Column {
                                    Text("Original Invoice #${t.invoiceNumber}", fontWeight = FontWeight.Bold, fontSize = 15.sp)
                                    Text("Date: ${t.salesDate}", fontSize = 12.sp, color = MaterialTheme.colorScheme.outline)
                                }
                                Surface(
                                    color = if (t.syncStatus == "SYNCED") Color(0xFFE8F5E9) else Color(0xFFFFF3E0),
                                    shape = RoundedCornerShape(6.dp)
                                ) {
                                    Text(
                                        text = t.syncStatus,
                                        fontSize = 11.sp,
                                        fontWeight = FontWeight.Bold,
                                        color = if (t.syncStatus == "SYNCED") Color(0xFF2E7D32) else Color(0xFFE65100),
                                        modifier = Modifier.padding(horizontal = 6.dp, vertical = 2.dp)
                                    )
                                }
                            }
                            Divider(modifier = Modifier.padding(vertical = 8.dp))
                            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                                Text("Customer: ${t.customerName ?: "Walk-in"}", fontSize = 13.sp)
                                Text(
                                    String.format("Original Total: KES %.2f", t.totalAmount),
                                    fontWeight = FontWeight.Bold,
                                    fontSize = 13.sp
                                )
                            }
                        }
                    }
                }

                // Quick selection controls
                item {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Text("Select Items to Refund", fontWeight = FontWeight.Bold, fontSize = 15.sp)
                        Row(horizontalArrangement = Arrangement.spacedBy(6.dp)) {
                            TextButton(onClick = { viewModel.selectAllForFullRefund() }) {
                                Text("Full Refund", fontSize = 12.sp)
                            }
                            TextButton(onClick = { viewModel.clearSelection() }) {
                                Text("Clear", fontSize = 12.sp)
                            }
                        }
                    }
                }

                // Refund items
                items(uiState.refundItems, key = { it.originalItem.itemSequence }) { refundItem ->
                    RefundItemCard(
                        itemState = refundItem,
                        onToggle = { viewModel.toggleItemSelection(refundItem.originalItem.itemSequence) },
                        onQuantityChange = { qty -> viewModel.updateRefundQuantity(refundItem.originalItem.itemSequence, qty) }
                    )
                }

                // Refund Options (Reason & Restock)
                item {
                    RefundOptionsCard(
                        selectedReasonCode = uiState.selectedReasonCode,
                        restockInventory = uiState.restockInventory,
                        onReasonChange = { code, desc -> viewModel.setRefundReason(code, desc) },
                        onRestockChange = { viewModel.setRestockInventory(it) }
                    )
                }

                // Financial Summary Card
                item {
                    Card(
                        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.primaryContainer),
                        shape = RoundedCornerShape(12.dp),
                        modifier = Modifier.shadow(2.dp, RoundedCornerShape(12.dp))
                    ) {
                        Column(modifier = Modifier.fillMaxWidth().padding(14.dp)) {
                            Text("Credit Note Financial Summary", fontWeight = FontWeight.Bold, fontSize = 14.sp)
                            Spacer(modifier = Modifier.height(8.dp))
                            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                                Text("Refund Items Quantity:", fontSize = 13.sp)
                                Text("${uiState.totalRefundQuantity}", fontWeight = FontWeight.SemiBold, fontSize = 13.sp)
                            }
                            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                                Text("Refund Taxable Subtotal:", fontSize = 13.sp)
                                Text(String.format("KES %.2f", uiState.totalRefundTaxable), fontSize = 13.sp)
                            }
                            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                                Text("Refund VAT:", fontSize = 13.sp)
                                Text(String.format("KES %.2f", uiState.totalRefundTax), fontSize = 13.sp)
                            }
                            Divider(modifier = Modifier.padding(vertical = 6.dp))
                            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                                Text("Total Refund Payable:", fontWeight = FontWeight.Bold, fontSize = 16.sp)
                                Text(
                                    String.format("KES %.2f", uiState.totalRefundAmount),
                                    fontWeight = FontWeight.Bold,
                                    fontSize = 16.sp,
                                    color = MaterialTheme.colorScheme.primary
                                )
                            }
                        }
                    }
                }

                // Issue Button
                item {
                    Button(
                        onClick = { showConfirmDialog = true },
                        enabled = !uiState.isProcessing && uiState.totalRefundQuantity > 0.0,
                        modifier = Modifier.fillMaxWidth().height(52.dp),
                        colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.error),
                        shape = RoundedCornerShape(10.dp)
                    ) {
                        if (uiState.isProcessing) {
                            CircularProgressIndicator(modifier = Modifier.size(24.dp), color = Color.White)
                        } else {
                            Icon(Icons.Default.ReceiptLong, contentDescription = "Issue")
                            Spacer(modifier = Modifier.width(8.dp))
                            Text(
                                "Issue Fiscal Credit Note (KES ${String.format("%.2f", uiState.totalRefundAmount)})",
                                fontWeight = FontWeight.Bold,
                                fontSize = 15.sp
                            )
                        }
                    }
                }
            }
        }
    }

    if (showConfirmDialog) {
        val t = uiState.originalTransactionWithItems?.transaction
        AlertDialog(
            onDismissRequest = { showConfirmDialog = false },
            icon = { Icon(Icons.Default.Warning, contentDescription = "Confirm", tint = MaterialTheme.colorScheme.error) },
            title = { Text("Confirm Credit Note") },
            text = {
                Column(verticalArrangement = Arrangement.spacedBy(6.dp)) {
                    Text("Are you sure you want to issue an official KRA Fiscal Credit Note against Invoice #${t?.invoiceNumber}?")
                    Spacer(modifier = Modifier.height(4.dp))
                    Text("• Total Refund: KES ${String.format("%.2f", uiState.totalRefundAmount)}", fontWeight = FontWeight.Bold)
                    Text("• Reason: ${uiState.selectedReasonDescription}")
                    Text("• Restock into Inventory: ${if (uiState.restockInventory) "YES" else "NO"}")
                    Spacer(modifier = Modifier.height(4.dp))
                    Text("This creates an immutable fiscal adjustment and queues it to KRA eTIMS.", fontSize = 12.sp, color = MaterialTheme.colorScheme.outline)
                }
            },
            confirmButton = {
                Button(
                    onClick = {
                        showConfirmDialog = false
                        viewModel.issueCreditNote()
                    },
                    colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.error)
                ) {
                    Text("Confirm & Issue")
                }
            },
            dismissButton = {
                OutlinedButton(onClick = { showConfirmDialog = false }) {
                    Text("Cancel")
                }
            }
        )
    }
}

@Composable
fun RefundItemCard(
    itemState: RefundItemState,
    onToggle: () -> Unit,
    onQuantityChange: (Double) -> Unit
) {
    val orig = itemState.originalItem
    Card(
        shape = RoundedCornerShape(10.dp),
        colors = CardDefaults.cardColors(
            containerColor = if (itemState.isSelected) MaterialTheme.colorScheme.surface else MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.5f)
        ),
        modifier = Modifier.fillMaxWidth()
    ) {
        Column(modifier = Modifier.fillMaxWidth().padding(12.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                verticalAlignment = Alignment.CenterVertically
            ) {
                Checkbox(
                    checked = itemState.isSelected,
                    onCheckedChange = { onToggle() }
                )
                Spacer(modifier = Modifier.width(6.dp))
                Column(modifier = Modifier.weight(1f)) {
                    Text(orig.itemName, fontWeight = FontWeight.Bold, fontSize = 14.sp)
                    Text(
                        "Code: ${orig.itemCode} | Tax: Rate ${orig.taxTypeCode}",
                        fontSize = 12.sp,
                        color = MaterialTheme.colorScheme.outline
                    )
                }
                Text(
                    String.format("KES %.2f", orig.unitPrice),
                    fontWeight = FontWeight.SemiBold,
                    fontSize = 13.sp
                )
            }

            AnimatedVisibility(visible = itemState.isSelected) {
                Column(modifier = Modifier.fillMaxWidth().padding(top = 8.dp)) {
                    Divider()
                    Spacer(modifier = Modifier.height(8.dp))
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Text(
                            "Purchased: ${orig.quantity} | Max Refund: ${orig.quantity}",
                            fontSize = 12.sp,
                            color = MaterialTheme.colorScheme.outline
                        )
                        Row(verticalAlignment = Alignment.CenterVertically) {
                            IconButton(
                                onClick = { onQuantityChange(itemState.refundQuantity - 1.0) },
                                enabled = itemState.refundQuantity > 0.0
                            ) {
                                Icon(Icons.Default.RemoveCircleOutline, contentDescription = "Decrease")
                            }
                            Text(
                                "${itemState.refundQuantity.toInt()}",
                                fontWeight = FontWeight.Bold,
                                fontSize = 15.sp,
                                modifier = Modifier.padding(horizontal = 6.dp)
                            )
                            IconButton(
                                onClick = { onQuantityChange(itemState.refundQuantity + 1.0) },
                                enabled = itemState.refundQuantity < orig.quantity
                            ) {
                                Icon(Icons.Default.AddCircleOutline, contentDescription = "Increase")
                            }
                        }
                    }
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.End
                    ) {
                        Text(
                            String.format("Item Refund: KES %.2f", itemState.lineRefundTotal),
                            fontWeight = FontWeight.Bold,
                            fontSize = 13.sp,
                            color = MaterialTheme.colorScheme.error
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun RefundOptionsCard(
    selectedReasonCode: String,
    restockInventory: Boolean,
    onReasonChange: (String, String) -> Unit,
    onRestockChange: (Boolean) -> Unit
) {
    val reasons = listOf(
        "01" to "Defective / Damaged Goods",
        "02" to "Pricing / Billing Error",
        "03" to "Customer Return / Dissatisfied",
        "04" to "Order Cancelled"
    )

    Card(
        shape = RoundedCornerShape(12.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface)
    ) {
        Column(modifier = Modifier.fillMaxWidth().padding(14.dp), verticalArrangement = Arrangement.spacedBy(10.dp)) {
            Text("Refund Configuration", fontWeight = FontWeight.Bold, fontSize = 14.sp)

            Text("Select Reason:", fontSize = 13.sp, color = MaterialTheme.colorScheme.outline)
            Column(verticalArrangement = Arrangement.spacedBy(4.dp)) {
                reasons.forEach { (code, desc) ->
                    Surface(
                        onClick = { onReasonChange(code, desc) },
                        shape = RoundedCornerShape(8.dp),
                        color = if (selectedReasonCode == code) MaterialTheme.colorScheme.primaryContainer else MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.4f),
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Row(
                            modifier = Modifier.fillMaxWidth().padding(10.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            RadioButton(
                                selected = selectedReasonCode == code,
                                onClick = { onReasonChange(code, desc) }
                            )
                            Spacer(modifier = Modifier.width(8.dp))
                            Text("[$code] $desc", fontSize = 13.sp, fontWeight = if (selectedReasonCode == code) FontWeight.Bold else FontWeight.Normal)
                        }
                    }
                }
            }

            Divider()

            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Column(modifier = Modifier.weight(1f)) {
                    Text("Restock into Inventory", fontWeight = FontWeight.SemiBold, fontSize = 13.sp)
                    Text("Automatically increment on-hand stock count", fontSize = 11.sp, color = MaterialTheme.colorScheme.outline)
                }
                Switch(
                    checked = restockInventory,
                    onCheckedChange = { onRestockChange(it) }
                )
            }
        }
    }
}
