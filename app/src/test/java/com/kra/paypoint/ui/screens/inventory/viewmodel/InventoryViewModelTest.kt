package com.kra.paypoint.ui.screens.inventory.viewmodel

import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.InventoryRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
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
import org.mockito.Mockito.anyDouble
import org.mockito.Mockito.anyString
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify

@OptIn(ExperimentalCoroutinesApi::class)
class InventoryViewModelTest {

    private lateinit var masterDataRepository: MasterDataRepository
    private lateinit var inventoryRepository: InventoryRepository
    private lateinit var authRepository: AuthRepository
    private lateinit var viewModel: InventoryViewModel
    private val testDispatcher = StandardTestDispatcher()

    private val sampleItem = ItemEntity(
        itemCode = "ITM-50",
        itemClassificationCode = "5020",
        itemTypeCode = "1",
        itemName = "Apple Juice 500ml",
        defaultUnitPrice = 80.0,
        stockQuantity = 10.0,
        isUsed = true
    )

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        masterDataRepository = mock(MasterDataRepository::class.java)
        inventoryRepository = mock(InventoryRepository::class.java)
        authRepository = mock(AuthRepository::class.java)

        `when`(masterDataRepository.getAllItems()).thenReturn(flowOf(listOf(sampleItem)))
        `when`(inventoryRepository.getAllMovements()).thenReturn(flowOf(emptyList()))
        `when`(authRepository.currentUser).thenReturn(MutableStateFlow(User("C01", "Cashier", "ROLE_CASHIER", "00", "P012345678X")))

        viewModel = InventoryViewModel(masterDataRepository, inventoryRepository, authRepository)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `search filters item list`() = runTest {
        testScheduler.advanceUntilIdle()
        assertEquals(1, viewModel.filteredItems.value.size)

        viewModel.setSearchQuery("Mango")
        testScheduler.advanceUntilIdle()
        assertEquals(0, viewModel.filteredItems.value.size)

        viewModel.setSearchQuery("Apple")
        testScheduler.advanceUntilIdle()
        assertEquals(1, viewModel.filteredItems.value.size)
    }

    @Test
    fun `stockIn triggers inventoryRepository and resets selectedItem`() = runTest {
        `when`(inventoryRepository.recordStockIn(anyString(), anyDouble(), anyString(), anyString(), anyString(), anyString()))
            .thenReturn(Result.success(20.0))

        viewModel.selectItem(sampleItem)
        viewModel.stockIn(10.0, "PO Delivery")
        testScheduler.advanceUntilIdle()

        verify(inventoryRepository).recordStockIn(
            org.mockito.Mockito.eq("ITM-50"),
            org.mockito.Mockito.eq(10.0),
            anyString(),
            anyString(),
            anyString(),
            anyString()
        )
        assertEquals(null, viewModel.uiState.value.selectedItem)
        assertNotNull(viewModel.uiState.value.feedbackMessage)
    }
}
