package com.kra.paypoint.ui.screens.sales

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
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.window.Dialog
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.ui.screens.sales.viewmodel.CartItem
import com.kra.paypoint.ui.screens.sales.viewmodel.SalesViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SalesScreen(
    viewModel: SalesViewModel,
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    val availableItems by viewModel.availableItems.collectAsState()

    var searchQuery by remember { mutableStateOf("") }
    var showTaxBreakdown by remember { mutableStateOf(false) }

    // Filter available catalog based on search input
    val filteredItems = remember(searchQuery, availableItems) {
        if (searchQuery.isBlank()) emptyList()
        else availableItems.filter {
            it.itemName.contains(searchQuery, ignoreCase = true) ||
            it.itemCode.contains(searchQuery, ignoreCase = true) ||
            (it.barcode != null && it.barcode.contains(searchQuery))
        }
    }

    // Success Invoice Confirmation Dialog
    if (uiState.lastCompletedInvoiceNumber != null) {
        AlertDialog(
            onDismissRequest = viewModel::clearCart,
            icon = {
                Icon(
                    Icons.Default.CheckCircle,
                    contentDescription = null,
                    tint = MaterialTheme.colorScheme.primary,
                    modifier = Modifier.size(48.dp)
                )
            },
            title = {
                Text("Fiscal Receipt Created", fontWeight = FontWeight.Bold)
            },
            text = {
                Column {
                    Text("Invoice Number: #${uiState.lastCompletedInvoiceNumber}")
                    Spacer(modifier = Modifier.height(6.dp))
                    Text(
                        "Transaction has been securely committed offline to local Room database and queued for KRA eTIMS synchronization.",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            },
            confirmButton = {
                Button(onClick = viewModel::clearCart) {
                    Text("New Sale")
                }
            }
        )
    }

    // Tax Breakdown Modal Dialog
    if (showTaxBreakdown) {
        AlertDialog(
            onDismissRequest = { showTaxBreakdown = false },
            title = { Text("KRA Tax Calculation Breakdown", fontWeight = FontWeight.Bold) },
            text = {
                Column(modifier = Modifier.fillMaxWidth()) {
                    TaxRow(label = "Total Taxable (Supply):", value = String.format("KES %.2f", uiState.totalTaxableAmount))
                    TaxRow(label = "Total Tax Amount (VAT):", value = String.format("KES %.2f", uiState.totalTaxAmount))
                    Divider(modifier = Modifier.padding(vertical = 8.dp))
                    TaxRow(
                        label = "Gross Invoice Total:",
                        value = String.format("KES %.2f", uiState.totalAmount),
                        isBold = true
                    )
                }
            },
            confirmButton = {
                TextButton(onClick = { showTaxBreakdown = false }) {
                    Text("Close")
                }
            }
        )
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Fiscal Sales POS", fontWeight = FontWeight.SemiBold) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                },
                actions = {
                    if (uiState.cart.isNotEmpty()) {
                        IconButton(onClick = viewModel::clearCart) {
                            Icon(Icons.Default.DeleteSweep, contentDescription = "Clear Cart")
                        }
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primary,
                    titleContentColor = MaterialTheme.colorScheme.onPrimary,
                    navigationIconContentColor = MaterialTheme.colorScheme.onPrimary,
                    actionIconContentColor = MaterialTheme.colorScheme.onPrimary
                )
            )
        },
        bottomBar = {
            BottomAppBar(
                containerColor = MaterialTheme.colorScheme.surface,
                modifier = Modifier.shadow(8.dp)
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = 16.dp),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Column(
                        modifier = Modifier.clickable { showTaxBreakdown = true }
                    ) {
                        Row(verticalAlignment = Alignment.CenterVertically) {
                            Text("Total:", style = MaterialTheme.typography.bodyMedium)
                            Spacer(modifier = Modifier.width(4.dp))
                            Icon(
                                Icons.Default.Info,
                                contentDescription = "Tax Info",
                                modifier = Modifier.size(14.dp),
                                tint = MaterialTheme.colorScheme.primary
                            )
                        }
                        Text(
                            text = String.format("KES %.2f", uiState.totalAmount),
                            style = MaterialTheme.typography.titleLarge,
                            fontWeight = FontWeight.Bold,
                            color = MaterialTheme.colorScheme.primary
                        )
                        Text(
                            text = String.format("VAT: KES %.2f", uiState.totalTaxAmount),
                            style = MaterialTheme.typography.bodySmall,
                            color = MaterialTheme.colorScheme.secondary
                        )
                    }

                    Button(
                        onClick = viewModel::checkout,
                        enabled = uiState.cart.isNotEmpty() && !uiState.isLoading,
                        shape = RoundedCornerShape(8.dp),
                        colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.primary)
                    ) {
                        if (uiState.isLoading) {
                            CircularProgressIndicator(
                                modifier = Modifier.size(20.dp),
                                color = MaterialTheme.colorScheme.onPrimary,
                                strokeWidth = 2.dp
                            )
                        } else {
                            Icon(
                                Icons.Default.ShoppingCart,
                                contentDescription = null,
                                modifier = Modifier.padding(end = 6.dp)
                            )
                            Text("Checkout (${uiState.itemCount})", fontSize = 15.sp)
                        }
                    }
                }
            }
        }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(16.dp)
        ) {
            // Error notice if any
            AnimatedVisibility(visible = uiState.errorMessage != null) {
                Card(
                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp),
                    colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.errorContainer)
                ) {
                    Text(
                        text = uiState.errorMessage ?: "",
                        color = MaterialTheme.colorScheme.onErrorContainer,
                        style = MaterialTheme.typography.bodySmall,
                        modifier = Modifier.padding(12.dp)
                    )
                }
            }

            // Search Bar for Barcode or Item name
            OutlinedTextField(
                value = searchQuery,
                onValueChange = { searchQuery = it },
                placeholder = { Text("Scan Barcode or Search Catalog") },
                leadingIcon = { Icon(Icons.Default.Search, contentDescription = null) },
                trailingIcon = {
                    if (searchQuery.isNotEmpty()) {
                        IconButton(onClick = { searchQuery = "" }) {
                            Icon(Icons.Default.Clear, contentDescription = "Clear")
                        }
                    }
                },
                modifier = Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(8.dp),
                singleLine = true
            )

            // Live Search Suggestions Dropdown / List
            if (filteredItems.isNotEmpty()) {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(top = 4.dp, bottom = 12.dp),
                    shape = RoundedCornerShape(8.dp),
                    elevation = CardDefaults.cardElevation(defaultElevation = 6.dp)
                ) {
                    LazyColumn(modifier = Modifier.heightIn(max = 200.dp)) {
                        items(filteredItems) { item ->
                            Row(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .clickable {
                                        viewModel.addItemToCart(item)
                                        searchQuery = ""
                                    }
                                    .padding(12.dp),
                                horizontalArrangement = Arrangement.SpaceBetween,
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                Column {
                                    Text(item.itemName, fontWeight = FontWeight.SemiBold)
                                    Text(
                                        "${item.itemCode} • Tax: ${item.taxTypeCode}",
                                        style = MaterialTheme.typography.bodySmall,
                                        color = MaterialTheme.colorScheme.onSurfaceVariant
                                    )
                                }
                                Text(
                                    String.format("KES %.2f", item.defaultUnitPrice),
                                    fontWeight = FontWeight.Bold,
                                    color = MaterialTheme.colorScheme.primary
                                )
                            }
                            Divider()
                        }
                    }
                }
            }

            Spacer(modifier = Modifier.height(12.dp))

            // Cart Items List
            if (uiState.cart.isEmpty()) {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .weight(1f),
                    contentAlignment = Alignment.Center
                ) {
                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                        Icon(
                            Icons.Default.ShoppingBag,
                            contentDescription = null,
                            modifier = Modifier.size(64.dp),
                            tint = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.2f)
                        )
                        Spacer(modifier = Modifier.height(8.dp))
                        Text(
                            "Cart is empty",
                            style = MaterialTheme.typography.titleMedium,
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                        )
                        Text(
                            "Search catalog above to add items",
                            style = MaterialTheme.typography.bodySmall,
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.4f)
                        )
                    }
                }
            } else {
                LazyColumn(
                    modifier = Modifier
                        .fillMaxWidth()
                        .weight(1f),
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    items(uiState.cart) { cartItem ->
                        CartItemRow(
                            cartItem = cartItem,
                            onIncrement = {
                                viewModel.updateCartItemQuantity(
                                    cartItem.item.itemCode,
                                    cartItem.quantity + 1.0
                                )
                            },
                            onDecrement = {
                                viewModel.updateCartItemQuantity(
                                    cartItem.item.itemCode,
                                    cartItem.quantity - 1.0
                                )
                            },
                            onRemove = {
                                viewModel.removeCartItem(cartItem.item.itemCode)
                            }
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun CartItemRow(
    cartItem: CartItem,
    onIncrement: () -> Unit,
    onDecrement: () -> Unit,
    onRemove: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(8.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.4f))
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = cartItem.item.itemName,
                    style = MaterialTheme.typography.bodyMedium,
                    fontWeight = FontWeight.Bold
                )
                Text(
                    text = "Tax ${cartItem.item.taxTypeCode} • ${String.format("KES %.2f", cartItem.unitPrice)} each",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Text(
                    text = String.format("KES %.2f", cartItem.lineTotal),
                    style = MaterialTheme.typography.bodyMedium,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.primary
                )
            }

            Row(verticalAlignment = Alignment.CenterVertically) {
                IconButton(onClick = onDecrement, modifier = Modifier.size(32.dp)) {
                    Icon(Icons.Default.RemoveCircleOutline, contentDescription = "Decrease")
                }
                Text(
                    text = String.format("%.0f", cartItem.quantity),
                    fontWeight = FontWeight.Bold,
                    modifier = Modifier.padding(horizontal = 8.dp)
                )
                IconButton(onClick = onIncrement, modifier = Modifier.size(32.dp)) {
                    Icon(Icons.Default.AddCircleOutline, contentDescription = "Increase")
                }
                IconButton(onClick = onRemove, modifier = Modifier.size(32.dp)) {
                    Icon(
                        Icons.Default.DeleteOutline,
                        contentDescription = "Remove",
                        tint = MaterialTheme.colorScheme.error
                    )
                }
            }
        }
    }
}

@Composable
fun TaxRow(label: String, value: String, isBold: Boolean = false) {
    Row(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 4.dp),
        horizontalArrangement = Arrangement.SpaceBetween
    ) {
        Text(
            text = label,
            style = MaterialTheme.typography.bodyMedium,
            fontWeight = if (isBold) FontWeight.Bold else FontWeight.Normal
        )
        Text(
            text = value,
            style = MaterialTheme.typography.bodyMedium,
            fontWeight = if (isBold) FontWeight.Bold else FontWeight.SemiBold
        )
    }
}
