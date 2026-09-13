package com.kra.paypoint.ui.screens.dashboard.viewmodel

import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.TransactionRepository
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.launch
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.advanceUntilIdle
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify

@OptIn(ExperimentalCoroutinesApi::class)
class DashboardViewModelTest {

    private lateinit var authRepository: AuthRepository
    private lateinit var transactionRepository: TransactionRepository
    private lateinit var viewModel: DashboardViewModel
    private val testDispatcher = StandardTestDispatcher()
    private val userFlow = MutableStateFlow<User?>(null)

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        authRepository = mock(AuthRepository::class.java)
        transactionRepository = mock(TransactionRepository::class.java)

        `when`(authRepository.currentUser).thenReturn(userFlow)
        `when`(transactionRepository.getPendingCount()).thenReturn(flowOf(3))

        viewModel = DashboardViewModel(authRepository, transactionRepository)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `pendingSyncCount reflects transactionRepository flow`() = runTest {
        // stateIn(WhileSubscribed) only starts producing once collected — see the same note
        // in InventoryViewModelTest.
        backgroundScope.launch { viewModel.pendingSyncCount.collect {} }
        testScheduler.advanceUntilIdle()
        assertEquals(3, viewModel.pendingSyncCount.value)
    }

    @Test
    fun `triggerSync triggers transaction repository sync`() = runTest {
        viewModel.triggerSync()
        testScheduler.advanceUntilIdle()
        verify(transactionRepository).triggerSync()
        assertNotNull(viewModel.uiState.value.syncMessage)
    }
}
