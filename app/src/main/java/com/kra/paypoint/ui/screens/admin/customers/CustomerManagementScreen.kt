package com.kra.paypoint.ui.screens.admin.customers

import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Business
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardCapitalization
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.domain.model.EtimsCodes
import com.kra.paypoint.domain.usecase.MasterDataValidator
import com.kra.paypoint.ui.components.CodeDropdown
import com.kra.paypoint.ui.screens.admin.AdminListScaffold
import com.kra.paypoint.ui.screens.admin.SaveConfirmDialog
import com.kra.paypoint.ui.screens.admin.StatusChip
import com.kra.paypoint.ui.screens.admin.customers.viewmodel.CustomerForm
import com.kra.paypoint.ui.screens.admin.customers.viewmodel.CustomerManagementViewModel

private val customerTypes = listOf(
    EtimsCodes.Code(MasterDataValidator.CUSTOMER_INDIVIDUAL, "Individual"),
    EtimsCodes.Code(MasterDataValidator.CUSTOMER_CORPORATE, "Corporate (non-individual)")
)

@Composable
fun CustomerManagementScreen(
    viewModel: CustomerManagementViewModel,
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    val customers by viewModel.customers.collectAsStateWithLifecycle()
    var query by remember { mutableStateOf("") }

    val visible = remember(customers, query) {
        customers.filter {
            query.isBlank() || it.customerName.contains(query, true) || it.customerTin.contains(query, true) ||
                it.telNo?.contains(query) == true
        }
    }

    uiState.form?.let { form ->
        CustomerFormDialog(
            form = form,
            isSaving = uiState.isSaving,
            errorMessage = uiState.errorMessage,
            onChange = viewModel::updateForm,
            onSave = viewModel::save,
            onDismiss = viewModel::dismissForm
        )
    }

    AdminListScaffold(
        title = "Customer Management",
        newLabel = "New Customer",
        query = query,
        onQueryChange = { query = it },
        queryHint = "Search PIN, name or phone",
        message = uiState.message,
        onMessageShown = viewModel::clearMessage,
        onNew = viewModel::startNew,
        onNavigateBack = onNavigateBack
    ) {
        items(visible, key = { it.customerTin }) { customer ->
            CustomerRow(customer = customer, onClick = { viewModel.startEdit(customer) })
        }
    }
}

@Composable
private fun CustomerRow(customer: CustomerEntity, onClick: () -> Unit) {
    val isCorporate = customer.customerType == MasterDataValidator.CUSTOMER_CORPORATE
    Card(onClick = onClick, modifier = Modifier.fillMaxWidth()) {
        Row(modifier = Modifier.fillMaxWidth().padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
            Icon(
                if (isCorporate) Icons.Default.Business else Icons.Default.Person,
                contentDescription = null,
                tint = MaterialTheme.colorScheme.primary
            )
            Spacer(Modifier.width(12.dp))
            Column(modifier = Modifier.weight(1f)) {
                Text(customer.customerName, fontWeight = FontWeight.Bold)
                Text(
                    customer.customerTin + listOfNotNull(customer.telNo, customer.email).joinToString("") { " • $it" },
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Row(horizontalArrangement = Arrangement.spacedBy(6.dp), modifier = Modifier.padding(top = 4.dp)) {
                    StatusChip(if (isCorporate) "CORPORATE" else "INDIVIDUAL")
                    if (customer.syncStatus != "SYNCED") StatusChip("PENDING KRA", isWarning = true)
                    if (!customer.isUsed) StatusChip("INACTIVE", isWarning = true)
                }
            }
        }
    }
}

@Composable
private fun CustomerFormDialog(
    form: CustomerForm,
    isSaving: Boolean,
    errorMessage: String?,
    onChange: ((CustomerForm) -> CustomerForm) -> Unit,
    onSave: () -> Unit,
    onDismiss: () -> Unit
) {
    var confirming by remember { mutableStateOf(false) }
    if (confirming) {
        SaveConfirmDialog(what = "customer account", onConfirm = { confirming = false; onSave() }, onDismiss = { confirming = false })
    }
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text(if (form.isNew) "New Customer" else "Edit Customer") },
        text = {
            Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                CodeDropdown(
                    label = "Type",
                    options = customerTypes,
                    selectedCode = form.customerType,
                    onSelect = { c -> onChange { it.copy(customerType = c) } }
                )
                OutlinedTextField(
                    value = form.pin,
                    onValueChange = { v -> onChange { it.copy(pin = v.uppercase().trim()) } },
                    label = { Text("PIN Code") },
                    enabled = form.isNew,
                    supportingText = {
                        Text(
                            if (form.customerType == MasterDataValidator.CUSTOMER_CORPORATE) "Non-individual PINs start with P"
                            else "Individual PINs start with A"
                        )
                    },
                    keyboardOptions = KeyboardOptions(capitalization = KeyboardCapitalization.Characters),
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = form.name,
                    onValueChange = { v -> onChange { it.copy(name = v) } },
                    label = { Text("Customer Name") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = form.phone,
                    onValueChange = { v -> onChange { it.copy(phone = v) } },
                    label = { Text("Phone Number") },
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone),
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = form.email,
                    onValueChange = { v -> onChange { it.copy(email = v) } },
                    label = { Text("Email") },
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                if (!form.isNew) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Switch(checked = form.isUsed, onCheckedChange = { v -> onChange { it.copy(isUsed = v) } })
                        Spacer(Modifier.width(8.dp))
                        Text(if (form.isUsed) "Active" else "Inactive")
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
