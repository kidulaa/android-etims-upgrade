package com.kra.paypoint.ui.screens.dashboard

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.semantics.heading
import androidx.compose.ui.semantics.semantics
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.isCreditNote
import com.kra.paypoint.ui.screens.dashboard.viewmodel.DashboardViewModel
import com.kra.paypoint.ui.screens.dashboard.viewmodel.HomeSnapshot
import com.kra.paypoint.ui.theme.PayPointTheme
import java.time.Instant
import java.time.LocalDate
import java.time.LocalTime
import java.time.ZoneId
import java.time.format.DateTimeFormatter

/**
 * Home: today's business at a glance, one big way to start a sale, and the things that need
 * attention (unsynced invoices, low stock). Everything else lives under Receipts / More.
 */
@Composable
fun DashboardScreen(
    viewModel: DashboardViewModel,
    onNewSale: () -> Unit,
    onNewQuotation: () -> Unit,
    onCreditNote: () -> Unit,
    onCloseout: () -> Unit,
    onOpenReceipts: () -> Unit,
    onManageStock: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    val home by viewModel.home.collectAsStateWithLifecycle()

    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .windowInsetsPadding(WindowInsets.statusBars),
        contentPadding = PaddingValues(start = 16.dp, end = 16.dp, top = 12.dp, bottom = 24.dp),
        verticalArrangement = Arrangement.spacedBy(16.dp)
    ) {
        item { Greeting(name = uiState.operatorName, isAdmin = uiState.isAdmin) }

        item { TodayCard(home) }

        item {
            Button(
                onClick = onNewSale,
                modifier = Modifier.fillMaxWidth().height(60.dp),
                shape = MaterialTheme.shapes.large
            ) {
                Icon(Icons.Default.AddShoppingCart, contentDescription = null)
                Spacer(Modifier.width(10.dp))
                Text("New Sale", style = MaterialTheme.typography.titleMedium)
            }
        }

        item {
            Row(horizontalArrangement = Arrangement.spacedBy(10.dp)) {
                QuickAction("Quotation", Icons.Default.Description, onNewQuotation, Modifier.weight(1f))
                QuickAction("Credit Note", Icons.Default.Replay, onCreditNote, Modifier.weight(1f))
                QuickAction("X / Z Report", Icons.Default.Summarize, onCloseout, Modifier.weight(1f))
            }
        }

        item {
            SyncCard(
                unsynced = home.unsynced,
                isSyncing = uiState.isSyncing,
                message = uiState.syncMessage,
                onSync = viewModel::triggerSync
            )
        }

        if (uiState.isAdmin && home.lowStock.isNotEmpty()) {
            item { LowStockCard(home.lowStock, onManageStock) }
        }

        item {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    "Recent receipts",
                    style = MaterialTheme.typography.titleMedium,
                    modifier = Modifier.semantics { heading() }
                )
                TextButton(onClick = onOpenReceipts) { Text("See all") }
            }
        }

        if (home.recent.isEmpty()) {
            item {
                Text(
                    "No sales yet. Tap New Sale to issue your first receipt.",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                    modifier = Modifier.padding(vertical = 8.dp)
                )
            }
        } else {
            items(home.recent, key = { it.id }) { tx -> RecentReceiptRow(tx, onClick = onOpenReceipts) }
        }
    }
}

@Composable
private fun Greeting(name: String, isAdmin: Boolean) {
    val hour = remember { LocalTime.now().hour }
    val greeting = when (hour) {
        in 5..11 -> "Good morning"
        in 12..16 -> "Good afternoon"
        else -> "Good evening"
    }
    val date = remember { LocalDate.now().format(DateTimeFormatter.ofPattern("EEEE, d MMMM")) }
    Row(verticalAlignment = Alignment.CenterVertically) {
        Column(modifier = Modifier.weight(1f)) {
            Text(date, style = MaterialTheme.typography.labelLarge, color = MaterialTheme.colorScheme.onSurfaceVariant)
            Text(
                "$greeting, ${name.substringBefore(' ')}",
                style = MaterialTheme.typography.headlineSmall,
                maxLines = 1,
                overflow = TextOverflow.Ellipsis,
                modifier = Modifier.semantics { heading() }
            )
        }
        Surface(shape = CircleShape, color = MaterialTheme.colorScheme.primaryContainer, modifier = Modifier.size(48.dp)) {
            Box(contentAlignment = Alignment.Center) {
                Text(
                    name.firstOrNull()?.uppercase() ?: "?",
                    style = MaterialTheme.typography.titleLarge,
                    color = MaterialTheme.colorScheme.onPrimaryContainer
                )
            }
        }
    }
    Spacer(Modifier.height(4.dp))
    AssistChip(
        onClick = {},
        label = { Text(if (isAdmin) "Admin" else "Cashier") },
        leadingIcon = { Icon(Icons.Default.Badge, contentDescription = null, modifier = Modifier.size(18.dp)) }
    )
}

@Composable
private fun TodayCard(home: HomeSnapshot) {
    Card(
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.primary,
            contentColor = MaterialTheme.colorScheme.onPrimary
        ),
        shape = MaterialTheme.shapes.extraLarge,
        modifier = Modifier.fillMaxWidth()
    ) {
        Column(modifier = Modifier.padding(20.dp)) {
            Text("Today's sales", style = MaterialTheme.typography.labelLarge)
            Text(
                "KES %,.2f".format(home.salesToday),
                style = MaterialTheme.typography.displaySmall,
                fontWeight = FontWeight.Bold,
                maxLines = 1
            )
            Spacer(Modifier.height(12.dp))
            Row(horizontalArrangement = Arrangement.spacedBy(24.dp)) {
                Metric("Invoices", home.invoicesToday.toString())
                Metric("VAT", "%,.2f".format(home.vatToday))
                if (home.refundsToday > 0) Metric("Refunds", home.refundsToday.toString())
            }
        }
    }
}

@Composable
private fun Metric(label: String, value: String) {
    Column {
        Text(value, style = MaterialTheme.typography.titleLarge)
        Text(label, style = MaterialTheme.typography.labelMedium)
    }
}

@Composable
private fun QuickAction(label: String, icon: ImageVector, onClick: () -> Unit, modifier: Modifier) {
    FilledTonalButton(
        onClick = onClick,
        modifier = modifier.heightIn(min = 72.dp),
        shape = MaterialTheme.shapes.large,
        contentPadding = PaddingValues(8.dp)
    ) {
        Column(horizontalAlignment = Alignment.CenterHorizontally) {
            Icon(icon, contentDescription = null)
            Spacer(Modifier.height(4.dp))
            Text(label, style = MaterialTheme.typography.labelMedium, maxLines = 1, overflow = TextOverflow.Ellipsis)
        }
    }
}

@Composable
private fun SyncCard(unsynced: Int, isSyncing: Boolean, message: String?, onSync: () -> Unit) {
    val pending = unsynced > 0
    val status = PayPointTheme.status
    Card(
        colors = CardDefaults.cardColors(
            containerColor = if (pending) status.warningContainer else status.successContainer,
            contentColor = if (pending) status.onWarningContainer else status.onSuccessContainer
        ),
        shape = MaterialTheme.shapes.large,
        modifier = Modifier.fillMaxWidth()
    ) {
        Row(modifier = Modifier.padding(16.dp), verticalAlignment = Alignment.CenterVertically) {
            Icon(if (pending) Icons.Default.CloudUpload else Icons.Default.CloudDone, contentDescription = null)
            Spacer(Modifier.width(12.dp))
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    if (pending) "$unsynced invoice(s) waiting for KRA" else "All invoices filed with KRA",
                    style = MaterialTheme.typography.titleSmall
                )
                Text(
                    message ?: if (pending) "They send automatically when online." else "Nothing waiting to sync.",
                    style = MaterialTheme.typography.bodySmall
                )
            }
            AnimatedVisibility(visible = pending || isSyncing) {
                FilledTonalButton(onClick = onSync, enabled = !isSyncing) {
                    if (isSyncing) CircularProgressIndicator(modifier = Modifier.size(16.dp), strokeWidth = 2.dp)
                    else Text("Sync now")
                }
            }
        }
    }
}

@Composable
private fun LowStockCard(items: List<ItemEntity>, onManage: () -> Unit) {
    OutlinedCard(shape = MaterialTheme.shapes.large, modifier = Modifier.fillMaxWidth()) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                Icon(Icons.Default.Inventory2, contentDescription = null, tint = MaterialTheme.colorScheme.error)
                Spacer(Modifier.width(10.dp))
                Text("Low stock (${items.size})", style = MaterialTheme.typography.titleSmall, modifier = Modifier.weight(1f))
                TextButton(onClick = onManage) { Text("Manage") }
            }
            items.take(3).forEach { item ->
                Row(Modifier.fillMaxWidth().padding(vertical = 2.dp)) {
                    Text(item.itemName, style = MaterialTheme.typography.bodyMedium, modifier = Modifier.weight(1f), maxLines = 1, overflow = TextOverflow.Ellipsis)
                    Text(
                        "${qty(item.stockQuantity)} left",
                        style = MaterialTheme.typography.bodyMedium,
                        color = if (item.stockQuantity <= 0) MaterialTheme.colorScheme.error else MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            }
        }
    }
}

@Composable
private fun RecentReceiptRow(tx: TransactionEntity, onClick: () -> Unit) {
    ListItem(
        modifier = Modifier
            .clip(RoundedCornerShape(16.dp))
            .clickable(onClick = onClick),
        colors = ListItemDefaults.colors(containerColor = MaterialTheme.colorScheme.surfaceContainerLow),
        leadingContent = {
            Surface(
                shape = CircleShape,
                color = if (tx.isCreditNote) MaterialTheme.colorScheme.errorContainer else MaterialTheme.colorScheme.secondaryContainer,
                modifier = Modifier.size(40.dp)
            ) {
                Box(contentAlignment = Alignment.Center) {
                    Icon(
                        if (tx.isCreditNote) Icons.Default.Replay else Icons.Default.ReceiptLong,
                        contentDescription = if (tx.isCreditNote) "Credit note" else "Sale",
                        modifier = Modifier.size(20.dp)
                    )
                }
            }
        },
        headlineContent = { Text(tx.customerName ?: "Walk-in Customer", maxLines = 1, overflow = TextOverflow.Ellipsis) },
        supportingContent = { Text("#${tx.invoiceNumber} • ${timeLabel(tx.createdAt)}") },
        trailingContent = {
            Column(horizontalAlignment = Alignment.End) {
                Text(
                    (if (tx.isCreditNote) "−" else "") + "%,.2f".format(tx.totalAmount),
                    style = MaterialTheme.typography.titleSmall
                )
                Text(
                    if (tx.syncStatus == "SYNCED") "Filed" else "Pending",
                    style = MaterialTheme.typography.labelSmall,
                    color = if (tx.syncStatus == "SYNCED") PayPointTheme.status.success else PayPointTheme.status.warning
                )
            }
        }
    )
}

private fun qty(v: Double) = if (v % 1.0 == 0.0) v.toLong().toString() else "%.2f".format(v)

private fun timeLabel(epochMillis: Long): String {
    val dt = Instant.ofEpochMilli(epochMillis).atZone(ZoneId.systemDefault())
    return if (dt.toLocalDate() == LocalDate.now()) dt.format(DateTimeFormatter.ofPattern("HH:mm"))
    else dt.format(DateTimeFormatter.ofPattern("d MMM"))
}
