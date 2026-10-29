package com.kra.paypoint.ui.screens.sales.viewmodel

import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import com.kra.paypoint.domain.model.EtimsCodes.PaymentType
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.DocumentRepository
import com.kra.paypoint.domain.repository.SaleExtras
import com.kra.paypoint.domain.repository.MasterDataRepository
import com.kra.paypoint.domain.repository.PreferencesRepository
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.hardware.printer.PrinterService
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
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertNull
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock
import org.mockito.Mockito.never
import org.mockito.kotlin.any
import org.mockito.kotlin.argumentCaptor
import org.mockito.kotlin.verify

@OptIn(ExperimentalCoroutinesApi::class)
class SalesViewModelTest {

    private lateinit var viewModel: SalesViewModel
    private lateinit var transactionRepository: TransactionRepository
    private lateinit var masterDataRepository: MasterDataRepository
    private lateinit var authRepository: AuthRepository
    private lateinit var deviceRepository: DeviceRepository
    private lateinit var preferencesRepository: PreferencesRepository
    private lateinit var printerService: PrinterService
    private lateinit var documentRepository: DocumentRepository
    private val nonVatFlag = MutableStateFlow(false)
    private val testDispatcher = StandardTestDispatcher()

    private val testOperator = User(
        id = "CASHIER01",
        name = "Jane Cashier",
        authorityCode = User.ROLE_USER,
        branchId = "00",
        tin = "P012345678X"
    )

    private val water = ItemEntity(
        itemCode = "KE2NTU0000001",
        itemClassificationCode = "50202306",
        itemTypeCode = "2",
        itemName = "Bottled Mineral Water 500ml",
        barcode = "616110000001",
        quantityUnitCode = "U",
        taxTypeCode = "B", // 16% standard VAT
        defaultUnitPrice = 116.0, // 100 taxable + 16 VAT
        stockQuantity = 50.0
    )

    private val bread = ItemEntity(
        itemCode = "KE2NTU0000002",
        itemClassificationCode = "50202306",
        itemTypeCode = "2",
        itemName = "Bread 400g",
        taxTypeCode = "A", // Exempt
        defaultUnitPrice = 65.0
    )

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
        transactionRepository = mock(TransactionRepository::class.java)
        masterDataRepository = mock(MasterDataRepository::class.java)
        authRepository = mock(AuthRepository::class.java)
        deviceRepository = mock(DeviceRepository::class.java)
        preferencesRepository = mock(PreferencesRepository::class.java)
        printerService = mock(PrinterService::class.java)
        documentRepository = mock(DocumentRepository::class.java)

        `when`(transactionRepository.getPendingCount()).thenReturn(flowOf(0))
        `when`(masterDataRepository.getAllItems()).thenReturn(flowOf(emptyList()))
        `when`(authRepository.currentUser).thenReturn(MutableStateFlow(testOperator))
        `when`(preferencesRepository.nonVatFlag).thenReturn(nonVatFlag)

        viewModel = newViewModel()
    }

    private fun newViewModel() = SalesViewModel(
        transactionRepository, masterDataRepository, authRepository, deviceRepository, preferencesRepository, printerService, documentRepository
    )

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `addItemToCart splits a 16 percent band B line into taxable and VAT`() {
        viewModel.addItemToCart(water, quantity = 2.0)

        val state = viewModel.uiState.value
        assertEquals(1, state.cart.size)
        assertEquals(232.0, state.totalAmount, 0.001)
        val line = state.cart.first()
        assertEquals(200.0, line.taxableAmount, 0.001)
        assertEquals(32.0, line.taxAmount, 0.001)
    }

    @Test
    fun `band A is exempt and carries no VAT`() {
        viewModel.addItemToCart(bread, quantity = 2.0)

        val line = viewModel.uiState.value.cart.first()
        assertEquals(130.0, line.taxableAmount, 0.001)
        assertEquals(0.0, line.taxAmount, 0.001)
    }

    @Test
    fun `discount reduces the line total and is reported separately from supply amount`() {
        viewModel.addItemToCart(water, quantity = 2.0)
        viewModel.setDiscount(water.itemCode, 10.0)

        val line = viewModel.uiState.value.cart.first()
        assertEquals(232.0, line.supplyAmount, 0.001)
        assertEquals(23.2, line.discountAmount, 0.001)
        assertEquals(208.8, line.lineTotal, 0.001)
        assertEquals(208.8, line.taxableAmount + line.taxAmount, 0.001)
    }

    @Test
    fun `discount outside 0 to 100 percent is rejected`() {
        viewModel.addItemToCart(water)
        viewModel.setDiscount(water.itemCode, 120.0)

        assertEquals(0.0, viewModel.uiState.value.cart.first().discountPercent, 0.0)
        assertNotNull(viewModel.uiState.value.errorMessage)
    }

    @Test
    fun `change price overrides the registered price for this sale only`() {
        viewModel.addItemToCart(water, quantity = 3.0)
        viewModel.changePrice(water.itemCode, 100.0)

        val line = viewModel.uiState.value.cart.first()
        assertEquals(100.0, line.unitPrice, 0.0)
        assertEquals(300.0, line.lineTotal, 0.001)
        assertEquals(116.0, line.item.defaultUnitPrice, 0.0)
    }

    @Test
    fun `cash change is computed and under-tendering blocks checkout`() = runTest {
        viewModel.addItemToCart(water) // 116.00
        viewModel.onAmountReceivedChanged("200")
        assertEquals(84.0, viewModel.uiState.value.change, 0.001)

        viewModel.onAmountReceivedChanged("100")
        assertFalse(viewModel.uiState.value.isPaymentSufficient)
        viewModel.checkout()
        testScheduler.advanceUntilIdle()

        verify(transactionRepository, never()).processSale(any(), any(), any())
        assertNotNull(viewModel.uiState.value.errorMessage)
    }

    @Test
    fun `mobile wallet payments are always for the exact amount`() {
        viewModel.addItemToCart(water)
        viewModel.setPaymentType(PaymentType.MOBILE_MONEY)
        viewModel.onAmountReceivedChanged("1")

        assertTrue(viewModel.uiState.value.isPaymentSufficient)
    }

    @Test
    fun `non-VAT mode files every line under band D with no VAT`() {
        nonVatFlag.value = true
        viewModel.addItemToCart(water)

        val line = viewModel.uiState.value.cart.first()
        assertEquals("D", line.taxTypeCode)
        assertEquals(0.0, line.taxAmount, 0.0)
    }

    @Test
    fun `checkout files a KRA sale with receipt type S and per-band totals`() = runTest {
        `when`(transactionRepository.processSale(any(), any(), any())).thenReturn(500L to 101L)

        viewModel.addItemToCart(water, quantity = 1.0)
        viewModel.addItemToCart(bread, quantity = 1.0)
        viewModel.setPaymentType(PaymentType.MOBILE_MONEY)
        viewModel.checkout()
        testScheduler.advanceUntilIdle()

        val captor = argumentCaptor<TrnsSalesSaveReq>()
        verify(transactionRepository).processSale(captor.capture(), any(), any())
        val req = captor.firstValue
        assertEquals("N", req.salesTyCd)
        assertEquals("S", req.rcptTyCd)
        assertEquals("06", req.pmtTyCd)
        assertEquals("P012345678X", req.tin)
        assertEquals(65.0, req.taxblAmtA, 0.001)
        assertEquals(0.0, req.taxAmtA, 0.001)
        assertEquals(100.0, req.taxblAmtB, 0.001)
        assertEquals(16.0, req.taxAmtB, 0.001)
        assertEquals(16, req.taxRtB)
        assertEquals(0, req.taxRtA)
        assertEquals(181.0, req.totAmt, 0.001)
        assertEquals(2.0, req.totItemCnt, 0.0)

        val after = viewModel.uiState.value
        assertEquals(0, after.cart.size)
        assertEquals(101L, after.lastCompletedInvoiceNumber)
        assertEquals(101L, after.completedSale?.invoiceNumber)
    }

    @Test
    fun `startNew clears the completed sale`() = runTest {
        `when`(transactionRepository.processSale(any(), any(), any())).thenReturn(500L to 101L)
        viewModel.addItemToCart(bread)
        viewModel.checkout()
        testScheduler.advanceUntilIdle()

        viewModel.startNewSale()

        assertNull(viewModel.uiState.value.completedSale)
    }

    @Test
    fun `checkout refuses to submit a sale with no signed-in operator`() = runTest {
        `when`(authRepository.currentUser).thenReturn(MutableStateFlow(null))
        viewModel = newViewModel()

        viewModel.addItemToCart(bread, quantity = 1.0)
        viewModel.checkout()

        val state = viewModel.uiState.value
        assertNotNull(state.errorMessage)
        assertEquals(1, state.cart.size) // sale never went through, cart untouched
    }

    @Test
    fun `fixed discount is capped validation and reported as an effective rate`() {
        viewModel.addItemToCart(water, quantity = 2.0) // 232.00
        viewModel.setFixedDiscount(water.itemCode, 500.0)
        assertNotNull(viewModel.uiState.value.errorMessage)
        assertEquals(0.0, viewModel.uiState.value.cart.first().discountAmount, 0.0)

        viewModel.setFixedDiscount(water.itemCode, 23.2)
        val line = viewModel.uiState.value.cart.first()
        assertEquals(23.2, line.discountAmount, 0.001)
        assertEquals(10.0, line.discountPercent, 0.001)
        assertEquals(208.8, line.lineTotal, 0.001)
    }

    @Test
    fun `credit sale needs a customer and is recorded unpaid with a due date and code 02`() = runTest {
        `when`(transactionRepository.processSale(any(), any(), any())).thenReturn(500L to 101L)
        viewModel.addItemToCart(bread)
        viewModel.setPaymentType(PaymentType.CREDIT)
        viewModel.checkout()
        testScheduler.advanceUntilIdle()
        verify(transactionRepository, never()).processSale(any(), any(), any())
        assertNotNull(viewModel.uiState.value.errorMessage)

        viewModel.selectCustomer(com.kra.paypoint.data.local.entity.CustomerEntity(customerTin = "P051234567Z", customerName = "Acme Ltd"))
        viewModel.setCreditDays(14)
        viewModel.onNoteChanged("Deliver Friday")
        viewModel.checkout()
        testScheduler.advanceUntilIdle()

        val req = argumentCaptor<TrnsSalesSaveReq>()
        val extras = argumentCaptor<SaleExtras>()
        verify(transactionRepository).processSale(req.capture(), any(), extras.capture())
        assertEquals("02", req.firstValue.pmtTyCd)
        assertEquals("Deliver Friday", req.firstValue.remark)
        assertFalse(extras.firstValue.isPaid)
        assertEquals(
            java.time.LocalDate.now().plusDays(14).format(java.time.format.DateTimeFormatter.BASIC_ISO_DATE),
            extras.firstValue.dueDate
        )
        assertTrue(viewModel.uiState.value.completedSale!!.isCredit)
    }

    @Test
    fun `invoicing a quotation loads its lines and marks it converted after the sale`() = runTest {
        val doc = com.kra.paypoint.data.local.entity.CommercialDocumentWithItems(
            document = com.kra.paypoint.data.local.entity.CommercialDocumentEntity(
                id = 9L, docType = "QUOTATION", sequence = 3L, partyName = "Brian", docDate = "20260101", note = "Quoted"
            ),
            items = listOf(
                com.kra.paypoint.data.local.entity.CommercialDocumentItemEntity(
                    documentId = 9L, itemSequence = 1, itemCode = water.itemCode, itemName = water.itemName,
                    quantity = 3.0, unitPrice = 100.0, discountAmount = 30.0, taxTypeCode = "B", totalAmount = 270.0
                )
            )
        )
        `when`(documentRepository.get(9L)).thenReturn(doc)
        `when`(masterDataRepository.getItemByCode(water.itemCode)).thenReturn(water)
        `when`(transactionRepository.processSale(any(), any(), any())).thenReturn(500L to 101L)

        viewModel.loadFromDocument(9L)
        testScheduler.advanceUntilIdle()
        val line = viewModel.uiState.value.cart.single()
        assertEquals(3.0, line.quantity, 0.0)
        assertEquals(100.0, line.unitPrice, 0.0)
        assertEquals(270.0, line.lineTotal, 0.001)
        assertEquals("QUO000003", viewModel.uiState.value.sourceDocumentNumber)

        viewModel.checkout()
        testScheduler.advanceUntilIdle()
        verify(documentRepository).markConverted(9L, "INV 101")
    }

    @Test
    fun `a double tap on confirm issues only one invoice`() = runTest {
        `when`(transactionRepository.processSale(any(), any(), any())).thenReturn(500L to 101L)
        viewModel.addItemToCart(bread)

        viewModel.checkout()
        viewModel.checkout()
        testScheduler.advanceUntilIdle()

        verify(transactionRepository, org.mockito.Mockito.times(1)).processSale(any(), any(), any())
    }
}
