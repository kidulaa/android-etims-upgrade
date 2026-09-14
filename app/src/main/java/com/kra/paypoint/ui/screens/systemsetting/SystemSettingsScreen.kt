package com.kra.paypoint.ui.screens.systemsetting

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Save
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import com.kra.paypoint.ui.screens.systemsetting.viewmodel.SystemSettingsViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SystemSettingsScreen(
    viewModel: SystemSettingsViewModel,
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    val scrollState = rememberScrollState()

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("System Settings", fontWeight = FontWeight.SemiBold) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                },
                actions = {
                    IconButton(onClick = viewModel::saveSettings) {
                        Icon(Icons.Default.Save, contentDescription = "Save Settings")
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primary,
                    titleContentColor = MaterialTheme.colorScheme.onPrimary,
                    navigationIconContentColor = MaterialTheme.colorScheme.onPrimary,
                    actionIconContentColor = MaterialTheme.colorScheme.onPrimary
                )
            )
        }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .verticalScroll(scrollState)
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            if (uiState.saveSuccess) {
                Card(
                    colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.primaryContainer),
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Text(
                        text = "Settings saved successfully",
                        modifier = Modifier.padding(16.dp),
                        color = MaterialTheme.colorScheme.onPrimaryContainer
                    )
                }
            }

            Text("Device Information (Read-Only)", style = MaterialTheme.typography.titleMedium, color = MaterialTheme.colorScheme.primary)
            
            uiState.deviceRegistration?.let { reg ->
                OutlinedTextField(
                    value = reg.taxprNm.orEmpty(),
                    onValueChange = {},
                    label = { Text("Company Name") },
                    readOnly = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = reg.bsnsActv.orEmpty(),
                    onValueChange = {},
                    label = { Text("President Name / Business Activity") },
                    readOnly = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = reg.bhfNm.orEmpty(),
                    onValueChange = {},
                    label = { Text("Branch Name") },
                    readOnly = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = reg.mgrNm.orEmpty(),
                    onValueChange = {},
                    label = { Text("Manager Name") },
                    readOnly = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = reg.tin,
                    onValueChange = {},
                    label = { Text("TIN Number") },
                    readOnly = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = reg.sdcId,
                    onValueChange = {},
                    label = { Text("SDC ID") },
                    readOnly = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = reg.branchId,
                    onValueChange = {},
                    label = { Text("Branch Code") },
                    readOnly = true,
                    modifier = Modifier.fillMaxWidth()
                )
                OutlinedTextField(
                    value = reg.mrcNo,
                    onValueChange = {},
                    label = { Text("MRC No") },
                    readOnly = true,
                    modifier = Modifier.fillMaxWidth()
                )
            } ?: Text("Device not yet initialized.", color = MaterialTheme.colorScheme.error)

            Spacer(modifier = Modifier.height(8.dp))
            Text("Editable Settings", style = MaterialTheme.typography.titleMedium, color = MaterialTheme.colorScheme.primary)

            OutlinedTextField(
                value = uiState.email,
                onValueChange = viewModel::updateEmail,
                label = { Text("Email Address") },
                modifier = Modifier.fillMaxWidth()
            )
            OutlinedTextField(
                value = uiState.phone,
                onValueChange = viewModel::updatePhone,
                label = { Text("Phone Number") },
                modifier = Modifier.fillMaxWidth()
            )
            OutlinedTextField(
                value = uiState.address,
                onValueChange = viewModel::updateAddress,
                label = { Text("Address / Location") },
                modifier = Modifier.fillMaxWidth()
            )

            Row(
                modifier = Modifier.fillMaxWidth(),
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Text("Non-VAT Mode", style = MaterialTheme.typography.bodyLarge)
                Switch(
                    checked = uiState.isNonVat,
                    onCheckedChange = viewModel::updateNonVat
                )
            }

            Button(
                onClick = viewModel::saveSettings,
                modifier = Modifier.fillMaxWidth().padding(top = 16.dp),
                enabled = !uiState.isSaving
            ) {
                if (uiState.isSaving) {
                    CircularProgressIndicator(modifier = Modifier.size(24.dp), color = MaterialTheme.colorScheme.onPrimary)
                } else {
                    Text("Save Settings")
                }
            }
        }
    }
}
