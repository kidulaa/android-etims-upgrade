package com.kra.paypoint

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import com.kra.paypoint.domain.repository.PreferencesRepository
import javax.inject.Inject
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.kra.paypoint.ui.screens.closeout.FiscalCloseoutScreen
import com.kra.paypoint.ui.screens.closeout.viewmodel.FiscalCloseoutViewModel
import com.kra.paypoint.ui.screens.creditnote.CreditNoteScreen
import com.kra.paypoint.ui.screens.creditnote.viewmodel.CreditNoteViewModel
import com.kra.paypoint.ui.screens.dashboard.DashboardScreen
import com.kra.paypoint.ui.screens.dashboard.viewmodel.DashboardViewModel
import com.kra.paypoint.ui.screens.hardware.HardwareSettingsScreen
import com.kra.paypoint.ui.screens.hardware.viewmodel.HardwareSettingsViewModel
import com.kra.paypoint.ui.screens.inventory.InventoryScreen
import com.kra.paypoint.ui.screens.inventory.viewmodel.InventoryViewModel
import com.kra.paypoint.ui.screens.login.LoginScreen
import com.kra.paypoint.ui.screens.login.viewmodel.LoginViewModel
import com.kra.paypoint.ui.screens.master.MasterDataScreen
import com.kra.paypoint.ui.screens.master.viewmodel.MasterDataViewModel
import com.kra.paypoint.ui.screens.onboarding.OnboardingScreen
import com.kra.paypoint.ui.screens.onboarding.viewmodel.OnboardingViewModel
import com.kra.paypoint.ui.screens.receipts.ReceiptsScreen
import com.kra.paypoint.ui.screens.receipts.viewmodel.ReceiptsViewModel
import com.kra.paypoint.ui.screens.sales.SalesScreen
import com.kra.paypoint.ui.screens.sales.viewmodel.SalesViewModel
import com.kra.paypoint.ui.screens.systemsetting.SystemSettingsScreen
import com.kra.paypoint.ui.screens.systemsetting.viewmodel.SystemSettingsViewModel
import com.kra.paypoint.ui.theme.PayPointTheme
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class MainActivity : ComponentActivity() {

    @Inject
    lateinit var preferencesRepository: PreferencesRepository

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            PayPointTheme {
                val hasSeenOnboarding by preferencesRepository.hasSeenOnboarding.collectAsState()
                
                if (hasSeenOnboarding == null) {
                    // Loading state while DataStore is resolved
                    return@PayPointTheme
                }

                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background
                ) {
                    PayPointApp(hasSeenOnboarding = hasSeenOnboarding!!)
                }
            }
        }
    }
}

@Composable
fun PayPointApp(hasSeenOnboarding: Boolean) {
    val navController = rememberNavController()

    val startDestination = if (hasSeenOnboarding) "login" else "onboarding"

    // "login" also serves as the splash/session-resolution gate: LoginScreen shows a
    // spinner while AuthRepository restores (or fails to restore) a saved session, then
    // routes to sign-in, first-run setup, or straight through to the dashboard.
    NavHost(navController = navController, startDestination = startDestination) {
        composable("onboarding") {
            val onboardingViewModel: OnboardingViewModel = hiltViewModel()
            OnboardingScreen(
                viewModel = onboardingViewModel,
                onNavigateToLogin = {
                    navController.navigate("login") {
                        popUpTo("onboarding") { inclusive = true }
                    }
                }
            )
        }
        composable("login") {
            val loginViewModel: LoginViewModel = hiltViewModel()
            LoginScreen(
                viewModel = loginViewModel,
                onNavigateToDashboard = {
                    navController.navigate("dashboard") {
                        popUpTo("login") { inclusive = true }
                    }
                }
            )
        }
        composable("dashboard") {
            val dashboardViewModel: DashboardViewModel = hiltViewModel()
            DashboardScreen(
                viewModel = dashboardViewModel,
                onNavigateToSales = { navController.navigate("sales") },
                onNavigateToMasterData = { navController.navigate("masterData") },
                onNavigateToReceipts = { navController.navigate("receipts") },
                onNavigateToHardware = { navController.navigate("hardware") },
                onNavigateToCloseout = { navController.navigate("closeout") },
                onNavigateToInventory = { navController.navigate("inventory") },
                onNavigateToCreditNote = { navController.navigate("creditNote") },
                onNavigateToSystemSetting = { navController.navigate("systemSetting") },
                onLogout = {
                    navController.navigate("login") {
                        popUpTo("dashboard") { inclusive = true }
                    }
                }
            )
        }
        composable("sales") {
            val salesViewModel: SalesViewModel = hiltViewModel()
            SalesScreen(
                viewModel = salesViewModel,
                onNavigateBack = { navController.popBackStack() }
            )
        }
        composable("masterData") {
            val masterDataViewModel: MasterDataViewModel = hiltViewModel()
            MasterDataScreen(
                viewModel = masterDataViewModel,
                onNavigateBack = { navController.popBackStack() }
            )
        }
        composable("receipts") {
            val receiptsViewModel: ReceiptsViewModel = hiltViewModel()
            ReceiptsScreen(
                viewModel = receiptsViewModel,
                onNavigateBack = { navController.popBackStack() },
                onIssueRefund = { invcNo -> navController.navigate("creditNote?invoiceNumber=$invcNo") }
            )
        }
        composable("hardware") {
            val hardwareViewModel: HardwareSettingsViewModel = hiltViewModel()
            HardwareSettingsScreen(
                viewModel = hardwareViewModel,
                onNavigateBack = { navController.popBackStack() }
            )
        }
        composable("closeout") {
            val closeoutViewModel: FiscalCloseoutViewModel = hiltViewModel()
            FiscalCloseoutScreen(
                viewModel = closeoutViewModel,
                onNavigateBack = { navController.popBackStack() }
            )
        }
        composable("inventory") {
            val inventoryViewModel: InventoryViewModel = hiltViewModel()
            InventoryScreen(
                viewModel = inventoryViewModel,
                onNavigateBack = { navController.popBackStack() }
            )
        }
        composable(
            route = "creditNote?invoiceNumber={invoiceNumber}",
            arguments = listOf(
                navArgument("invoiceNumber") {
                    type = NavType.LongType
                    defaultValue = 0L
                }
            )
        ) { backStackEntry ->
            val invoiceNumber = backStackEntry.arguments?.getLong("invoiceNumber")?.takeIf { it > 0L }
            val creditNoteViewModel: CreditNoteViewModel = hiltViewModel()
            CreditNoteScreen(
                viewModel = creditNoteViewModel,
                initialInvoiceNumber = invoiceNumber,
                onNavigateBack = { navController.popBackStack() }
            )
        }
        composable("systemSetting") {
            val systemSettingsViewModel: SystemSettingsViewModel = hiltViewModel()
            SystemSettingsScreen(
                viewModel = systemSettingsViewModel,
                onNavigateBack = { navController.popBackStack() }
            )
        }
    }
}
