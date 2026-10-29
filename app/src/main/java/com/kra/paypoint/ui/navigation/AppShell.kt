package com.kra.paypoint.ui.navigation

import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.WindowInsets
import androidx.compose.foundation.layout.consumeWindowInsets
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material.icons.filled.MoreHoriz
import androidx.compose.material.icons.filled.PointOfSale
import androidx.compose.material.icons.filled.ReceiptLong
import androidx.compose.material.icons.outlined.Home
import androidx.compose.material.icons.outlined.MoreHoriz
import androidx.compose.material.icons.outlined.PointOfSale
import androidx.compose.material.icons.outlined.ReceiptLong
import androidx.compose.material3.Icon
import androidx.compose.material3.NavigationBar
import androidx.compose.material3.NavigationBarItem
import androidx.compose.material3.NavigationRail
import androidx.compose.material3.NavigationRailItem
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.unit.dp

/**
 * The app's top-level sections. "Sell" opens the full-screen sales flow (no bar, so the
 * cart's own Payment bar has the whole bottom edge); the others are tabs.
 */
enum class TopLevelDestination(
    val route: String,
    val label: String,
    val icon: ImageVector,
    val selectedIcon: ImageVector,
    val showsBar: Boolean = true
) {
    HOME("dashboard", "Home", Icons.Outlined.Home, Icons.Filled.Home),
    SELL("sales", "Sell", Icons.Outlined.PointOfSale, Icons.Filled.PointOfSale, showsBar = false),
    RECEIPTS("receipts", "Receipts", Icons.Outlined.ReceiptLong, Icons.Filled.ReceiptLong),
    MORE("more", "More", Icons.Outlined.MoreHoriz, Icons.Filled.MoreHoriz);

    companion object {
        /** Matches a nav route pattern such as "sales?fromDocument={fromDocument}". */
        fun of(route: String?): TopLevelDestination? =
            route?.substringBefore('?')?.let { base -> entries.firstOrNull { it.route == base } }
    }
}

/**
 * Bottom navigation bar on phones, navigation rail from 600dp (counter tablets, landscape).
 * The [content] (the NavHost) always sits in the same slot, so showing or hiding the bar never
 * recreates navigation state.
 */
@Composable
fun PayPointAppShell(
    currentRoute: String?,
    onNavigate: (TopLevelDestination) -> Unit,
    content: @Composable () -> Unit
) {
    val current = TopLevelDestination.of(currentRoute)
    val showNavigation = current?.showsBar == true

    BoxWithConstraints(modifier = Modifier.fillMaxSize()) {
        val wide = maxWidth >= 600.dp
        Scaffold(
            // The bar/rail handle their own system-bar insets; screens handle the rest.
            contentWindowInsets = WindowInsets(0, 0, 0, 0),
            bottomBar = {
                if (showNavigation && !wide) {
                    NavigationBar {
                        TopLevelDestination.entries.forEach { dest ->
                            NavigationBarItem(
                                selected = dest == current,
                                onClick = { onNavigate(dest) },
                                icon = { Icon(if (dest == current) dest.selectedIcon else dest.icon, contentDescription = null) },
                                label = { Text(dest.label) }
                            )
                        }
                    }
                }
            }
        ) { padding ->
            Row(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(padding)
                    .consumeWindowInsets(padding)
            ) {
                if (showNavigation && wide) {
                    NavigationRail {
                        TopLevelDestination.entries.forEach { dest ->
                            NavigationRailItem(
                                selected = dest == current,
                                onClick = { onNavigate(dest) },
                                icon = { Icon(if (dest == current) dest.selectedIcon else dest.icon, contentDescription = null) },
                                label = { Text(dest.label) }
                            )
                        }
                    }
                }
                Box(modifier = Modifier.weight(1f)) { content() }
            }
        }
    }
}
