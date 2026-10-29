package com.kra.paypoint.ui.screens.login.viewmodel

import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.test.UnconfinedTestDispatcher
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock

@OptIn(ExperimentalCoroutinesApi::class)
class LoginViewModelTest {

    private lateinit var authRepository: AuthRepository
    private lateinit var deviceRepository: DeviceRepository
    private lateinit var masterDataRepository: MasterDataRepository
    private lateinit var viewModel: LoginViewModel
    // Unconfined so viewModelScope's two independent init{} coroutines (session restore +
    // setup-state check) and login()'s coroutine all run eagerly to completion, instead of
    // needing scheduler.advanceUntilIdle() to reach into Dispatchers.Main's own separate
    // TestCoroutineScheduler (a different one from runTest{}'s, since this dispatcher is
    // constructed standalone rather than sharing runTest's scheduler).
    private val testDispatcher = UnconfinedTestDispatcher()
    private val userFlow = MutableStateFlow<User?>(null)

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        authRepository = mock(AuthRepository::class.java)
        deviceRepository = mock(DeviceRepository::class.java)
        masterDataRepository = mock(MasterDataRepository::class.java)
        `when`(authRepository.currentUser).thenReturn(userFlow)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `when an operator account already exists, sign-in mode is shown, not setup`() = runTest {
        `when`(authRepository.hasAnyUsers()).thenReturn(true)
        viewModel = LoginViewModel(authRepository, deviceRepository, masterDataRepository)
        testScheduler.advanceUntilIdle()

        val state = viewModel.uiState.value
        assertFalse(state.checkingSetupState)
        assertFalse(state.needsFirstRunSetup)
    }

    @Test
    fun `when no operator account exists yet, first-run setup mode is shown`() = runTest {
        `when`(authRepository.hasAnyUsers()).thenReturn(false)
        viewModel = LoginViewModel(authRepository, deviceRepository, masterDataRepository)
        testScheduler.advanceUntilIdle()

        val state = viewModel.uiState.value
        assertFalse(state.checkingSetupState)
        assertTrue(state.needsFirstRunSetup)
    }

    @Test
    fun `empty credentials sets error message`() = runTest {
        `when`(authRepository.hasAnyUsers()).thenReturn(true)
        viewModel = LoginViewModel(authRepository, deviceRepository, masterDataRepository)
        testScheduler.advanceUntilIdle()

        viewModel.login()
        val state = viewModel.uiState.value
        assertFalse(state.isAuthenticated)
        assertNotNull(state.errorMessage)
    }

    @Test
    fun `successful login updates isAuthenticated to true`() = runTest {
        `when`(authRepository.hasAnyUsers()).thenReturn(true)
        viewModel = LoginViewModel(authRepository, deviceRepository, masterDataRepository)
        testScheduler.advanceUntilIdle()

        val testUser = User(
            id = "CASHIER01",
            name = "Test Cashier",
            authorityCode = "ROLE_CASHIER",
            branchId = "00",
            tin = "P012345678X",
            deviceSerial = "SN123456"
        )
        `when`(authRepository.login("CASHIER01", "pin1234")).thenReturn(Result.success(testUser))

        viewModel.onUsernameChanged("CASHIER01")
        viewModel.onPasswordChanged("pin1234")
        viewModel.login()

        testScheduler.advanceUntilIdle()

        val state = viewModel.uiState.value
        assertTrue(state.isAuthenticated)
        assertEquals(false, state.isLoading)
    }

    private fun newSignInViewModel(): LoginViewModel {
        kotlinx.coroutines.runBlocking { `when`(authRepository.hasAnyUsers()).thenReturn(true) }
        return LoginViewModel(authRepository, deviceRepository, masterDataRepository)
    }

    @Test
    fun `forgot password opens the reset form with the typed user ID`() = runTest {
        viewModel = newSignInViewModel()
        viewModel.onUsernameChanged("cashier1")
        viewModel.startPasswordReset()

        assertEquals("cashier1", viewModel.uiState.value.reset?.username)
        viewModel.cancelPasswordReset()
        assertEquals(null, viewModel.uiState.value.reset)
    }

    @Test
    fun `reset form validates locally before calling the repository`() = runTest {
        viewModel = newSignInViewModel()
        viewModel.startPasswordReset()
        viewModel.updateReset {
            it.copy(username = "cashier1", tin = "P051234567Z", branchId = "00", deviceSerial = "MOBILE001",
                newPassword = "secret1", confirmPassword = "different")
        }
        viewModel.submitPasswordReset()

        assertEquals("Passwords do not match.", viewModel.uiState.value.reset?.errorMessage)
        org.mockito.Mockito.verify(authRepository, org.mockito.Mockito.never())
            .resetForgottenPassword(org.mockito.kotlin.any(), org.mockito.kotlin.any(), org.mockito.kotlin.any(), org.mockito.kotlin.any(), org.mockito.kotlin.any())
    }

    @Test
    fun `successful reset returns to sign-in with the user ID filled in`() = runTest {
        viewModel = newSignInViewModel()
        `when`(authRepository.resetForgottenPassword("cashier1", "P051234567Z", "00", "MOBILE001", "secret1"))
            .thenReturn(Result.success(Unit))
        viewModel.startPasswordReset()
        viewModel.updateReset {
            it.copy(username = "cashier1", tin = "P051234567Z", branchId = "00", deviceSerial = "MOBILE001",
                newPassword = "secret1", confirmPassword = "secret1")
        }
        viewModel.submitPasswordReset()

        val state = viewModel.uiState.value
        assertEquals(null, state.reset)
        assertEquals("cashier1", state.username)
        assertNotNull(state.infoMessage)
    }

    @Test
    fun `failed reset keeps the form open with the repository's message`() = runTest {
        viewModel = newSignInViewModel()
        `when`(authRepository.resetForgottenPassword("cashier1", "P051234567Z", "00", "WRONG", "secret1"))
            .thenReturn(Result.failure(IllegalArgumentException("Those details don't match an active user on this device.")))
        viewModel.startPasswordReset()
        viewModel.updateReset {
            it.copy(username = "cashier1", tin = "P051234567Z", branchId = "00", deviceSerial = "WRONG",
                newPassword = "secret1", confirmPassword = "secret1")
        }
        viewModel.submitPasswordReset()

        val reset = viewModel.uiState.value.reset
        assertEquals("Those details don't match an active user on this device.", reset?.errorMessage)
        assertFalse(reset!!.isSubmitting)
    }
}
