package com.kra.paypoint.ui.screens.dashboard

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.shadow
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.kra.paypoint.ui.screens.dashboard.viewmodel.DashboardViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DashboardScreen(
    viewModel: DashboardViewModel,
    onNavigateToSales: () -> Unit,
    onNavigateToMasterData: () -> Unit,
    onNavigateToReceipts: () -> Unit = {},
    onNavigateToHardware: () -> Unit = {},
    onNavigateToCloseout: () -> Unit = {},
    onNavigateToInventory: () -> Unit = {},
    onNavigateToCreditNote: () -> Unit = {},
    onLogout: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    val pendingCount by viewModel.pendingSyncCount.collectAsState()

    Scaffold(
        topBar = {
            TopAppBar(
                title = {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Text("eTIMS PayPoint", fontWeight = FontWeight.Bold)
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primary,
                    titleContentColor = MaterialTheme.colorScheme.onPrimary
                ),
                actions = {
                    TextButton(
                        onClick = {
                            viewModel.logout()
                            onLogout()
                        }
                    ) {
                        Icon(
                            imageVector = Icons.Default.ExitToApp,
                            contentDescription = "Logout",
                            tint = MaterialTheme.colorScheme.onPrimary,
                            modifier = Modifier.padding(end = 4.dp)
                        )
                        Text("Logout", color = MaterialTheme.colorScheme.onPrimary)
                    }
                }
            )
        }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(16.dp)
        ) {
            // Operator Header Card
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 16.dp),
                shape = RoundedCornerShape(12.dp),
                colors = CardDefaults.cardColors(
                    containerColor = MaterialTheme.colorScheme.primaryContainer.copy(alpha = 0.4f)
                )
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(16.dp),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Column {
                        Text(
                            text = "Logged In Cashier",
                            style = MaterialTheme.typography.bodySmall,
                            color = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                        Text(
                            text = uiState.operatorName,
                            style = MaterialTheme.typography.titleMedium,
                            fontWeight = FontWeight.Bold
                        )
                    }

                    Surface(
                        color = MaterialTheme.colorScheme.primary,
                        shape = CircleShape,
                        modifier = Modifier.size(40.dp)
                    ) {
                        Box(contentAlignment = Alignment.Center) {
                            Icon(
                                imageVector = Icons.Default.Person,
                                contentDescription = null,
                                tint = MaterialTheme.colorScheme.onPrimary
                            )
                        }
                    }
                }
            }

            // Offline Sync Status Banner
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 20.dp),
                shape = RoundedCornerShape(12.dp),
                colors = CardDefaults.cardColors(
                    containerColor = if (pendingCount > 0)
                        MaterialTheme.colorScheme.tertiaryContainer.copy(alpha = 0.5f)
                    else
                        MaterialTheme.colorScheme.surfaceVariant.copy(alpha = 0.6f)
                )
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(14.dp),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        modifier = Modifier.weight(1f)
                    ) {
                        Icon(
                            imageVector = if (pendingCount > 0) Icons.Default.CloudOff else Icons.Default.CloudDone,
                            contentDescription = null,
                            tint = if (pendingCount > 0) MaterialTheme.colorScheme.error else MaterialTheme.colorScheme.primary,
                            modifier = Modifier.padding(end = 12.dp)
                        )
                        Column {
                            Text(
                                text = if (pendingCount > 0)
                                    "$pendingCount Offline Invoices Pending"
                                else
                                    "All Invoices Synced with KRA",
                                style = MaterialTheme.typography.bodyMedium,
                                fontWeight = FontWeight.SemiBold
                            )
                            Text(
                                text = if (pendingCount > 0)
                                    "Will sync automatically when connected"
                                else
                                    "Background queue idle",
                                style = MaterialTheme.typography.bodySmall,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                        }
                    }

                    Button(
                        onClick = viewModel::triggerSync,
                        enabled = !uiState.isSyncing,
                        shape = RoundedCornerShape(8.dp),
                        contentPadding = PaddingValues(horizontal = 12.dp, vertical = 6.dp)
                    ) {
                        if (uiState.isSyncing) {
                            CircularProgressIndicator(
                                modifier = Modifier.size(16.dp),
                                color = MaterialTheme.colorScheme.onPrimary,
                                strokeWidth = 2.dp
                            )
                        } else {
                            Icon(
                                imageVector = Icons.Default.Sync,
                                contentDescription = "Sync",
                                modifier = Modifier.size(16.dp).padding(end = 4.dp)
                            )
                            Text("Sync", fontSize = 13.sp)
                        }
                    }
                }
            }

            AnimatedVisibility(visible = uiState.syncMessage != null) {
                Text(
                    text = uiState.syncMessage ?: "",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.primary,
                    modifier = Modifier.padding(bottom = 12.dp)
                )
            }

            Text(
                text = "Quick Actions",
                style = MaterialTheme.typography.titleMedium,
                fontWeight = FontWeight.Bold,
                modifier = Modifier.padding(bottom = 12.dp)
            )

            // Grid Actions
            LazyVerticalGrid(
                columns = GridCells.Fixed(2),
                horizontalArrangement = Arrangement.spacedBy(16.dp),
                verticalArrangement = Arrangement.spacedBy(16.dp),
                modifier = Modifier.fillMaxWidth()
            ) {
                item {
                    DashboardActionCard(
                        title = "New Sale",
                        subtitle = "Issue Fiscal Receipt",
                        icon = Icons.Default.ShoppingCart,
                        onClick = onNavigateToSales
                    )
                }
                item {
                    DashboardActionCard(
                        title = "Master Data",
                        subtitle = "Items & Customers",
                        icon = Icons.Default.Storage,
                        onClick = onNavigateToMasterData
                    )
                }
                item {
                    DashboardActionCard(
                        title = "Receipts",
                        subtitle = "History & Auditing",
                        icon = Icons.Default.ReceiptLong,
                        badge = if (pendingCount > 0) "$pendingCount" else null,
                        onClick = onNavigateToReceipts
                    )
                }
                item {
                    DashboardActionCard(
                        title = "Hardware",
                        subtitle = "Printers & Peripherals",
                        icon = Icons.Default.Print,
                        onClick = onNavigateToHardware
                    )
                }
                item {
                    DashboardActionCard(
                        title = "Z-Closeout",
                        subtitle = "Fiscal Reconciliation",
                        icon = Icons.Default.PointOfSale,
                        onClick = onNavigateToCloseout
                    )
                }
                item {
                    DashboardActionCard(
                        title = "Inventory",
                        subtitle = "Stock In, Out & Adjust",
                        icon = Icons.Default.Inventory,
                        onClick = onNavigateToInventory
                    )
                }
                item {
                    DashboardActionCard(
                        title = "Credit Note",
                        subtitle = "Refunds & Reversals",
                        icon = Icons.Default.RemoveCircleOutline,
                        onClick = onNavigateToCreditNote
                    )
                }
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DashboardActionCard(
    title: String,
    subtitle: String,
    icon: ImageVector,
    badge: String? = null,
    onClick: () -> Unit
) {
    Card(
        onClick = onClick,
        modifier = Modifier
            .fillMaxWidth()
            .height(130.dp)
            .shadow(4.dp, RoundedCornerShape(12.dp)),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surface
        )
    ) {
        Box(modifier = Modifier.fillMaxSize().padding(16.dp)) {
            if (badge != null) {
                Surface(
                    color = MaterialTheme.colorScheme.error,
                    shape = CircleShape,
                    modifier = Modifier
                        .size(24.dp)
                        .align(Alignment.TopEnd)
                ) {
                    Box(contentAlignment = Alignment.Center) {
                        Text(
                            text = badge,
                            color = MaterialTheme.colorScheme.onError,
                            fontSize = 11.sp,
                            fontWeight = FontWeight.Bold
                        )
                    }
                }
            }

            Column(
                modifier = Modifier.fillMaxSize(),
                verticalArrangement = Arrangement.Center,
                horizontalAlignment = Alignment.Start
            ) {
                Icon(
                    imageVector = icon,
                    contentDescription = null,
                    tint = MaterialTheme.colorScheme.primary,
                    modifier = Modifier.size(28.dp).padding(bottom = 6.dp)
                )
                Text(
                    text = title,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.onSurface
                )
                Spacer(modifier = Modifier.height(2.dp))
                Text(
                    text = subtitle,
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
        }
    }
}
