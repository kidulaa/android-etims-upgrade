package com.kra.paypoint.ui.screens.admin

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyListScope
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Search
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp

/**
 * Shared shell for the Admin Menu list screens (users, items, customers): title bar, search,
 * "New" action and a transient message, mirroring the guide's New / Query / Save / Back layout.
 */
@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun AdminListScaffold(
    title: String,
    newLabel: String,
    query: String,
    onQueryChange: (String) -> Unit,
    queryHint: String,
    message: String?,
    onMessageShown: () -> Unit,
    onNew: () -> Unit,
    onNavigateBack: () -> Unit,
    header: @Composable ColumnScope.() -> Unit = {},
    content: LazyListScope.() -> Unit
) {
    val snackbarHostState = remember { SnackbarHostState() }
    LaunchedEffect(message) {
        if (message != null) {
            snackbarHostState.showSnackbar(message)
            onMessageShown()
        }
    }
    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(title, fontWeight = FontWeight.SemiBold) },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) { Icon(Icons.Default.ArrowBack, contentDescription = "Back") }
                },
            )
        },
        floatingActionButton = {
            ExtendedFloatingActionButton(
                onClick = onNew,
                icon = { Icon(Icons.Default.Add, contentDescription = null) },
                text = { Text(newLabel) }
            )
        },
        snackbarHost = { SnackbarHost(snackbarHostState) }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(horizontal = 16.dp)
        ) {
            OutlinedTextField(
                value = query,
                onValueChange = onQueryChange,
                leadingIcon = { Icon(Icons.Default.Search, contentDescription = null) },
                placeholder = { Text(queryHint) },
                singleLine = true,
                modifier = Modifier.fillMaxWidth().padding(vertical = 12.dp)
            )
            header()
            LazyColumn(
                modifier = Modifier.fillMaxSize(),
                verticalArrangement = Arrangement.spacedBy(8.dp),
                contentPadding = PaddingValues(bottom = 88.dp),
                content = content
            )
        }
    }
}

/** Small status pill, e.g. ADMIN / INACTIVE / PENDING KRA. */
@Composable
fun StatusChip(text: String, isWarning: Boolean = false) {
    Surface(
        color = if (isWarning) MaterialTheme.colorScheme.errorContainer else MaterialTheme.colorScheme.secondaryContainer,
        shape = MaterialTheme.shapes.small
    ) {
        Text(
            text,
            style = MaterialTheme.typography.labelSmall,
            color = if (isWarning) MaterialTheme.colorScheme.onErrorContainer else MaterialTheme.colorScheme.onSecondaryContainer,
            modifier = Modifier.padding(horizontal = 6.dp, vertical = 2.dp)
        )
    }
}

/** Confirm-then-save dialog footer, matching the guide's "Do you want to save …? NO / YES". */
@Composable
fun SaveConfirmDialog(what: String, onConfirm: () -> Unit, onDismiss: () -> Unit) {
    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("Confirm") },
        text = { Text("Do you want to save this $what?") },
        confirmButton = { TextButton(onClick = onConfirm) { Text("YES") } },
        dismissButton = { TextButton(onClick = onDismiss) { Text("NO") } }
    )
}
