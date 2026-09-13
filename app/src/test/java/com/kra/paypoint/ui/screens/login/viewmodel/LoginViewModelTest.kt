package com.kra.paypoint.ui.screens.login.viewmodel

import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.test.StandardTestDispatcher
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
    private lateinit var viewModel: LoginViewModel
    private val testDispatcher = StandardTestDispatcher()
    private val userFlow = MutableStateFlow<User?>(null)

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        authRepository = mock(AuthRepository::class.java)
        `when`(authRepository.currentUser).thenReturn(userFlow)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `when an operator account already exists, sign-in mode is shown, not setup`() = runTest {
        `when`(authRepository.hasAnyUsers()).thenReturn(true)
        viewModel = LoginViewModel(authRepository)
        testScheduler.advanceUntilIdle()

        val state = viewModel.uiState.value
        assertFalse(state.checkingSetupState)
        assertFalse(state.needsFirstRunSetup)
    }

    @Test
    fun `when no operator account exists yet, first-run setup mode is shown`() = runTest {
        `when`(authRepository.hasAnyUsers()).thenReturn(false)
        viewModel = LoginViewModel(authRepository)
        testScheduler.advanceUntilIdle()

        val state = viewModel.uiState.value
        assertFalse(state.checkingSetupState)
        assertTrue(state.needsFirstRunSetup)
    }

    @Test
    fun `empty credentials sets error message`() = runTest {
        `when`(authRepository.hasAnyUsers()).thenReturn(true)
        viewModel = LoginViewModel(authRepository)
        testScheduler.advanceUntilIdle()

        viewModel.login()
        val state = viewModel.uiState.value
        assertFalse(state.isAuthenticated)
        assertNotNull(state.errorMessage)
    }

    @Test
    fun `successful login updates isAuthenticated to true`() = runTest {
        `when`(authRepository.hasAnyUsers()).thenReturn(true)
        viewModel = LoginViewModel(authRepository)
        testScheduler.advanceUntilIdle()

        val testUser = User(
            id = "CASHIER01",
            name = "Test Cashier",
            authorityCode = "ROLE_CASHIER",
            branchId = "00",
            tin = "P012345678X"
        )
        `when`(authRepository.login("CASHIER01", "pin1234")).thenAnswer {
            userFlow.value = testUser
            Result.success(testUser)
        }

        viewModel.onUsernameChanged("CASHIER01")
        viewModel.onPasswordChanged("pin1234")
        viewModel.login()

        testScheduler.advanceUntilIdle()

        val state = viewModel.uiState.value
        assertTrue(state.isAuthenticated)
        assertEquals(false, state.isLoading)
    }
}
