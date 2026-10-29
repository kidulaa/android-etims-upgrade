package com.kra.paypoint.ui.screens.more

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyListScope
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ExitToApp
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.semantics.heading
import androidx.compose.ui.semantics.semantics
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.kra.paypoint.ui.screens.dashboard.viewmodel.DashboardViewModel

/** One row on the More screen; [route] is a nav route from MainActivity. */
private data class MoreEntry(val title: String, val subtitle: String, val icon: ImageVector, val route: String)

private val salesEntries = listOf(
    MoreEntry("Quotations & Proforma", "Non-fiscal quotes you can turn into invoices", Icons.Default.Description, "documents/SALES"),
    MoreEntry("Credit Notes", "Full or partial refunds", Icons.Default.Replay, "creditNote"),
    MoreEntry("X / Z Reports", "Shift reading and daily closeout", Icons.Default.Summarize, "closeout")
)

private val adminEntries = listOf(
    MoreEntry("Items", "Products & services registered with KRA", Icons.Default.Category, "items"),
    MoreEntry("Inventory", "Stock in, out and adjustments", Icons.Default.Inventory2, "inventory"),
    MoreEntry("Customers", "Buyers and their KRA PINs", Icons.Default.People, "customers"),
    MoreEntry("Purchases", "LPOs and supplier invoices", Icons.Default.ShoppingBasket, "documents/PURCHASES"),
    MoreEntry("Suppliers", "Who you buy from", Icons.Default.LocalShipping, "suppliers"),
    MoreEntry("Users", "Cashier and admin accounts", Icons.Default.ManageAccounts, "users"),
    MoreEntry("Master Data", "Sync items, customers and codes from eTIMS", Icons.Default.Sync, "masterData")
)

private val settingsEntries = listOf(
    MoreEntry("Business & Receipt", "Contact details and non-VAT mode", Icons.Default.Storefront, "systemSetting"),
    MoreEntry("Device & Printer", "eTIMS device registration and printer", Icons.Default.Print, "hardware")
)

@Composable
fun MoreScreen(
    viewModel: DashboardViewModel,
    onNavigate: (String) -> Unit,
    onLoggedOut: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    var confirmLogout by remember { mutableStateOf(false) }

    if (confirmLogout) {
        AlertDialog(
            onDismissRequest = { confirmLogout = false },
            icon = { Icon(Icons.AutoMirrored.Filled.ExitToApp, contentDescription = null) },
            title = { Text("Sign out?") },
            text = { Text("Unsynced invoices stay on this device and send once someone signs in again.") },
            confirmButton = {
                TextButton(onClick = {
                    confirmLogout = false
                    viewModel.logout()
                    onLoggedOut()
                }) { Text("Sign out") }
            },
            dismissButton = { TextButton(onClick = { confirmLogout = false }) { Text("Cancel") } }
        )
    }

    LazyColumn(
        modifier = Modifier.fillMaxSize().windowInsetsPadding(WindowInsets.statusBars),
        contentPadding = PaddingValues(bottom = 24.dp)
    ) {
        item {
            Text(
                "More",
                style = MaterialTheme.typography.headlineSmall,
                modifier = Modifier.padding(start = 16.dp, top = 16.dp, bottom = 8.dp).semantics { heading() }
            )
        }
        item {
            ListItem(
                leadingContent = {
                    Surface(shape = CircleShape, color = MaterialTheme.colorScheme.primaryContainer, modifier = Modifier.size(48.dp)) {
                        Box(contentAlignment = Alignment.Center) {
                            Text(
                                uiState.operatorName.firstOrNull()?.uppercase() ?: "?",
                                style = MaterialTheme.typography.titleLarge,
                                color = MaterialTheme.colorScheme.onPrimaryContainer
                            )
                        }
                    }
                },
                headlineContent = { Text(uiState.operatorName, style = MaterialTheme.typography.titleMedium) },
                supportingContent = { Text(if (uiState.isAdmin) "Admin • full access" else "Cashier • sales and receipts") }
            )
        }

        section("Sales", salesEntries, onNavigate)
        if (uiState.isAdmin) {
            section("Admin", adminEntries, onNavigate)
            section("Settings", settingsEntries, onNavigate)
        }

        item {
            HorizontalDivider(modifier = Modifier.padding(vertical = 8.dp))
            ListItem(
                modifier = Modifier.clickable { confirmLogout = true },
                leadingContent = { Icon(Icons.AutoMirrored.Filled.ExitToApp, contentDescription = null, tint = MaterialTheme.colorScheme.error) },
                headlineContent = { Text("Sign out", color = MaterialTheme.colorScheme.error) }
            )
        }
    }
}

private fun LazyListScope.section(title: String, entries: List<MoreEntry>, onNavigate: (String) -> Unit) {
    item(key = "header-$title") {
        Text(
            title,
            style = MaterialTheme.typography.titleSmall,
            color = MaterialTheme.colorScheme.primary,
            modifier = Modifier.padding(start = 16.dp, top = 16.dp, bottom = 4.dp).semantics { heading() }
        )
    }
    entries.forEach { entry ->
        item(key = entry.route) {
            ListItem(
                modifier = Modifier.clickable { onNavigate(entry.route) },
                leadingContent = { Icon(entry.icon, contentDescription = null, tint = MaterialTheme.colorScheme.onSurfaceVariant) },
                headlineContent = { Text(entry.title) },
                supportingContent = { Text(entry.subtitle) },
                trailingContent = { Icon(Icons.Default.ChevronRight, contentDescription = null) }
            )
        }
    }
}
