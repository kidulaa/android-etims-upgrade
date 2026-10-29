package com.kra.paypoint.ui.screens.documents

import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.DeleteOutline
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.domain.model.Discount
import com.kra.paypoint.domain.model.EtimsCodes.PaymentType
import com.kra.paypoint.domain.model.EtimsCodes.TaxType
import com.kra.paypoint.ui.screens.documents.viewmodel.DocumentEditorViewModel
import com.kra.paypoint.ui.screens.documents.viewmodel.EditorKind
import com.kra.paypoint.ui.screens.documents.viewmodel.EditorLine

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DocumentEditorScreen(
    viewModel: DocumentEditorViewModel,
    onDone: () -> Unit,
    onNavigateBack: () -> Unit
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    val items by viewModel.items.collectAsStateWithLifecycle()
    val customers by viewModel.customers.collectAsStateWithLifecycle()
    val suppliers by viewModel.suppliers.collectAsStateWithLifecycle()
    val kind = state.kind

    var showPartyPicker by remember { mutableStateOf(false) }
    var showItemPicker by remember { mutableStateOf(false) }
    var editing by remember { mutableStateOf<Pair<ItemEntity, EditorLine?>?>(null) }

    LaunchedEffect(state.savedId) { if (state.savedId != null) onDone() }

    if (showPartyPicker) {
        PartyPickerDialog(
            isSupplier = kind.isPurchaseSide,
            customers = customers,
            suppliers = suppliers,
            onQuery = viewModel::onPartyQueryChanged,
            onPickCustomer = { viewModel.selectCustomer(it); showPartyPicker = false },
            onPickSupplier = { viewModel.selectSupplier(it); showPartyPicker = false },
            onDismiss = { showPartyPicker = false }
        )
    }
    if (showItemPicker) {
        ItemPickerDialog(
            items = items,
            onPick = { item ->
                showItemPicker = false
                editing = item to state.lines.firstOrNull { it.item.itemCode == item.itemCode }
            },
            onDismiss = { showItemPicker = false }
        )
    }
    editing?.let { (item, existing) ->
        LineDialog(
            item = item,
            existing = existing,
            isPurchase = kind.isPurchaseSide,
            onSave = { qty, price, discount ->
                viewModel.upsertLine(item, qty, price, discount)
                editing = null
            },
            onDismiss = { editing = null }
        )
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(if (state.existingId != null) "Edit ${kind.title}" else "Create ${kind.title}", fontWeight = FontWeight.SemiBold) },
                navigationIcon = { IconButton(onClick = onNavigateBack) { Icon(Icons.Default.ArrowBack, contentDescription = "Back") } },
            )
        },
        bottomBar = {
            Surface(tonalElevation = 3.dp) {
                Row(
                    modifier = Modifier.fillMaxWidth().padding(horizontal = 16.dp, vertical = 10.dp),
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Column {
                        Text("KES %,.2f".format(state.total), style = MaterialTheme.typography.titleLarge, fontWeight = FontWeight.Bold)
                        Text(
                            "VAT %,.2f".format(state.tax) + if (state.discount > 0) " • Disc %,.2f".format(state.discount) else "",
                            style = MaterialTheme.typography.bodySmall
                        )
                    }
                    Button(onClick = viewModel::save, enabled = !state.isSaving && state.lines.isNotEmpty()) {
                        if (state.isSaving) CircularProgressIndicator(Modifier.size(18.dp), strokeWidth = 2.dp)
                        else Text(if (kind == EditorKind.PURCHASE_INVOICE) "Record" else "Save")
                    }
                }
            }
        }
    ) { padding ->
        Column(
            modifier = Modifier.fillMaxSize().padding(padding).verticalScroll(rememberScrollState()).padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp)
        ) {
            if (kind == EditorKind.PROFORMA || kind == EditorKind.QUOTATION) {
                AssistChip(onClick = {}, label = { Text("Not a tax invoice — create an invoice from it when the customer pays") })
            }
            state.errorMessage?.let {
                Card(colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.errorContainer)) {
                    Text(it, color = MaterialTheme.colorScheme.onErrorContainer, modifier = Modifier.padding(12.dp))
                }
            }

            Text(if (kind.isPurchaseSide) "Supplier Details" else "Customer Details", style = MaterialTheme.typography.titleSmall)
            if (kind.isPurchaseSide) {
                OutlinedCard(onClick = { showPartyPicker = true }, modifier = Modifier.fillMaxWidth()) {
                    Column(Modifier.padding(12.dp)) {
                        Text(state.selectedSupplier?.supplierName ?: "Select supplier…", fontWeight = FontWeight.SemiBold)
                        state.selectedSupplier?.let { Text("PIN ${it.supplierTin}", style = MaterialTheme.typography.bodySmall) }
                    }
                }
            } else {
                OutlinedTextField(
                    value = state.partyName,
                    onValueChange = viewModel::setPartyName,
                    label = { Text("Customer name") },
                    supportingText = { state.partyTin?.let { Text("Registered customer • PIN $it") } },
                    trailingIcon = { TextButton(onClick = { showPartyPicker = true }) { Text("Select") } },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
            }

            Text("Details", style = MaterialTheme.typography.titleSmall)
            if (kind == EditorKind.PURCHASE_INVOICE) {
                OutlinedTextField(
                    value = state.supplierInvoiceNumber,
                    onValueChange = { v -> viewModel.update { it.copy(supplierInvoiceNumber = v) } },
                    label = { Text("Supplier's invoice number") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                Text("Payment", style = MaterialTheme.typography.labelLarge)
                Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                    listOf(PaymentType.CASH, PaymentType.MOBILE_MONEY, PaymentType.CREDIT).forEach { code ->
                        FilterChip(
                            selected = state.paymentTypeCode == code,
                            onClick = { viewModel.update { it.copy(paymentTypeCode = code) } },
                            label = { Text(PaymentType.label(code)) }
                        )
                    }
                }
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Switch(checked = state.restockInventory, onCheckedChange = { v -> viewModel.update { it.copy(restockInventory = v) } })
                    Spacer(Modifier.width(8.dp))
                    Text("Add received goods to stock")
                }
            }
            if (!kind.isPurchaseSide) {
                Text("Due in", style = MaterialTheme.typography.labelLarge)
                Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                    listOf(7, 14, 30, 60).forEach { days ->
                        FilterChip(
                            selected = state.dueInDays == days,
                            onClick = { viewModel.update { it.copy(dueInDays = days) } },
                            label = { Text("$days days") }
                        )
                    }
                }
            }
            OutlinedTextField(
                value = state.note,
                onValueChange = { v -> viewModel.update { it.copy(note = v) } },
                label = { Text("Add note") },
                minLines = 2,
                modifier = Modifier.fillMaxWidth()
            )

            Row(verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.SpaceBetween, modifier = Modifier.fillMaxWidth()) {
                Text("Items", style = MaterialTheme.typography.titleSmall)
                FilledTonalButton(onClick = { showItemPicker = true }) {
                    Icon(Icons.Default.Add, contentDescription = null, modifier = Modifier.size(18.dp))
                    Spacer(Modifier.width(4.dp))
                    Text("Add Item")
                }
            }
            if (state.lines.isEmpty()) {
                Text(
                    "No items yet. Add products or services from your item list.",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
            state.lines.forEach { line ->
                ElevatedCard(onClick = { editing = line.item to line }, modifier = Modifier.fillMaxWidth()) {
                    Row(Modifier.padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
                        Column(Modifier.weight(1f)) {
                            Text(line.item.itemName, fontWeight = FontWeight.SemiBold)
                            Text(
                                "%s × %,.2f • %s".format(qty(line.quantity), line.unitPrice, TaxType.label(line.taxTypeCode)) +
                                    if (line.priced.discountAmount > 0) " • −%,.2f".format(line.priced.discountAmount) else "",
                                style = MaterialTheme.typography.bodySmall
                            )
                        }
                        Text("%,.2f".format(line.priced.lineTotal), fontWeight = FontWeight.Bold)
                        IconButton(onClick = { viewModel.removeLine(line.item.itemCode) }) {
                            Icon(Icons.Default.DeleteOutline, contentDescription = "Remove ${line.item.itemName}")
                        }
                    }
                }
            }
            Spacer(Modifier.height(24.dp))
        }
    }
}

private fun qty(v: Double) = if (v % 1.0 == 0.0) v.toLong().toString() else "%.2f".format(v)

@Composable
private fun PartyPickerDialog(
    isSupplier: Boolean,
    customers: List<com.kra.paypoint.data.local.entity.CustomerEntity>,
    suppliers: List<com.kra.paypoint.data.local.entity.SupplierEntity>,
    onQuery: (String) -> Unit,
    onPickCustomer: (com.kra.paypoint.data.local.entity.CustomerEntity) -> Unit,
    onPickSupplier: (com.kra.paypoint.data.local.entity.SupplierEntity) -> Unit,
    onDismiss: () -> Unit
) {
    var query by remember { mutableStateOf("") }
    DisposableEffect(Unit) { onDispose { onQuery("") } }
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text(if (isSupplier) "Select Supplier" else "Select Customer") },
        text = {
            Column {
                OutlinedTextField(
                    value = query,
                    onValueChange = { query = it; onQuery(it) },
                    label = { Text("Search PIN or name") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                LazyColumn(Modifier.heightIn(max = 320.dp)) {
                    if (isSupplier) {
                        items(suppliers, key = { it.supplierTin }) { s ->
                            ListItem(
                                headlineContent = { Text(s.supplierName) },
                                supportingContent = { Text(s.supplierTin) },
                                modifier = Modifier.clickable { onPickSupplier(s) }
                            )
                        }
                    } else {
                        items(customers, key = { it.customerTin }) { c ->
                            ListItem(
                                headlineContent = { Text(c.customerName) },
                                supportingContent = { Text(c.customerTin) },
                                modifier = Modifier.clickable { onPickCustomer(c) }
                            )
                        }
                    }
                    val empty = if (isSupplier) suppliers.isEmpty() else customers.isEmpty()
                    if (empty) {
                        item {
                            Text(
                                if (isSupplier) "No suppliers match. Add them under Suppliers." else "No customers match. Add them under Customer Management.",
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

@Composable
private fun ItemPickerDialog(items: List<ItemEntity>, onPick: (ItemEntity) -> Unit, onDismiss: () -> Unit) {
    var query by remember { mutableStateOf("") }
    val filtered = remember(items, query) {
        items.filter { query.isBlank() || it.itemName.contains(query, true) || it.itemCode.contains(query, true) }
    }
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("Add Item") },
        text = {
            Column {
                OutlinedTextField(
                    value = query,
                    onValueChange = { query = it },
                    label = { Text("Search product or service") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                LazyColumn(Modifier.heightIn(max = 360.dp)) {
                    items(filtered, key = { it.itemCode }) { item ->
                        ListItem(
                            headlineContent = { Text(item.itemName) },
                            supportingContent = {
                                Text((if (item.itemTypeCode == "3") "Service" else "Stock ${qty(item.stockQuantity)}") + " • KES %,.2f".format(item.defaultUnitPrice))
                            },
                            modifier = Modifier.clickable { onPick(item) }
                        )
                    }
                    if (filtered.isEmpty()) {
                        item { Text("No items. Register products and services under Item Management.", modifier = Modifier.padding(8.dp)) }
                    }
                }
            }
        },
        confirmButton = { TextButton(onClick = onDismiss) { Text("Close") } }
    )
}

@Composable
private fun LineDialog(
    item: ItemEntity,
    existing: EditorLine?,
    isPurchase: Boolean,
    onSave: (Double, Double, Discount) -> Unit,
    onDismiss: () -> Unit
) {
    var qtyText by remember { mutableStateOf(existing?.quantity?.let(::qty) ?: "1") }
    var priceText by remember { mutableStateOf("%.2f".format(existing?.unitPrice ?: item.defaultUnitPrice)) }
    val existingDiscount = existing?.discount
    var isPercent by remember { mutableStateOf(existingDiscount !is Discount.Fixed) }
    var discountText by remember {
        mutableStateOf(
            when (existingDiscount) {
                is Discount.Fixed -> "%.2f".format(existingDiscount.amount)
                is Discount.Percent -> if (existingDiscount.rate > 0) qty(existingDiscount.rate) else ""
                null -> ""
            }
        )
    }
    val q = qtyText.toDoubleOrNull()
    val p = priceText.replace(",", "").toDoubleOrNull()
    val d = discountText.ifBlank { "0" }.replace(",", "").toDoubleOrNull()

    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text(item.itemName) },
        text = {
            Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                OutlinedTextField(
                    value = priceText, onValueChange = { priceText = it },
                    label = { Text(if (isPurchase) "Unit cost (KES, VAT incl.)" else "Unit price (KES)") },
                    isError = p == null, singleLine = true,
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = qtyText, onValueChange = { qtyText = it },
                    label = { Text("Quantity") },
                    supportingText = { if (item.itemTypeCode != "3" && !isPurchase) Text("In stock: ${qty(item.stockQuantity)}") },
                    isError = q == null || q <= 0, singleLine = true,
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                    modifier = Modifier.fillMaxWidth()
                )
                Row(verticalAlignment = Alignment.CenterVertically) {
                    FilterChip(selected = isPercent, onClick = { isPercent = true }, label = { Text("Percentage %") })
                    Spacer(Modifier.width(8.dp))
                    FilterChip(selected = !isPercent, onClick = { isPercent = false }, label = { Text("Fixed KES") })
                }
                OutlinedTextField(
                    value = discountText, onValueChange = { discountText = it },
                    label = { Text("Discount (optional)") },
                    isError = d == null, singleLine = true,
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                    modifier = Modifier.fillMaxWidth()
                )
            }
        },
        confirmButton = {
            TextButton(
                enabled = q != null && q > 0 && p != null && d != null,
                onClick = { onSave(q!!, p!!, if (isPercent) Discount.Percent(d!!) else Discount.Fixed(d!!)) }
            ) { Text(if (existing == null) "Add" else "Update") }
        },
        dismissButton = { TextButton(onClick = onDismiss) { Text("Cancel") } }
    )
}
