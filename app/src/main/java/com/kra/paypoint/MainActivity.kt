package com.kra.paypoint

import androidx.lifecycle.compose.collectAsStateWithLifecycle
import android.animation.ValueAnimator
import androidx.compose.animation.core.tween
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.animation.slideInHorizontally
import androidx.compose.animation.slideOutHorizontally
import androidx.navigation.NavHostController
import androidx.navigation.compose.currentBackStackEntryAsState
import com.kra.paypoint.ui.navigation.PayPointAppShell
import com.kra.paypoint.ui.navigation.TopLevelDestination
import com.kra.paypoint.ui.screens.more.MoreScreen
import android.os.Bundle
import android.os.SystemClock
import android.view.animation.AccelerateInterpolator
import androidx.compose.runtime.remember
import androidx.core.splashscreen.SplashScreen.Companion.installSplashScreen
import androidx.core.splashscreen.SplashScreenViewProvider
import com.kra.paypoint.domain.repository.AuthRepository
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.runtime.Composable
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
import com.kra.paypoint.ui.screens.admin.customers.CustomerManagementScreen
import com.kra.paypoint.ui.screens.admin.customers.viewmodel.CustomerManagementViewModel
import com.kra.paypoint.ui.screens.admin.items.ItemManagementScreen
import com.kra.paypoint.ui.screens.admin.items.viewmodel.ItemManagementViewModel
import com.kra.paypoint.ui.screens.admin.users.UserManagementScreen
import com.kra.paypoint.ui.screens.admin.users.viewmodel.UserManagementViewModel
import com.kra.paypoint.ui.screens.closeout.FiscalCloseoutScreen
import com.kra.paypoint.ui.screens.documents.DocumentEditorScreen
import com.kra.paypoint.ui.screens.documents.DocumentsMode
import com.kra.paypoint.ui.screens.documents.DocumentsScreen
import com.kra.paypoint.ui.screens.documents.viewmodel.DocumentEditorViewModel
import com.kra.paypoint.ui.screens.documents.viewmodel.DocumentsViewModel
import com.kra.paypoint.ui.screens.documents.viewmodel.EditorKind
import com.kra.paypoint.ui.screens.purchases.SuppliersScreen
import com.kra.paypoint.ui.screens.purchases.viewmodel.SuppliersViewModel
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

    @Inject
    lateinit var authRepository: AuthRepository

    override fun onCreate(savedInstanceState: Bundle?) {
        // Must run before super.onCreate so the splash theme hands over to Theme.PayPoint.
        val splash = installSplashScreen()
        super.onCreate(savedInstanceState)

        // Hold the splash until we know where to go — onboarding flag read and any saved session
        // restored — instead of showing a blank frame and then a login-screen spinner. Capped so
        // a stalled read can never trap the user on the splash.
        val launchedAt = SystemClock.uptimeMillis()
        splash.setKeepOnScreenCondition {
            !isStartupResolved() && SystemClock.uptimeMillis() - launchedAt < MAX_SPLASH_MILLIS
        }
        splash.setOnExitAnimationListener(::animateSplashExit)

        enableEdgeToEdge()
        setContent {
            PayPointTheme {
                val hasSeenOnboarding by preferencesRepository.hasSeenOnboarding.collectAsStateWithLifecycle()
                val sessionResolved by authRepository.isSessionResolved.collectAsStateWithLifecycle()

                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background
                ) {
                    val onboarded = hasSeenOnboarding ?: return@Surface
                    if (!sessionResolved) return@Surface
                    // Decided once: the nav graph must not be rebuilt when sign-in state changes later.
                    val startDestination = remember {
                        when {
                            !onboarded -> "onboarding"
                            authRepository.isAuthenticated() -> "dashboard"
                            else -> "login"
                        }
                    }
                    PayPointApp(startDestination = startDestination)
                }
            }
        }
    }

    private fun isStartupResolved(): Boolean =
        preferencesRepository.hasSeenOnboarding.value != null && authRepository.isSessionResolved.value

    /** Short fade with a slight zoom of the logo; skipped when system animations are off. */
    private fun animateSplashExit(provider: SplashScreenViewProvider) {
        val view = provider.view
        if (!ValueAnimator.areAnimatorsEnabled()) {
            provider.remove()
            return
        }
        val iconView = provider.iconView
        view.animate()
            .alpha(0f)
            .setDuration(SPLASH_EXIT_MILLIS)
            .setInterpolator(AccelerateInterpolator())
            .withEndAction { provider.remove() }
            .start()
        iconView.animate()
            .scaleX(1.15f)
            .scaleY(1.15f)
            .setDuration(SPLASH_EXIT_MILLIS)
            .setInterpolator(AccelerateInterpolator())
            .start()
    }

    private companion object {
        const val MAX_SPLASH_MILLIS = 4_000L
        const val SPLASH_EXIT_MILLIS = 300L
    }
}

@Composable
fun PayPointApp(startDestination: String) {
    val navController = rememberNavController()
    val backStackEntry by navController.currentBackStackEntryAsState()

    PayPointAppShell(
        currentRoute = backStackEntry?.destination?.route,
        onNavigate = { dest ->
            if (dest == TopLevelDestination.SELL) navController.navigate(dest.route)
            else navController.navigateToTab(dest)
        }
    ) {
        PayPointNavHost(navController, startDestination)
    }
}

/**
 * Switches tabs the standard way: one copy of each tab on the stack, Home as the root, and each
 * tab's scroll position / state saved and restored when you come back to it.
 */
private fun NavHostController.navigateToTab(dest: TopLevelDestination) {
    navigate(dest.route) {
        popUpTo(TopLevelDestination.HOME.route) { saveState = true }
        launchSingleTop = true
        restoreState = true
    }
}

@Composable
private fun PayPointNavHost(navController: NavHostController, startDestination: String) {
    // Startup routing happens behind the splash (MainActivity): a restored session opens on the
    // dashboard directly. "login" still resolves first-run setup vs. sign-in itself.
    NavHost(
        navController = navController,
        startDestination = startDestination,
        // Short fade + slight slide: feels responsive on a busy till without getting in the way.
        enterTransition = { fadeIn(tween(220)) + slideInHorizontally(tween(220)) { it / 12 } },
        exitTransition = { fadeOut(tween(160)) },
        popEnterTransition = { fadeIn(tween(220)) + slideInHorizontally(tween(220)) { -it / 12 } },
        popExitTransition = { fadeOut(tween(160)) + slideOutHorizontally(tween(220)) { it / 12 } }
    ) {
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
                onNewSale = { navController.navigate("sales") },
                onNewQuotation = { navController.navigate("docEditor?kind=${EditorKind.QUOTATION.name}") },
                onCreditNote = { navController.navigate("creditNote") },
                onCloseout = { navController.navigate("closeout") },
                onOpenReceipts = { navController.navigateToTab(TopLevelDestination.RECEIPTS) },
                onManageStock = { navController.navigate("inventory") }
            )
        }
        composable("more") {
            val dashboardViewModel: DashboardViewModel = hiltViewModel()
            MoreScreen(
                viewModel = dashboardViewModel,
                onNavigate = { route -> navController.navigate(route) },
                onLoggedOut = {
                    // Clear everything so Back can't return to a signed-in screen.
                    navController.navigate("login") { popUpTo(navController.graph.id) { inclusive = true } }
                }
            )
        }
        composable(
            route = "sales?fromDocument={fromDocument}",
            arguments = listOf(navArgument("fromDocument") { type = NavType.LongType; defaultValue = 0L })
        ) { entry ->
            val salesViewModel: SalesViewModel = hiltViewModel()
            SalesScreen(
                viewModel = salesViewModel,
                onNavigateBack = { navController.popBackStack() },
                fromDocumentId = entry.arguments?.getLong("fromDocument")?.takeIf { it > 0L }
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
        composable("users") {
            val viewModel: UserManagementViewModel = hiltViewModel()
            UserManagementScreen(viewModel = viewModel, onNavigateBack = { navController.popBackStack() })
        }
        composable("items") {
            val viewModel: ItemManagementViewModel = hiltViewModel()
            ItemManagementScreen(viewModel = viewModel, onNavigateBack = { navController.popBackStack() })
        }
        composable("customers") {
            val viewModel: CustomerManagementViewModel = hiltViewModel()
            CustomerManagementScreen(viewModel = viewModel, onNavigateBack = { navController.popBackStack() })
        }
        composable(
            route = "documents/{mode}",
            arguments = listOf(navArgument("mode") { type = NavType.StringType })
        ) { entry ->
            val mode = DocumentsMode.valueOf(entry.arguments?.getString("mode") ?: DocumentsMode.SALES.name)
            val viewModel: DocumentsViewModel = hiltViewModel()
            DocumentsScreen(
                viewModel = viewModel,
                mode = mode,
                onCreate = { kind -> navController.navigate("docEditor?kind=${kind.name}") },
                onEdit = { kind, id -> navController.navigate("docEditor?kind=${kind.name}&id=$id") },
                onCreateInvoiceFrom = { id -> navController.navigate("sales?fromDocument=$id") },
                onCreatePurchaseFromLpo = { id -> navController.navigate("docEditor?kind=${EditorKind.PURCHASE_INVOICE.name}&fromLpo=$id") },
                onNavigateBack = { navController.popBackStack() }
            )
        }
        composable(
            route = "docEditor?kind={kind}&id={id}&fromLpo={fromLpo}",
            arguments = listOf(
                navArgument(DocumentEditorViewModel.ARG_KIND) { type = NavType.StringType; defaultValue = EditorKind.QUOTATION.name },
                navArgument(DocumentEditorViewModel.ARG_ID) { type = NavType.LongType; defaultValue = 0L },
                navArgument(DocumentEditorViewModel.ARG_FROM_LPO) { type = NavType.LongType; defaultValue = 0L }
            )
        ) {
            val viewModel: DocumentEditorViewModel = hiltViewModel()
            DocumentEditorScreen(
                viewModel = viewModel,
                onDone = { navController.popBackStack() },
                onNavigateBack = { navController.popBackStack() }
            )
        }
        composable("suppliers") {
            val viewModel: SuppliersViewModel = hiltViewModel()
            SuppliersScreen(viewModel = viewModel, onNavigateBack = { navController.popBackStack() })
        }
    }
}
