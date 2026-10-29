package com.kra.paypoint.ui.screens.sales

import androidx.lifecycle.compose.collectAsStateWithLifecycle
import android.content.ActivityNotFoundException
import android.content.Intent
import android.net.Uri
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
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.domain.model.EtimsCodes.PaymentType
import com.kra.paypoint.domain.model.EtimsCodes.TaxType
import com.kra.paypoint.ui.components.NumberInputDialog
import com.kra.paypoint.ui.screens.sales.viewmodel.CartItem
import com.kra.paypoint.ui.screens.sales.viewmodel.SalesUiState
import com.kra.paypoint.ui.screens.sales.viewmodel.SalesViewModel

private enum class LineEdit { QUANTITY, DISCOUNT, DISCOUNT_AMOUNT, PRICE }

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SalesScreen(
    viewModel: SalesViewModel,
    onNavigateBack: () -> Unit,
    fromDocumentId: Long? = null
) {
    LaunchedEffect(fromDocumentId) { fromDocumentId?.let(viewModel::loadFromDocument) }
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    val availableItems by viewModel.availableItems.collectAsStateWithLifecycle()
    val customerResults by viewModel.customerResults.collectAsStateWithLifecycle()
    val context = LocalContext.current

    var searchQuery by remember { mutableStateOf("") }
    var showCustomerPicker by remember { mutableStateOf(false) }
    var showPayment by remember { mutableStateOf(false) }
    var showCancelSaleConfirm by remember { mutableStateOf(false) }
    var lineEdit by remember { mutableStateOf<Pair<CartItem, LineEdit>?>(null) }

    val filteredItems = remember(searchQuery, availableItems) {
        if (searchQuery.isBlank()) emptyList()
        else availableItems.filter {
            it.itemName.contains(searchQuery, ignoreCase = true) ||
                it.itemCode.contains(searchQuery, ignoreCase = true) ||
                (it.barcode != null && it.barcode.contains(searchQuery))
        }
    }

    uiState.completedSale?.let { sale ->
        AlertDialog(
            onDismissRequest = {},
            icon = {
                Icon(
                    Icons.Default.CheckCircle,
                    contentDescription = null,
                    tint = MaterialTheme.colorScheme.primary,
                    modifier = Modifier.size(48.dp)
                )
            },
            title = { Text("Invoice #${sale.invoiceNumber} Issued", fontWeight = FontWeight.Bold) },
            text = {
                Column(verticalArrangement = Arrangement.spacedBy(4.dp)) {
                    SummaryRow("Total", "KES %,.2f".format(sale.totalAmount), bold = true)
                    if (sale.isCredit) {
                        SummaryRow("Balance due", "KES %,.2f".format(sale.totalAmount))
                    } else {
                        SummaryRow("Received", "KES %,.2f".format(sale.amountReceived))
                        SummaryRow("Change", "KES %,.2f".format(sale.change))
                    }
                    Spacer(Modifier.height(4.dp))
                    Text(
                        "Saved on this device and queued for KRA eTIMS submission.",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                    uiState.printMessage?.let {
                        Text(it, style = MaterialTheme.typography.bodySmall, color = MaterialTheme.colorScheme.primary)
                    }
                    Spacer(Modifier.height(8.dp))
                    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        OutlinedButton(
                            onClick = viewModel::printLastReceipt,
                            enabled = !uiState.isPrinting,
                            modifier = Modifier.weight(1f)
                        ) {
                            Icon(Icons.Default.Print, contentDescription = null, modifier = Modifier.size(16.dp))
                            Spacer(Modifier.width(4.dp))
                            Text(if (uiState.isPrinting) "Printing…" else "Print")
                        }
                        OutlinedButton(
                            onClick = {
                                val body = viewModel.smsTextForLastReceipt() ?: return@OutlinedButton
                                val intent = Intent(Intent.ACTION_SENDTO, Uri.parse("smsto:${sale.customerPhone.orEmpty()}"))
                                    .putExtra("sms_body", body)
                                try {
                                    context.startActivity(intent)
                                } catch (_: ActivityNotFoundException) {
                                    // No SMS app on this device (e.g. a dedicated POS terminal) — nothing to hand off to.
                                }
                            },
                            modifier = Modifier.weight(1f)
                        ) {
                            Icon(Icons.Default.Sms, contentDescription = null, modifier = Modifier.size(16.dp))
                            Spacer(Modifier.width(4.dp))
                            Text("Send SMS")
                        }
                    }
                }
            },
            confirmButton = {
                Button(onClick = viewModel::startNewSale) { Text("New Sale") }
            }
        )
    }

    if (showCustomerPicker) {
        CustomerPickerDialog(
            customers = customerResults,
            onQueryChange = viewModel::onCustomerQueryChanged,
            onSelect = {
                viewModel.selectCustomer(it)
                showCustomerPicker = false
            },
            onDismiss = { showCustomerPicker = false }
        )
    }

    if (showPayment) {
        PaymentDialog(
            uiState = uiState,
            onPaymentTypeChange = viewModel::setPaymentType,
            onAmountChange = viewModel::onAmountReceivedChanged,
            onNoteChange = viewModel::onNoteChanged,
            onCreditDaysChange = viewModel::setCreditDays,
            onConfirm = {
                if (uiState.isPaymentSufficient && !(uiState.isCredit && uiState.selectedCustomer == null)) showPayment = false
                viewModel.checkout()
            },
            onDismiss = { showPayment = false }
        )
    }

    if (showCancelSaleConfirm) {
        AlertDialog(
            onDismissRequest = { showCancelSaleConfirm = false },
            title = { Text("Cancel Sale?") },
            text = { Text("All items in this sale will be removed. No invoice has been issued yet.") },
            confirmButton = {
                TextButton(onClick = {
                    viewModel.clearCart()
                    showCancelSaleConfirm = false
                }) { Text("Cancel Sale", color = MaterialTheme.colorScheme.error) }
            },
            dismissButton = { TextButton(onClick = { showCancelSaleConfirm = false }) { Text("Keep") } }
        )
    }

    lineEdit?.let { (line, kind) ->
        val code = line.item.itemCode
        when (kind) {
            LineEdit.QUANTITY -> NumberInputDialog(
                title = "Quantity – ${line.item.itemName}",
                label = "Quantity",
                initialValue = line.quantity,
                onConfirm = { viewModel.updateCartItemQuantity(code, it); lineEdit = null },
                onDismiss = { lineEdit = null }
            )
            LineEdit.DISCOUNT -> NumberInputDialog(
                title = "Discount – ${line.item.itemName}",
                label = "Discount (%)",
                initialValue = line.discountPercent,
                onConfirm = { viewModel.setDiscount(code, it); lineEdit = null },
                onDismiss = { lineEdit = null }
            )
            LineEdit.DISCOUNT_AMOUNT -> NumberInputDialog(
                title = "Discount – ${line.item.itemName}",
                label = "Discount amount (KES)",
                initialValue = line.discountAmount,
                onConfirm = { viewModel.setFixedDiscount(code, it); lineEdit = null },
                onDismiss = { lineEdit = null }
            )
            LineEdit.PRICE -> NumberInputDialog(
                title = "Change Price – ${line.item.itemName}",
                label = "Unit price (KES)",
                initialValue = line.unitPrice,
                onConfirm = { viewModel.changePrice(code, it); lineEdit = null },
                onDismiss = { lineEdit = null }
            )
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Sales", fontWeight = FontWeight.SemiBold) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                },
                actions = {
                    if (uiState.cart.isNotEmpty()) {
                        TextButton(onClick = { showCancelSaleConfirm = true }) {
                            Text("Cancel Sale", color = MaterialTheme.colorScheme.error)
                        }
                    }
                },
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
                    Column {
                        Text(
                            text = "KES %,.2f".format(uiState.totalAmount),
                            style = MaterialTheme.typography.titleLarge,
                            fontWeight = FontWeight.Bold,
                            color = MaterialTheme.colorScheme.primary
                        )
                        Text(
                            text = "VAT KES %,.2f".format(uiState.totalTaxAmount) +
                                if (uiState.totalDiscount > 0) " • Disc KES %,.2f".format(uiState.totalDiscount) else "",
                            style = MaterialTheme.typography.bodySmall,
                            color = MaterialTheme.colorScheme.secondary
                        )
                    }

                    Button(
                        onClick = { showPayment = true },
                        enabled = uiState.cart.isNotEmpty() && !uiState.isLoading,
                        shape = RoundedCornerShape(8.dp)
                    ) {
                        if (uiState.isLoading) {
                            CircularProgressIndicator(
                                modifier = Modifier.size(20.dp),
                                color = MaterialTheme.colorScheme.onPrimary,
                                strokeWidth = 2.dp
                            )
                        } else {
                            Icon(Icons.Default.Payments, contentDescription = null, modifier = Modifier.padding(end = 6.dp))
                            Text("Payment (${uiState.itemCount})", fontSize = 15.sp)
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

            uiState.sourceDocumentNumber?.let { number ->
                Text(
                    "Invoicing ${number} — it will be marked converted once this sale is issued.",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.primary,
                    modifier = Modifier.padding(bottom = 8.dp)
                )
            }

            CustomerBar(
                customer = uiState.selectedCustomer,
                onClick = { showCustomerPicker = true },
                onClear = { viewModel.selectCustomer(null) }
            )

            Spacer(Modifier.height(8.dp))

            OutlinedTextField(
                value = searchQuery,
                onValueChange = { searchQuery = it },
                placeholder = { Text("Search item by name, code or barcode") },
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

            if (filteredItems.isNotEmpty()) {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(top = 4.dp, bottom = 12.dp),
                    shape = RoundedCornerShape(8.dp),
                    elevation = CardDefaults.cardElevation(defaultElevation = 6.dp)
                ) {
                    LazyColumn(modifier = Modifier.heightIn(max = 240.dp)) {
                        items(filteredItems, key = { it.itemCode }) { item ->
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
                                Column(modifier = Modifier.weight(1f)) {
                                    Text(item.itemName, fontWeight = FontWeight.SemiBold)
                                    Text(
                                        "${item.itemCode} • ${TaxType.label(item.taxTypeCode)} • Stock ${qtyText(item.stockQuantity)}",
                                        style = MaterialTheme.typography.bodySmall,
                                        color = MaterialTheme.colorScheme.onSurfaceVariant
                                    )
                                }
                                Text(
                                    "KES %,.2f".format(item.defaultUnitPrice),
                                    fontWeight = FontWeight.Bold,
                                    color = MaterialTheme.colorScheme.primary
                                )
                            }
                            HorizontalDivider()
                        }
                    }
                }
            } else if (searchQuery.isNotBlank()) {
                Text(
                    "No matching items. Register items under Item Management.",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                    modifier = Modifier.padding(8.dp)
                )
            }

            Spacer(modifier = Modifier.height(12.dp))

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
                            "Register the item",
                            style = MaterialTheme.typography.titleMedium,
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                        )
                        Text(
                            "Search above to add items to this sale",
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
                    items(uiState.cart, key = { it.item.itemCode }) { cartItem ->
                        CartItemRow(
                            cartItem = cartItem,
                            onIncrement = { viewModel.updateCartItemQuantity(cartItem.item.itemCode, cartItem.quantity + 1.0) },
                            onDecrement = { viewModel.updateCartItemQuantity(cartItem.item.itemCode, cartItem.quantity - 1.0) },
                            onEditQuantity = { lineEdit = cartItem to LineEdit.QUANTITY },
                            onDiscount = { lineEdit = cartItem to LineEdit.DISCOUNT },
                            onDiscountAmount = { lineEdit = cartItem to LineEdit.DISCOUNT_AMOUNT },
                            onChangePrice = { lineEdit = cartItem to LineEdit.PRICE },
                            onRemove = { viewModel.removeCartItem(cartItem.item.itemCode) }
                        )
                    }
                }
            }
        }
    }
}

@Composable
private fun CustomerBar(customer: CustomerEntity?, onClick: () -> Unit, onClear: () -> Unit) {
    Card(
        modifier = Modifier.fillMaxWidth().clickable(onClick = onClick),
        shape = RoundedCornerShape(8.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.secondaryContainer.copy(alpha = 0.5f))
    ) {
        Row(
            modifier = Modifier.fillMaxWidth().padding(horizontal = 12.dp, vertical = 10.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Icon(Icons.Default.Badge, contentDescription = null, tint = MaterialTheme.colorScheme.secondary)
            Spacer(Modifier.width(10.dp))
            Column(modifier = Modifier.weight(1f)) {
                if (customer == null) {
                    Text("Walk-in customer", fontWeight = FontWeight.SemiBold)
                    Text("Tap to add buyer's PIN", style = MaterialTheme.typography.bodySmall)
                } else {
                    Text(customer.customerName, fontWeight = FontWeight.SemiBold)
                    Text("PIN: ${customer.customerTin}", style = MaterialTheme.typography.bodySmall)
                }
            }
            if (customer != null) {
                IconButton(onClick = onClear) { Icon(Icons.Default.Close, contentDescription = "Remove customer") }
            }
        }
    }
}

@Composable
private fun CustomerPickerDialog(
    customers: List<CustomerEntity>,
    onQueryChange: (String) -> Unit,
    onSelect: (CustomerEntity?) -> Unit,
    onDismiss: () -> Unit
) {
    var query by remember { mutableStateOf("") }
    DisposableEffect(Unit) { onDispose { onQueryChange("") } }
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("Select Customer") },
        text = {
            Column {
                OutlinedTextField(
                    value = query,
                    onValueChange = { query = it; onQueryChange(it) },
                    label = { Text("PIN, phone or customer name") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                Spacer(Modifier.height(8.dp))
                LazyColumn(modifier = Modifier.heightIn(max = 320.dp)) {
                    item {
                        ListItem(
                            headlineContent = { Text("Walk-in customer") },
                            supportingContent = { Text("No buyer PIN on the invoice") },
                            modifier = Modifier.clickable { onSelect(null) }
                        )
                        HorizontalDivider()
                    }
                    items(customers, key = { it.customerTin }) { c ->
                        ListItem(
                            headlineContent = { Text(c.customerName) },
                            supportingContent = { Text("${c.customerTin}${c.telNo?.let { " • $it" }.orEmpty()}") },
                            modifier = Modifier.clickable { onSelect(c) }
                        )
                    }
                    if (customers.isEmpty()) {
                        item {
                            Text(
                                "No registered customers match. Add them under Customer Management.",
                                style = MaterialTheme.typography.bodySmall,
                                modifier = Modifier.padding(8.dp)
                            )
                        }
                    }
                }
            }
        },
        confirmButton = { TextButton(onClick = onDismiss) { Text("Close") } }
    )
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun PaymentDialog(
    uiState: SalesUiState,
    onPaymentTypeChange: (String) -> Unit,
    onAmountChange: (String) -> Unit,
    onNoteChange: (String) -> Unit,
    onCreditDaysChange: (Int) -> Unit,
    onConfirm: () -> Unit,
    onDismiss: () -> Unit
) {
    val isCash = uiState.paymentTypeCode == PaymentType.CASH
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("Payment") },
        text = {
            Column(verticalArrangement = Arrangement.spacedBy(10.dp)) {
                SingleChoiceSegmentedButtonRow(modifier = Modifier.fillMaxWidth()) {
                    PaymentType.tillModes.forEachIndexed { index, mode ->
                        SegmentedButton(
                            selected = uiState.paymentTypeCode == mode.code,
                            onClick = { onPaymentTypeChange(mode.code) },
                            shape = SegmentedButtonDefaults.itemShape(index, PaymentType.tillModes.size)
                        ) { Text(mode.name, fontSize = 12.sp) }
                    }
                }
                SummaryRow("Amount to receive", "KES %,.2f".format(uiState.totalAmount), bold = true)
                if (isCash) {
                    OutlinedTextField(
                        value = uiState.amountReceivedInput,
                        onValueChange = onAmountChange,
                        label = { Text("Received (KES)") },
                        placeholder = { Text("%.2f".format(uiState.totalAmount)) },
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                        singleLine = true,
                        isError = !uiState.isPaymentSufficient,
                        supportingText = { if (!uiState.isPaymentSufficient) Text("Less than the amount due") },
                        modifier = Modifier.fillMaxWidth()
                    )
                    SummaryRow("Change", "KES %,.2f".format(uiState.change))
                }
                if (uiState.isCredit) {
                    Text("Due in", style = MaterialTheme.typography.labelLarge)
                    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        listOf(7, 14, 30, 60).forEach { days ->
                            FilterChip(
                                selected = uiState.creditDays == days,
                                onClick = { onCreditDaysChange(days) },
                                label = { Text("$days days") }
                            )
                        }
                    }
                    if (uiState.selectedCustomer == null) {
                        Text(
                            "Select the buyer first — a credit sale needs a registered customer.",
                            color = MaterialTheme.colorScheme.error,
                            style = MaterialTheme.typography.bodySmall
                        )
                    }
                }
                OutlinedTextField(
                    value = uiState.note,
                    onValueChange = onNoteChange,
                    label = { Text("Note (optional)") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                uiState.selectedCustomer?.let {
                    Text("Buyer: ${it.customerName} (${it.customerTin})", style = MaterialTheme.typography.bodySmall)
                }
            }
        },
        confirmButton = {
            Button(
                onClick = onConfirm,
                enabled = uiState.isPaymentSufficient && !uiState.isLoading && !(uiState.isCredit && uiState.selectedCustomer == null)
            ) {
                Text("Confirm")
            }
        },
        dismissButton = { TextButton(onClick = onDismiss) { Text("Cancel") } }
    )
}

@Composable
private fun SummaryRow(label: String, value: String, bold: Boolean = false) {
    Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
        Text(label, fontWeight = if (bold) FontWeight.Bold else FontWeight.Normal)
        Text(value, fontWeight = if (bold) FontWeight.Bold else FontWeight.SemiBold)
    }
}

private fun qtyText(value: Double): String = if (value % 1.0 == 0.0) value.toLong().toString() else "%.2f".format(value)

@Composable
fun CartItemRow(
    cartItem: CartItem,
    onIncrement: () -> Unit,
    onDecrement: () -> Unit,
    onEditQuantity: () -> Unit,
    onDiscount: () -> Unit,
    onDiscountAmount: () -> Unit,
    onChangePrice: () -> Unit,
    onRemove: () -> Unit
) {
    var menuOpen by remember { mutableStateOf(false) }
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
                Text(cartItem.item.itemName, style = MaterialTheme.typography.bodyMedium, fontWeight = FontWeight.Bold)
                Text(
                    text = "%,.2f × %s • %s".format(cartItem.unitPrice, qtyText(cartItem.quantity), TaxType.label(cartItem.taxTypeCode)) +
                        if (cartItem.priceOverride != null) " • price changed" else "",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                if (cartItem.discountPercent > 0) {
                    Text(
                        "Discount ${qtyText(cartItem.discountPercent)}% (−%,.2f)".format(cartItem.discountAmount),
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.tertiary
                    )
                }
                Text(
                    text = "KES %,.2f".format(cartItem.lineTotal),
                    style = MaterialTheme.typography.bodyMedium,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.primary
                )
            }

            Row(verticalAlignment = Alignment.CenterVertically) {
                IconButton(onClick = onDecrement) {
                    Icon(Icons.Default.RemoveCircleOutline, contentDescription = "Decrease")
                }
                Text(
                    text = qtyText(cartItem.quantity),
                    fontWeight = FontWeight.Bold,
                    modifier = Modifier.clickable(onClick = onEditQuantity).padding(horizontal = 8.dp)
                )
                IconButton(onClick = onIncrement) {
                    Icon(Icons.Default.AddCircleOutline, contentDescription = "Increase")
                }
                Box {
                    IconButton(onClick = { menuOpen = true }) {
                        Icon(Icons.Default.MoreVert, contentDescription = "Line options")
                    }
                    DropdownMenu(expanded = menuOpen, onDismissRequest = { menuOpen = false }) {
                        DropdownMenuItem(text = { Text("Quantity") }, onClick = { menuOpen = false; onEditQuantity() })
                        DropdownMenuItem(text = { Text("Discount (%)") }, onClick = { menuOpen = false; onDiscount() })
                        DropdownMenuItem(text = { Text("Discount (KES)") }, onClick = { menuOpen = false; onDiscountAmount() })
                        DropdownMenuItem(text = { Text("Change Price") }, onClick = { menuOpen = false; onChangePrice() })
                        DropdownMenuItem(
                            text = { Text("Cancel Item", color = MaterialTheme.colorScheme.error) },
                            onClick = { menuOpen = false; onRemove() }
                        )
                    }
                }
            }
        }
    }
}
