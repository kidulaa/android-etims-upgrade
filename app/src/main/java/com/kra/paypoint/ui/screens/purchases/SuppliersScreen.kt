package com.kra.paypoint.ui.screens.purchases

import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.LocalShipping
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardCapitalization
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import com.kra.paypoint.ui.screens.admin.AdminListScaffold
import com.kra.paypoint.ui.screens.admin.StatusChip
import com.kra.paypoint.ui.screens.purchases.viewmodel.SuppliersViewModel

@Composable
fun SuppliersScreen(viewModel: SuppliersViewModel, onNavigateBack: () -> Unit) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    val suppliers by viewModel.suppliers.collectAsStateWithLifecycle()
    var query by remember { mutableStateOf("") }
    val visible = remember(suppliers, query) {
        suppliers.filter { query.isBlank() || it.supplierName.contains(query, true) || it.supplierTin.contains(query, true) }
    }

    state.form?.let { form ->
        AlertDialog(
            onDismissRequest = viewModel::dismiss,
            title = { Text(if (form.isNew) "Add Supplier" else "Edit Supplier") },
            text = {
                Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    OutlinedTextField(
                        value = form.pin,
                        onValueChange = { v -> viewModel.updateForm { it.copy(pin = v.uppercase().trim()) } },
                        label = { Text("PIN of Supplier") },
                        enabled = form.isNew,
                        keyboardOptions = KeyboardOptions(capitalization = KeyboardCapitalization.Characters),
                        singleLine = true,
                        modifier = Modifier.fillMaxWidth()
                    )
                    OutlinedTextField(
                        value = form.name,
                        onValueChange = { v -> viewModel.updateForm { it.copy(name = v) } },
                        label = { Text("Supplier Name") },
                        singleLine = true,
                        modifier = Modifier.fillMaxWidth()
                    )
                    OutlinedTextField(
                        value = form.phone,
                        onValueChange = { v -> viewModel.updateForm { it.copy(phone = v) } },
                        label = { Text("Phone Number") },
                        placeholder = { Text("254…") },
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone),
                        singleLine = true,
                        modifier = Modifier.fillMaxWidth()
                    )
                    OutlinedTextField(
                        value = form.email,
                        onValueChange = { v -> viewModel.updateForm { it.copy(email = v) } },
                        label = { Text("Email (optional)") },
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                        singleLine = true,
                        modifier = Modifier.fillMaxWidth()
                    )
                    if (!form.isNew) {
                        Row(verticalAlignment = Alignment.CenterVertically) {
                            Switch(checked = form.isUsed, onCheckedChange = { v -> viewModel.updateForm { it.copy(isUsed = v) } })
                            Spacer(Modifier.width(8.dp))
                            Text(if (form.isUsed) "Active" else "Inactive")
                        }
                    }
                    state.errorMessage?.let { Text(it, color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall) }
                }
            },
            confirmButton = { Button(onClick = viewModel::save, enabled = !state.isSaving) { Text(if (state.isSaving) "Saving…" else "Add") } },
            dismissButton = { TextButton(onClick = viewModel::dismiss) { Text("Back") } }
        )
    }

    AdminListScaffold(
        title = "Suppliers",
        newLabel = "Add Supplier",
        query = query,
        onQueryChange = { query = it },
        queryHint = "Search PIN or name",
        message = state.message,
        onMessageShown = viewModel::messageShown,
        onNew = viewModel::startNew,
        onNavigateBack = onNavigateBack
    ) {
        items(visible, key = { it.supplierTin }) { s ->
            Card(onClick = { viewModel.startEdit(s) }, modifier = Modifier.fillMaxWidth()) {
                Row(Modifier.padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
                    Icon(Icons.Default.LocalShipping, contentDescription = null, tint = MaterialTheme.colorScheme.primary)
                    Spacer(Modifier.width(12.dp))
                    Column(Modifier.weight(1f)) {
                        Text(s.supplierName, fontWeight = FontWeight.Bold)
                        Text(s.supplierTin + (s.telNo?.let { " • $it" } ?: ""), style = MaterialTheme.typography.bodySmall)
                    }
                    if (!s.isUsed) StatusChip("INACTIVE", isWarning = true)
                }
            }
        }
    }
}
