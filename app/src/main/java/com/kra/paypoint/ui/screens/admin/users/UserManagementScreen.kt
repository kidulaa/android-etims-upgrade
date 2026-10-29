package com.kra.paypoint.ui.screens.admin.users

import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.ui.screens.admin.AdminListScaffold
import com.kra.paypoint.ui.screens.admin.SaveConfirmDialog
import com.kra.paypoint.ui.screens.admin.StatusChip
import com.kra.paypoint.ui.screens.admin.users.viewmodel.UserForm
import com.kra.paypoint.ui.screens.admin.users.viewmodel.UserManagementViewModel

@Composable
fun UserManagementScreen(
    viewModel: UserManagementViewModel,
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    val users by viewModel.users.collectAsStateWithLifecycle()
    val currentUser by viewModel.currentUser.collectAsStateWithLifecycle()
    var query by remember { mutableStateOf("") }

    val visible = remember(users, query) {
        users.filter { query.isBlank() || it.id.contains(query, true) || it.name.contains(query, true) }
    }

    uiState.form?.let { form ->
        UserFormDialog(
            form = form,
            isSaving = uiState.isSaving,
            errorMessage = uiState.errorMessage,
            onChange = viewModel::updateForm,
            onSave = viewModel::save,
            onDismiss = viewModel::dismissForm
        )
    }

    AdminListScaffold(
        title = "User Management",
        newLabel = "New User",
        query = query,
        onQueryChange = { query = it },
        queryHint = "Search user code or name",
        message = uiState.message,
        onMessageShown = viewModel::clearMessage,
        onNew = viewModel::startNew,
        onNavigateBack = onNavigateBack
    ) {
        items(visible, key = { it.id }) { user ->
            UserRow(
                user = user,
                isSelf = user.id == currentUser?.id,
                onEdit = { viewModel.startEdit(user) },
                onToggleActive = { viewModel.setActive(user, !user.isActive) }
            )
        }
    }
}

@Composable
private fun UserRow(user: User, isSelf: Boolean, onEdit: () -> Unit, onToggleActive: () -> Unit) {
    Card(onClick = onEdit, modifier = Modifier.fillMaxWidth()) {
        Row(
            modifier = Modifier.fillMaxWidth().padding(12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Icon(Icons.Default.Person, contentDescription = null, tint = MaterialTheme.colorScheme.primary)
            Spacer(Modifier.width(12.dp))
            Column(modifier = Modifier.weight(1f)) {
                Text(user.name + if (isSelf) " (you)" else "", fontWeight = FontWeight.Bold)
                Text(
                    "Code ${user.id}${user.telNo?.let { " • $it" }.orEmpty()}",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Row(horizontalArrangement = Arrangement.spacedBy(6.dp), modifier = Modifier.padding(top = 4.dp)) {
                    StatusChip(if (user.isAdmin) "ADMIN" else "USER")
                    if (!user.isActive) StatusChip("INACTIVE", isWarning = true)
                }
            }
            if (!isSelf) {
                TextButton(onClick = onToggleActive) { Text(if (user.isActive) "Deactivate" else "Activate") }
            }
        }
    }
}

@Composable
private fun UserFormDialog(
    form: UserForm,
    isSaving: Boolean,
    errorMessage: String?,
    onChange: ((UserForm) -> UserForm) -> Unit,
    onSave: () -> Unit,
    onDismiss: () -> Unit
) {
    val isNew = form.editingUserId == null
    var confirming by remember { mutableStateOf(false) }
    if (confirming) {
        SaveConfirmDialog(
            what = "user",
            onConfirm = { confirming = false; onSave() },
            onDismiss = { confirming = false }
        )
    }
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text(if (isNew) "New User" else "Edit User") },
        text = {
            Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
                OutlinedTextField(
                    value = form.userCode,
                    onValueChange = { v -> onChange { it.copy(userCode = v.trim()) } },
                    label = { Text("User Code") },
                    enabled = isNew,
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = form.userName,
                    onValueChange = { v -> onChange { it.copy(userName = v) } },
                    label = { Text("User Name") },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = form.password,
                    onValueChange = { v -> onChange { it.copy(password = v) } },
                    label = { Text(if (isNew) "Password (min 6)" else "New password (leave blank to keep)") },
                    visualTransformation = PasswordVisualTransformation(),
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Password),
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = form.telNo,
                    onValueChange = { v -> onChange { it.copy(telNo = v) } },
                    label = { Text("Tel No") },
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone),
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth()
                )
                Text("Permission", style = MaterialTheme.typography.labelLarge)
                Row(verticalAlignment = Alignment.CenterVertically) {
                    RadioButton(selected = !form.isAdmin, onClick = { onChange { it.copy(isAdmin = false) } })
                    Text("User (POS only)")
                    Spacer(Modifier.width(12.dp))
                    RadioButton(selected = form.isAdmin, onClick = { onChange { it.copy(isAdmin = true) } })
                    Text("Admin")
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
