package com.kra.paypoint.ui.screens.inventory

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
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
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.StockMovementEntity
import com.kra.paypoint.ui.screens.inventory.viewmodel.InventoryViewModel
import java.time.Instant
import java.time.ZoneId
import java.time.format.DateTimeFormatter

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun InventoryScreen(
    viewModel: InventoryViewModel,
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    val items by viewModel.filteredItems.collectAsState()
    val movements by viewModel.recentMovements.collectAsState()

    var selectedTab by remember { mutableStateOf(0) } // 0: Stock Levels, 1: Movement Audit Log

    // Stock Movement Dialog
    if (uiState.selectedItem != null) {
        val selectedItem = uiState.selectedItem!!
        StockMovementDialog(
            item = selectedItem,
            isProcessing = uiState.isProcessing,
            onDismiss = { viewModel.selectItem(null) },
            onStockIn = { qty, remark -> viewModel.stockIn(qty, remark) },
            onStockOut = { qty, reason, remark -> viewModel.stockOut(qty, "02", reason, remark) },
            onAdjust = { count, remark -> viewModel.adjustStock(count, remark) }
        )
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Stock & Inventory", fontWeight = FontWeight.SemiBold) },
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
            // Feedback notification banner
            AnimatedVisibility(visible = uiState.feedbackMessage != null) {
                Card(
                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp),
                    colors = CardDefaults.cardColors(
                        containerColor = if (uiState.isError)
                            MaterialTheme.colorScheme.errorContainer
                        else
                            MaterialTheme.colorScheme.primaryContainer
                    )
                ) {
                    Row(
                        modifier = Modifier.padding(12.dp),
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Icon(
                            imageVector = if (uiState.isError) Icons.Default.ErrorOutline else Icons.Default.CheckCircle,
                            contentDescription = null,
                            tint = if (uiState.isError) MaterialTheme.colorScheme.onErrorContainer else MaterialTheme.colorScheme.onPrimaryContainer,
                            modifier = Modifier.padding(end = 8.dp)
                        )
                        Text(
                            text = uiState.feedbackMessage ?: "",
                            style = MaterialTheme.typography.bodyMedium,
                            color = if (uiState.isError) MaterialTheme.colorScheme.onErrorContainer else MaterialTheme.colorScheme.onPrimaryContainer
                        )
                    }
                }
            }

            // Tab Row
            TabRow(
                selectedTabIndex = selectedTab,
                modifier = Modifier.fillMaxWidth().padding(bottom = 14.dp)
            ) {
                Tab(
                    selected = selectedTab == 0,
                    onClick = { selectedTab = 0 },
                    text = { Text("Stock Levels (${items.size})") }
                )
                Tab(
                    selected = selectedTab == 1,
                    onClick = { selectedTab = 1 },
                    text = { Text("Movement Log (${movements.size})") }
                )
            }

            if (selectedTab == 0) {
                // Search field
                OutlinedTextField(
                    value = uiState.searchQuery,
                    onValueChange = viewModel::setSearchQuery,
                    placeholder = { Text("Search item by name, barcode, or code") },
                    leadingIcon = { Icon(Icons.Default.Search, contentDescription = null) },
                    trailingIcon = {
                        if (uiState.searchQuery.isNotEmpty()) {
                            IconButton(onClick = { viewModel.setSearchQuery("") }) {
                                Icon(Icons.Default.Clear, contentDescription = "Clear")
                            }
                        }
                    },
                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp),
                    shape = RoundedCornerShape(8.dp),
                    singleLine = true
                )

                if (items.isEmpty()) {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        Text(
                            "No inventory items found. Sync Master Data first.",
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                        )
                    }
                } else {
                    LazyColumn(
                        modifier = Modifier.fillMaxSize(),
                        verticalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        items(items) { item ->
                            InventoryItemCard(
                                item = item,
                                onClick = { viewModel.selectItem(item) }
                            )
                        }
                    }
                }
            } else {
                // Movement Audit Log
                if (movements.isEmpty()) {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        Text(
                            "No stock movements recorded yet.",
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                        )
                    }
                } else {
                    LazyColumn(
                        modifier = Modifier.fillMaxSize(),
                        verticalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        items(movements) { movement ->
                            StockMovementCard(movement = movement)
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun InventoryItemCard(
    item: ItemEntity,
    onClick: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth().clickable(onClick = onClick),
        shape = RoundedCornerShape(8.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Row(
            modifier = Modifier.fillMaxWidth().padding(14.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(item.itemName, fontWeight = FontWeight.Bold, style = MaterialTheme.typography.titleMedium)
                Text(
                    "Code: ${item.itemCode} • Tax: ${item.taxTypeCode} • KES ${String.format("%.2f", item.defaultUnitPrice)}",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }

            Surface(
                color = when {
                    item.stockQuantity <= 0 -> Color(0xFFFFEBEE)
                    item.stockQuantity <= 20 -> Color(0xFFFFF3E0)
                    else -> Color(0xFFE8F5E9)
                },
                shape = RoundedCornerShape(12.dp)
            ) {
                Text(
                    text = "${item.stockQuantity.toInt()} ${item.packagingUnitCode}",
                    color = when {
                        item.stockQuantity <= 0 -> Color(0xFFC62828)
                        item.stockQuantity <= 20 -> Color(0xFFE65100)
                        else -> Color(0xFF2E7D32)
                    },
                    fontWeight = FontWeight.Bold,
                    fontSize = 13.sp,
                    modifier = Modifier.padding(horizontal = 10.dp, vertical = 5.dp)
                )
            }
        }
    }
}

@Composable
fun StockMovementCard(movement: StockMovementEntity) {
    val formatter = remember {
        DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm").withZone(ZoneId.systemDefault())
    }
    val dateStr = formatter.format(Instant.ofEpochMilli(movement.timestamp))

    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(8.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
        elevation = CardDefaults.cardElevation(defaultElevation = 1.dp)
    ) {
        Row(
            modifier = Modifier.fillMaxWidth().padding(12.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(movement.itemName, fontWeight = FontWeight.SemiBold)
                Text(
                    "${movement.reasonDescription} • $dateStr",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Text(
                    "Stock: ${movement.previousStock.toInt()} ➔ ${movement.newStock.toInt()}",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.primary,
                    fontWeight = FontWeight.Bold
                )
            }

            Surface(
                color = when (movement.movementType) {
                    "STOCK_IN" -> Color(0xFFE8F5E9)
                    "STOCK_OUT" -> Color(0xFFFFEBEE)
                    else -> Color(0xFFE3F2FD)
                },
                shape = RoundedCornerShape(8.dp)
            ) {
                Text(
                    text = when (movement.movementType) {
                        "STOCK_IN" -> "+${movement.quantity.toInt()}"
                        "STOCK_OUT" -> "-${movement.quantity.toInt()}"
                        else -> "Recount"
                    },
                    color = when (movement.movementType) {
                        "STOCK_IN" -> Color(0xFF2E7D32)
                        "STOCK_OUT" -> Color(0xFFC62828)
                        else -> Color(0xFF1565C0)
                    },
                    fontWeight = FontWeight.Bold,
                    fontSize = 13.sp,
                    modifier = Modifier.padding(horizontal = 8.dp, vertical = 4.dp)
                )
            }
        }
    }
}

@Composable
fun StockMovementDialog(
    item: ItemEntity,
    isProcessing: Boolean,
    onDismiss: () -> Unit,
    onStockIn: (Double, String?) -> Unit,
    onStockOut: (Double, String, String?) -> Unit,
    onAdjust: (Double, String?) -> Unit
) {
    var operationType by remember { mutableStateOf(0) } // 0: Stock In, 1: Stock Out, 2: Recount
    var quantityText by remember { mutableStateOf("") }
    var remarkText by remember { mutableStateOf("") }
    var reasonDescription by remember { mutableStateOf("Damaged Goods") }

    AlertDialog(
        onDismissRequest = onDismiss,
        title = {
            Column {
                Text("Manage Stock: ${item.itemName}", fontWeight = FontWeight.Bold, style = MaterialTheme.typography.titleMedium)
                Text("Current Inventory: ${item.stockQuantity.toInt()} units", style = MaterialTheme.typography.bodySmall, color = MaterialTheme.colorScheme.primary)
            }
        },
        text = {
            Column(modifier = Modifier.fillMaxWidth()) {
                // Operation Segmented Choice
                Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(4.dp)) {
                    FilterChip(
                        selected = operationType == 0,
                        onClick = { operationType = 0 },
                        label = { Text("Stock In", fontSize = 12.sp) },
                        modifier = Modifier.weight(1f)
                    )
                    FilterChip(
                        selected = operationType == 1,
                        onClick = { operationType = 1 },
                        label = { Text("Stock Out", fontSize = 12.sp) },
                        modifier = Modifier.weight(1f)
                    )
                    FilterChip(
                        selected = operationType == 2,
                        onClick = { operationType = 2 },
                        label = { Text("Recount", fontSize = 12.sp) },
                        modifier = Modifier.weight(1f)
                    )
                }

                Spacer(modifier = Modifier.height(12.dp))

                OutlinedTextField(
                    value = quantityText,
                    onValueChange = { quantityText = it },
                    label = {
                        Text(if (operationType == 2) "Verified Physical Count" else "Quantity (${item.packagingUnitCode})")
                    },
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true
                )

                if (operationType == 1) {
                    Spacer(modifier = Modifier.height(8.dp))
                    OutlinedTextField(
                        value = reasonDescription,
                        onValueChange = { reasonDescription = it },
                        label = { Text("Reason (e.g. Damage, Expired, Lost)") },
                        modifier = Modifier.fillMaxWidth(),
                        singleLine = true
                    )
                }

                Spacer(modifier = Modifier.height(8.dp))
                OutlinedTextField(
                    value = remarkText,
                    onValueChange = { remarkText = it },
                    label = { Text("Optional Remark / PO Number") },
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true
                )
            }
        },
        confirmButton = {
            Button(
                onClick = {
                    val qty = quantityText.toDoubleOrNull() ?: 0.0
                    when (operationType) {
                        0 -> onStockIn(qty, remarkText.ifBlank { null })
                        1 -> onStockOut(qty, reasonDescription, remarkText.ifBlank { null })
                        2 -> onAdjust(qty, remarkText.ifBlank { null })
                    }
                },
                enabled = !isProcessing && quantityText.toDoubleOrNull() != null
            ) {
                if (isProcessing) {
                    CircularProgressIndicator(modifier = Modifier.size(16.dp), color = MaterialTheme.colorScheme.onPrimary)
                } else {
                    Text("Save Movement")
                }
            }
        },
        dismissButton = {
            TextButton(onClick = onDismiss) {
                Text("Cancel")
            }
        }
    )
}
