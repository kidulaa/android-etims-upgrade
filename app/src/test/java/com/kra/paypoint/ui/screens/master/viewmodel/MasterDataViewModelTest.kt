package com.kra.paypoint.ui.screens.master.viewmodel

import com.kra.paypoint.data.local.dao.CustomerDao
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.remote.api.MasterDataService
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.flowOf
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
import org.mockito.Mockito.anyList
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify

@OptIn(ExperimentalCoroutinesApi::class)
class MasterDataViewModelTest {

    private lateinit var itemDao: ItemDao
    private lateinit var customerDao: CustomerDao
    private lateinit var masterDataService: MasterDataService
    private lateinit var viewModel: MasterDataViewModel
    private val testDispatcher = StandardTestDispatcher()

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        itemDao = mock(ItemDao::class.java)
        customerDao = mock(CustomerDao::class.java)
        masterDataService = mock(MasterDataService::class.java)

        `when`(itemDao.getItemCount()).thenReturn(flowOf(12))
        `when`(customerDao.getCustomerCount()).thenReturn(flowOf(4))

        viewModel = MasterDataViewModel(itemDao, customerDao, masterDataService)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `counts reflect dao counts`() = runTest {
        testScheduler.advanceUntilIdle()
        assertEquals(12, viewModel.itemCount.value)
        assertEquals(4, viewModel.customerCount.value)
    }

    @Test
    fun `syncItems inserts items and updates state`() = runTest {
        viewModel.syncItems()
        testScheduler.advanceUntilIdle()

        verify(itemDao).insertItems(anyList())
        assertEquals(false, viewModel.uiState.value.isSyncingItems)
        assertNotNull(viewModel.uiState.value.statusMessage)
    }
}
