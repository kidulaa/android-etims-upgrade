package com.kra.paypoint.ui.screens.master.viewmodel

import com.kra.paypoint.data.local.dao.CustomerDao
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.launch
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock
import org.mockito.kotlin.any

@OptIn(ExperimentalCoroutinesApi::class)
class MasterDataViewModelTest {

    private lateinit var itemDao: ItemDao
    private lateinit var customerDao: CustomerDao
    private lateinit var masterDataRepository: MasterDataRepository
    private lateinit var authRepository: AuthRepository
    private lateinit var viewModel: MasterDataViewModel
    private val testDispatcher = StandardTestDispatcher()

    private val testUser = User(
        id = "ADMIN01", name = "Admin", authorityCode = "ROLE_ADMIN",
        branchId = "00", tin = "P012345678X"
    )

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        itemDao = mock(ItemDao::class.java)
        customerDao = mock(CustomerDao::class.java)
        masterDataRepository = mock(MasterDataRepository::class.java)
        authRepository = mock(AuthRepository::class.java)

        `when`(itemDao.getItemCount()).thenReturn(flowOf(12))
        `when`(customerDao.getCustomerCount()).thenReturn(flowOf(4))
        `when`(authRepository.currentUser).thenReturn(MutableStateFlow(testUser))

        viewModel = MasterDataViewModel(itemDao, customerDao, masterDataRepository, authRepository)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `counts reflect dao counts`() = runTest {
        // stateIn(WhileSubscribed) only starts producing once collected.
        backgroundScope.launch { viewModel.itemCount.collect {} }
        backgroundScope.launch { viewModel.customerCount.collect {} }
        testScheduler.advanceUntilIdle()
        assertEquals(12, viewModel.itemCount.value)
        assertEquals(4, viewModel.customerCount.value)
    }

    @Test
    fun `syncItems pulls the real catalog from eTIMS, not fabricated data`() = runTest {
        `when`(masterDataRepository.syncItemsFromEtims(any(), any())).thenReturn(Result.success(5))

        viewModel.syncItems()
        testScheduler.advanceUntilIdle()

        assertEquals(false, viewModel.uiState.value.isSyncingItems)
        assertTrue(viewModel.uiState.value.statusMessage?.contains("5") == true)
    }

    @Test
    fun `syncItems reports failure instead of falling back to placeholder data`() = runTest {
        `when`(masterDataRepository.syncItemsFromEtims(any(), any()))
            .thenReturn(Result.failure(IllegalStateException("no connectivity")))

        viewModel.syncItems()
        testScheduler.advanceUntilIdle()

        assertNotNull(viewModel.uiState.value.statusMessage)
        assertTrue(viewModel.uiState.value.statusMessage!!.contains("failed"))
    }
}
