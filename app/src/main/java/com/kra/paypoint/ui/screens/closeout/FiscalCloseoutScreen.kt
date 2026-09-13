package com.kra.paypoint.ui.screens.closeout

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.shadow
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.kra.paypoint.data.local.entity.ZReportEntity
import com.kra.paypoint.domain.repository.XReportSummary
import com.kra.paypoint.ui.screens.closeout.viewmodel.FiscalCloseoutViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun FiscalCloseoutScreen(
    viewModel: FiscalCloseoutViewModel,
    onNavigateBack: () -> Unit
) {
    val uiState by viewModel.uiState.collectAsState()
    val pastZReports by viewModel.pastZReports.collectAsState()

    var showZCloseoutConfirmation by remember { mutableStateOf(false) }

    // Closeout Confirmation Dialog
    if (showZCloseoutConfirmation) {
        AlertDialog(
            onDismissRequest = { showZCloseoutConfirmation = false },
            icon = {
                Icon(
                    Icons.Default.Warning,
                    contentDescription = null,
                    tint = MaterialTheme.colorScheme.error,
                    modifier = Modifier.size(36.dp)
                )
            },
            title = { Text("Confirm Daily Fiscal Closeout", fontWeight = FontWeight.Bold) },
            text = {
                Column {
                    Text("Are you sure you want to perform the End-of-Day Z-Report closeout?")
                    Spacer(modifier = Modifier.height(8.dp))
                    Text(
                        "This will finalize today's fiscal shift, reset the active sales accumulators, assign a permanent Z-Report number, and print the fiscal audit journal.",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            },
            confirmButton = {
                Button(
                    onClick = {
                        showZCloseoutConfirmation = false
                        viewModel.closeShiftAndPrintZReport()
                    },
                    colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.error)
                ) {
                    Text("Close Fiscal Day")
                }
            },
            dismissButton = {
                TextButton(onClick = { showZCloseoutConfirmation = false }) {
                    Text("Cancel")
                }
            }
        )
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Fiscal Closeout & Z-Report", fontWeight = FontWeight.SemiBold) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                },
                actions = {
                    IconButton(onClick = viewModel::refreshShiftSummary) {
                        Icon(Icons.Default.Refresh, contentDescription = "Refresh Shift")
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
                .padding(16.dp)
        ) {
            // Feedback message banner
            AnimatedVisibility(visible = uiState.feedbackMessage != null) {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 14.dp),
                    colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.primaryContainer)
                ) {
                    Text(
                        text = uiState.feedbackMessage ?: "",
                        style = MaterialTheme.typography.bodyMedium,
                        color = MaterialTheme.colorScheme.onPrimaryContainer,
                        modifier = Modifier.padding(12.dp)
                    )
                }
            }

            // Current Shift Card (Live X-Reading)
            val summary = uiState.shiftSummary ?: XReportSummary(reportDate = "")
            CurrentShiftCard(
                summary = summary,
                isProcessing = uiState.isProcessing,
                onPrintXReport = viewModel::printXReport,
                onInitiateZCloseout = { showZCloseoutConfirmation = true }
            )

            Spacer(modifier = Modifier.height(16.dp))

            Text(
                text = "Z-Report Audit History",
                style = MaterialTheme.typography.titleMedium,
                fontWeight = FontWeight.Bold,
                modifier = Modifier.padding(bottom = 8.dp)
            )

            if (pastZReports.isEmpty()) {
                Box(
                    modifier = Modifier.fillMaxWidth().weight(1f),
                    contentAlignment = Alignment.Center
                ) {
                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                        Icon(
                            Icons.Default.History,
                            contentDescription = null,
                            modifier = Modifier.size(48.dp),
                            tint = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.2f)
                        )
                        Spacer(modifier = Modifier.height(6.dp))
                        Text(
                            "No past Z-Reports generated yet",
                            style = MaterialTheme.typography.bodyMedium,
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                        )
                    }
                }
            } else {
                LazyColumn(
                    modifier = Modifier.fillMaxWidth().weight(1f),
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    items(pastZReports) { report ->
                        PastZReportCard(
                            report = report,
                            onReprint = { viewModel.reprintZReport(report) }
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun CurrentShiftCard(
    summary: XReportSummary,
    isProcessing: Boolean,
    onPrintXReport: () -> Unit,
    onInitiateZCloseout: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(12.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
        elevation = CardDefaults.cardElevation(defaultElevation = 3.dp)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Column {
                    Text(
                        text = "Current Fiscal Shift",
                        style = MaterialTheme.typography.titleMedium,
                        fontWeight = FontWeight.Bold,
                        color = MaterialTheme.colorScheme.primary
                    )
                    Text(
                        text = "Date: ${summary.reportDate} • ${summary.totalTransactionCount} Invoices",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }

                Surface(
                    color = MaterialTheme.colorScheme.primaryContainer,
                    shape = RoundedCornerShape(16.dp)
                ) {
                    Text(
                        text = "SHIFT OPEN",
                        color = MaterialTheme.colorScheme.onPrimaryContainer,
                        fontSize = 11.sp,
                        fontWeight = FontWeight.Bold,
                        modifier = Modifier.padding(horizontal = 8.dp, vertical = 4.dp)
                    )
                }
            }

            Divider(modifier = Modifier.padding(vertical = 10.dp))

            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text("Gross Sales Total:", style = MaterialTheme.typography.bodyMedium)
                Text(
                    String.format("KES %.2f", summary.grossSalesAmount),
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.primary
                )
            }
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text("Total VAT Collected:", style = MaterialTheme.typography.bodySmall)
                Text(String.format("KES %.2f", summary.totalTaxAmount), style = MaterialTheme.typography.bodySmall, fontWeight = FontWeight.SemiBold)
            }
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text("Cash in Drawer:", style = MaterialTheme.typography.bodySmall)
                Text(String.format("KES %.2f", summary.cashAmount), style = MaterialTheme.typography.bodySmall)
            }
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text("Mobile Payments:", style = MaterialTheme.typography.bodySmall)
                Text(String.format("KES %.2f", summary.mobileAmount), style = MaterialTheme.typography.bodySmall)
            }

            Spacer(modifier = Modifier.height(14.dp))

            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.spacedBy(10.dp)
            ) {
                OutlinedButton(
                    onClick = onPrintXReport,
                    enabled = !isProcessing,
                    modifier = Modifier.weight(1f),
                    shape = RoundedCornerShape(8.dp)
                ) {
                    Icon(Icons.Default.Print, contentDescription = null, modifier = Modifier.size(16.dp).padding(end = 4.dp))
                    Text("Print X-Report")
                }

                Button(
                    onClick = onInitiateZCloseout,
                    enabled = !isProcessing,
                    modifier = Modifier.weight(1f),
                    shape = RoundedCornerShape(8.dp),
                    colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.primary)
                ) {
                    Icon(Icons.Default.Lock, contentDescription = null, modifier = Modifier.size(16.dp).padding(end = 4.dp))
                    Text("Z-Closeout")
                }
            }
        }
    }
}

@Composable
fun PastZReportCard(
    report: ZReportEntity,
    onReprint: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(8.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
        elevation = CardDefaults.cardElevation(defaultElevation = 1.dp)
    ) {
        Row(
            modifier = Modifier.fillMaxWidth().padding(12.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = "Z-Report #${report.zReportNumber}",
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold
                )
                Text(
                    text = "Date: ${report.reportDate} • ${report.totalTransactionCount} Invoices",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Text(
                    text = String.format("Gross: KES %.2f (VAT: KES %.2f)", report.grossSalesAmount, report.totalTaxAmount),
                    style = MaterialTheme.typography.bodySmall,
                    fontWeight = FontWeight.SemiBold,
                    color = MaterialTheme.colorScheme.primary
                )
            }

            IconButton(onClick = onReprint) {
                Icon(Icons.Default.Print, contentDescription = "Reprint Z-Report", tint = MaterialTheme.colorScheme.primary)
            }
        }
    }
}
