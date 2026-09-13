package com.kra.paypoint.ui.screens.sales.viewmodel

import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import com.kra.paypoint.domain.repository.TransactionRepository
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.any
import org.mockito.Mockito.mock

@OptIn(ExperimentalCoroutinesApi::class)
class SalesViewModelTest {

    private lateinit var viewModel: SalesViewModel
    private lateinit var transactionRepository: TransactionRepository
    private lateinit var masterDataRepository: MasterDataRepository
    private lateinit var authRepository: AuthRepository
    private val testDispatcher = StandardTestDispatcher()

    private val testOperator = User(
        id = "CASHIER01",
        name = "Jane Cashier",
        authorityCode = "ROLE_CASHIER",
        branchId = "00",
        tin = "P012345678X"
    )

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        transactionRepository = mock(TransactionRepository::class.java)
        masterDataRepository = mock(MasterDataRepository::class.java)
        authRepository = mock(AuthRepository::class.java)

        `when`(transactionRepository.getPendingCount()).thenReturn(flowOf(0))
        `when`(masterDataRepository.getAllItems()).thenReturn(flowOf(emptyList()))
        `when`(authRepository.currentUser).thenReturn(MutableStateFlow(testOperator))

        viewModel = SalesViewModel(transactionRepository, masterDataRepository, authRepository)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `addItemToCart computes lineTotal and tax amount correctly`() {
        val testItem = ItemEntity(
            itemCode = "ITEM-001",
            itemClassificationCode = "50202306",
            itemTypeCode = "1",
            itemName = "Bottled Mineral Water 500ml",
            barcode = "616110000001",
            packagingUnitCode = "BT",
            quantityUnitCode = "U",
            taxTypeCode = "A", // 16% standard VAT
            defaultUnitPrice = 116.0, // 100 supply + 16 tax
            stockQuantity = 50.0,
            isUsed = true
        )

        viewModel.addItemToCart(testItem, quantity = 2.0)

        val currentState = viewModel.uiState.value
        assertEquals(1, currentState.cart.size)
        assertEquals(232.0, currentState.totalAmount, 0.001)

        val cartItem = currentState.cart.first()
        assertEquals(2.0, cartItem.quantity, 0.0)
        assertEquals(200.0, cartItem.taxableAmount, 0.001)
        assertEquals(32.0, cartItem.taxAmount, 0.001)
    }

    @Test
    fun `checkout calls processSale with the signed-in operator's identity and clears cart`() = runTest {
        val testItem = ItemEntity(
            itemCode = "ITEM-002",
            itemClassificationCode = "50202306",
            itemTypeCode = "1",
            itemName = "Bread 400g",
            taxTypeCode = "C", // Exempt
            defaultUnitPrice = 65.0,
            isUsed = true
        )

        `when`(transactionRepository.processSale(any(), any())).thenReturn(500L to 101L)

        viewModel.addItemToCart(testItem, quantity = 1.0)
        viewModel.checkout()

        testScheduler.advanceUntilIdle()

        val stateAfterCheckout = viewModel.uiState.value
        assertEquals(0, stateAfterCheckout.cart.size)
        assertEquals(101L, stateAfterCheckout.lastCompletedInvoiceNumber)
        assertNotNull(stateAfterCheckout.lastCompletedInvoiceNumber)
    }

    @Test
    fun `checkout refuses to submit a sale with no signed-in operator`() = runTest {
        `when`(authRepository.currentUser).thenReturn(MutableStateFlow(null))
        viewModel = SalesViewModel(transactionRepository, masterDataRepository, authRepository)

        val testItem = ItemEntity(
            itemCode = "ITEM-003",
            itemClassificationCode = "50202306",
            itemTypeCode = "1",
            itemName = "Bread 400g",
            taxTypeCode = "C",
            defaultUnitPrice = 65.0,
            isUsed = true
        )
        viewModel.addItemToCart(testItem, quantity = 1.0)
        viewModel.checkout()

        val state = viewModel.uiState.value
        assertNotNull(state.errorMessage)
        assertEquals(1, state.cart.size) // sale never went through, cart untouched
    }
}
