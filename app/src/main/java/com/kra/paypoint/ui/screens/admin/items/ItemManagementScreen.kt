package com.kra.paypoint.ui.screens.admin.items

import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Inventory2
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import com.kra.paypoint.data.local.entity.ItemClsEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.domain.model.EtimsCodes.TaxType
import com.kra.paypoint.ui.components.CodeDropdown
import com.kra.paypoint.ui.screens.admin.AdminListScaffold
import com.kra.paypoint.ui.screens.admin.SaveConfirmDialog
import com.kra.paypoint.ui.screens.admin.StatusChip
import com.kra.paypoint.ui.screens.admin.items.viewmodel.ItemCodeLists
import com.kra.paypoint.ui.screens.admin.items.viewmodel.ItemForm
import com.kra.paypoint.ui.screens.admin.items.viewmodel.ItemManagementViewModel

@Composable
fun ItemManagementScreen(
    viewModel: ItemManagementViewModel,
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    val items by viewModel.items.collectAsStateWithLifecycle()
    val codeLists by viewModel.codeLists.collectAsStateWithLifecycle()
    val classResults by viewModel.classResults.collectAsStateWithLifecycle()
    var query by remember { mutableStateOf("") }

    val visible = remember(items, query) {
        items.filter {
            query.isBlank() || it.itemName.contains(query, true) || it.itemCode.contains(query, true) ||
                it.barcode?.contains(query) == true
        }
    }

    uiState.form?.let { form ->
        ItemFormDialog(
            form = form,
            codeLists = codeLists,
            classResults = classResults,
            isSaving = uiState.isSaving,
            errorMessage = uiState.errorMessage,
            onClassQuery = viewModel::onClassQueryChanged,
            onSelectClass = viewModel::selectClassification,
            onChange = viewModel::updateForm,
            onSave = viewModel::save,
            onDismiss = viewModel::dismissForm
        )
    }

    AdminListScaffold(
        title = "Item Management",
        newLabel = "New Item",
        query = query,
        onQueryChange = { query = it },
        queryHint = "Search item name, code or barcode",
        message = uiState.message,
        onMessageShown = viewModel::clearMessage,
        onNew = viewModel::startNew,
        onNavigateBack = onNavigateBack
    ) {
        if (visible.isEmpty()) {
            item {
                Text(
                    "No items yet. Tap New Item to register stock you sell, or sync the catalog from eTIMS under Master Data.",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                    modifier = Modifier.padding(8.dp)
                )
            }
        }
        items(visible, key = { it.itemCode }) { item ->
            ItemRow(item = item, onClick = { viewModel.startEdit(item) })
        }
    }
}

@Composable
private fun ItemRow(item: ItemEntity, onClick: () -> Unit) {
    Card(onClick = onClick, modifier = Modifier.fillMaxWidth()) {
        Row(modifier = Modifier.fillMaxWidth().padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
            Icon(Icons.Default.Inventory2, contentDescription = null, tint = MaterialTheme.colorScheme.primary)
            Spacer(Modifier.width(12.dp))
            Column(modifier = Modifier.weight(1f)) {
                Text(item.itemName, fontWeight = FontWeight.Bold)
                Text(
                    "${item.itemCode} • ${TaxType.label(item.taxTypeCode)} • " + if (item.itemTypeCode == ItemForm.ITEM_TYPE_SERVICE) "Service" else "Stock ${"%.2f".format(item.stockQuantity).trimEnd('0').trimEnd('.')}",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Row(horizontalArrangement = Arrangement.spacedBy(6.dp), modifier = Modifier.padding(top = 4.dp)) {
                    if (item.syncStatus != "SYNCED") StatusChip("PENDING KRA", isWarning = true)
                    if (!item.isUsed) StatusChip("DISCONTINUED", isWarning = true)
                }
            }
            Text("KES %,.2f".format(item.defaultUnitPrice), fontWeight = FontWeight.Bold, color = MaterialTheme.colorScheme.primary)
        }
    }
}

@Composable
private fun ItemFormDialog(
    form: ItemForm,
    codeLists: ItemCodeLists,
    classResults: List<ItemClsEntity>,
    isSaving: Boolean,
    errorMessage: String?,
    onClassQuery: (String) -> Unit,
    onSelectClass: (ItemClsEntity) -> Unit,
    onChange: ((ItemForm) -> ItemForm) -> Unit,
    onSave: () -> Unit,
    onDismiss: () -> Unit
) {
    var confirming by remember { mutableStateOf(false) }
    var classSearch by remember { mutableStateOf("") }
    if (confirming) {
        SaveConfirmDialog(what = "item", onConfirm = { confirming = false; onSave() }, onDismiss = { confirming = false })
    }
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text(if (form.isNew) "New Item" else "Edit Item") },
        text = {
            Column(
                modifier = Modifier.verticalScroll(rememberScrollState()),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                OutlinedTextField(
                    value = form.previewItemCode,
                    onValueChange = {},
                    readOnly = true,
                    label = { Text("Item Code") },
                    supportingText = { if (form.isNew) Text("Generated from origin, type and units") },
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = form.barcode,
                    onValueChange = { v -> onChange { it.copy(barcode = v) } },
                    label = { Text("Barcode") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                Row(verticalAlignment = Alignment.CenterVertically) {
                    FilterChip(
                        selected = !form.isService,
                        enabled = form.isNew,
                        onClick = { onChange { it.copy(itemTypeCode = if (it.isService) "2" else it.itemTypeCode) } },
                        label = { Text("Product") }
                    )
                    Spacer(Modifier.width(8.dp))
                    FilterChip(
                        selected = form.isService,
                        enabled = form.isNew,
                        onClick = { onChange { it.copy(itemTypeCode = ItemForm.ITEM_TYPE_SERVICE) } },
                        label = { Text("Service") }
                    )
                }
                OutlinedTextField(
                    value = form.itemName,
                    onValueChange = { v -> onChange { it.copy(itemName = v) } },
                    label = { Text(if (form.isService) "Service Name" else "Product Name") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = form.description,
                    onValueChange = { v -> onChange { it.copy(description = v) } },
                    label = { Text("Description") },
                    minLines = 2,
                    modifier = Modifier.fillMaxWidth()
                )
                Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                    OutlinedTextField(
                        value = form.salePrice,
                        onValueChange = { v -> onChange { it.copy(salePrice = v) } },
                        label = { Text("Sale Price") },
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                        singleLine = true,
                        modifier = Modifier.weight(1f)
                    )
                    if (!form.isService) OutlinedTextField(
                        value = form.beginningStock,
                        onValueChange = { v -> onChange { it.copy(beginningStock = v) } },
                        label = { Text(if (form.isNew) "Beginning Stock" else "Stock") },
                        enabled = form.isNew,
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                        singleLine = true,
                        modifier = Modifier.weight(1f)
                    )
                }
                if (!form.isNew && !form.isService) {
                    Text(
                        "Stock changes after registration go through Inventory (stock in / out / adjust).",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }

                OutlinedTextField(
                    value = classSearch,
                    onValueChange = { classSearch = it; onClassQuery(it) },
                    label = { Text("Find Class (name or code)") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                if (classResults.isNotEmpty()) {
                    Card(modifier = Modifier.fillMaxWidth().heightIn(max = 180.dp)) {
                        Column(modifier = Modifier.verticalScroll(rememberScrollState())) {
                            classResults.forEach { cls ->
                                Text(
                                    "${cls.code} – ${cls.name}",
                                    style = MaterialTheme.typography.bodySmall,
                                    modifier = Modifier
                                        .fillMaxWidth()
                                        .clickable { onSelectClass(cls); classSearch = "" }
                                        .padding(10.dp)
                                )
                            }
                        }
                    }
                } else if (classSearch.length >= 2) {
                    Text(
                        "No synced classifications match — sync Master Data, or type the KRA class code below.",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
                Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                    OutlinedTextField(
                        value = form.classCode,
                        onValueChange = { v -> onChange { it.copy(classCode = v.trim(), className = "") } },
                        label = { Text("Class Code") },
                        singleLine = true,
                        modifier = Modifier.weight(1f)
                    )
                    OutlinedTextField(
                        value = form.className,
                        onValueChange = {},
                        readOnly = true,
                        label = { Text("Class Name") },
                        singleLine = true,
                        modifier = Modifier.weight(1.4f)
                    )
                }

                val codeFieldsLocked = !form.isNew // they're baked into the item code
                CodeDropdown("Origin", codeLists.countries, form.originNationCode, { c -> onChange { it.copy(originNationCode = c) } }, enabled = !codeFieldsLocked)
                CodeDropdown("Item Type", codeLists.itemTypes, form.itemTypeCode, { c -> onChange { it.copy(itemTypeCode = c) } }, enabled = !codeFieldsLocked)
                CodeDropdown("Packaging Unit", codeLists.packagingUnits, form.packagingUnitCode, { c -> onChange { it.copy(packagingUnitCode = c) } }, enabled = !codeFieldsLocked)
                CodeDropdown("Quantity Unit", codeLists.quantityUnits, form.quantityUnitCode, { c -> onChange { it.copy(quantityUnitCode = c) } }, enabled = !codeFieldsLocked)
                CodeDropdown("Tax Type", codeLists.taxTypes, form.taxTypeCode, { c -> onChange { it.copy(taxTypeCode = c) } })

                if (!form.isNew) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Switch(checked = form.isUsed, onCheckedChange = { v -> onChange { it.copy(isUsed = v) } })
                        Spacer(Modifier.width(8.dp))
                        Text(if (form.isUsed) "Active (available for sale)" else "Discontinued")
                    }
                }
                errorMessage?.let { Text(it, color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall) }
            }
        },
        confirmButton = {
            Button(onClick = { confirming = true }, enabled = !isSaving) { Text(if (isSaving) "Saving…" else "Save") }
        },
        dismissButton = { TextButton(onClick = onDismiss) { Text("Back") } }
    )
}
