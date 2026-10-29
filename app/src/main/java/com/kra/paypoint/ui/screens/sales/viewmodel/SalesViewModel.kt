package com.kra.paypoint.ui.screens.sales.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveItem
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import com.kra.paypoint.domain.model.EtimsCodes.PaymentType
import com.kra.paypoint.domain.model.EtimsCodes.ReceiptType
import com.kra.paypoint.domain.model.EtimsCodes.SalesType
import com.kra.paypoint.domain.model.EtimsCodes.TaxType
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.model.Discount
import com.kra.paypoint.domain.model.PricedLine
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.DocumentRepository
import com.kra.paypoint.domain.repository.SaleExtras
import com.kra.paypoint.domain.repository.MasterDataRepository
import com.kra.paypoint.domain.repository.PreferencesRepository
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.domain.usecase.TaxCalculationEngine
import com.kra.paypoint.hardware.printer.PrinterService
import com.kra.paypoint.ui.export.ReceiptHeader
import com.kra.paypoint.ui.export.ReceiptJournal
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.flatMapLatest
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import javax.inject.Inject

data class CartItem(
    val item: ItemEntity,
    val quantity: Double = 1.0,
    val discount: Discount = Discount.NONE,
    /** Set by "Change Price"; null means the item's registered sale price. */
    val priceOverride: Double? = null,
    /** Band this line is filed under — the item's own, or D for a non-VAT taxpayer. */
    val taxTypeCode: String = item.taxTypeCode
) {
    val unitPrice: Double get() = priceOverride ?: item.defaultUnitPrice

    val priced: PricedLine get() = PricedLine(unitPrice, quantity, discount, taxTypeCode)

    /** prc × qty, before discount — KRA's splyAmt. */
    val supplyAmount: Double get() = priced.supplyAmount
    val discountAmount: Double get() = priced.discountAmount
    /** Effective rate even for a fixed-amount discount, as KRA's dcRt expects. */
    val discountPercent: Double get() = priced.discountRate
    val lineTotal: Double get() = priced.lineTotal
    val taxableAmount: Double get() = priced.taxableAmount
    val taxAmount: Double get() = priced.taxAmount
}

/** A finished sale kept on screen for Print / Send SMS, per the eTIMS guide's last step. */
data class CompletedSale(
    val invoiceNumber: Long,
    val totalAmount: Double,
    val amountReceived: Double,
    val change: Double,
    val customerPhone: String?,
    val isCredit: Boolean = false
)

data class SalesUiState(
    val cart: List<CartItem> = emptyList(),
    val selectedCustomer: CustomerEntity? = null,
    val paymentTypeCode: String = PaymentType.CASH,
    /** Raw text from the "amount received" field; blank means the exact total. */
    val amountReceivedInput: String = "",
    val searchQuery: String = "",
    val note: String = "",
    /** Days until a credit sale falls due. */
    val creditDays: Int = 30,
    /** Quotation/proforma this sale was started from; marked converted once the invoice is issued. */
    val sourceDocumentId: Long? = null,
    val sourceDocumentNumber: String? = null,
    val isLoading: Boolean = false,
    val lastCompletedInvoiceNumber: Long? = null,
    val completedSale: CompletedSale? = null,
    val isPrinting: Boolean = false,
    val printMessage: String? = null,
    val errorMessage: String? = null
) {
    val totalAmount: Double get() = cart.sumOf { it.lineTotal }
    val totalTaxAmount: Double get() = cart.sumOf { it.taxAmount }
    val totalTaxableAmount: Double get() = cart.sumOf { it.taxableAmount }
    val totalDiscount: Double get() = cart.sumOf { it.discountAmount }
    val itemCount: Int get() = cart.size

    val amountReceived: Double
        get() = amountReceivedInput.trim().replace(",", "").toDoubleOrNull() ?: totalAmount

    val change: Double get() = (amountReceived - totalAmount).coerceAtLeast(0.0)

    val isCredit: Boolean get() = paymentTypeCode == PaymentType.CREDIT

    /** Only cash can be over-tendered; wallet/card are for the exact amount and credit is paid later. */
    val isPaymentSufficient: Boolean
        get() = paymentTypeCode != PaymentType.CASH || amountReceived + 0.005 >= totalAmount
}

@HiltViewModel
class SalesViewModel @Inject constructor(
    private val transactionRepository: TransactionRepository,
    private val masterDataRepository: MasterDataRepository,
    private val authRepository: AuthRepository,
    private val deviceRepository: DeviceRepository,
    private val preferencesRepository: PreferencesRepository,
    private val printerService: PrinterService,
    private val documentRepository: DocumentRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(SalesUiState())
    val uiState: StateFlow<SalesUiState> = _uiState.asStateFlow()

    val availableItems: StateFlow<List<ItemEntity>> = masterDataRepository.getAllItems()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    val pendingSyncCount: StateFlow<Int> = transactionRepository.getPendingCount()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), 0)

    private val customerQuery = MutableStateFlow("")

    @OptIn(ExperimentalCoroutinesApi::class)
    val customerResults: StateFlow<List<CustomerEntity>> = customerQuery
        .flatMapLatest { q ->
            if (q.isBlank()) masterDataRepository.getAllCustomers() else masterDataRepository.searchCustomers(q.trim())
        }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    /** Kept so Print / Send SMS can rebuild the receipt after the cart has been cleared. */
    private var lastSale: TransactionWithItems? = null

    fun onCustomerQueryChanged(query: String) {
        customerQuery.value = query
    }

    fun addItemToCart(item: ItemEntity, quantity: Double = 1.0) {
        val bandForSale = if (preferencesRepository.nonVatFlag.value) TaxType.NON_VAT else item.taxTypeCode
        _uiState.update { state ->
            val existingIndex = state.cart.indexOfFirst { it.item.itemCode == item.itemCode }
            val updatedCart = if (existingIndex >= 0) {
                state.cart.toMutableList().apply {
                    val existing = this[existingIndex]
                    this[existingIndex] = existing.copy(quantity = existing.quantity + quantity)
                }
            } else {
                state.cart + CartItem(item = item, quantity = quantity, taxTypeCode = bandForSale)
            }
            state.copy(cart = updatedCart, errorMessage = null)
        }
    }

    fun updateCartItemQuantity(itemCode: String, newQuantity: Double) {
        _uiState.update { state ->
            val updatedCart = if (newQuantity <= 0) {
                state.cart.filterNot { it.item.itemCode == itemCode }
            } else {
                state.cart.map {
                    if (it.item.itemCode == itemCode) it.copy(quantity = newQuantity) else it
                }
            }
            state.copy(cart = updatedCart)
        }
    }

    fun setDiscount(itemCode: String, percent: Double) = applyDiscount(itemCode, Discount.Percent(percent))

    /** Discount as a fixed KES amount off the line, the guide's alternative to a percentage. */
    fun setFixedDiscount(itemCode: String, amount: Double) = applyDiscount(itemCode, Discount.Fixed(amount))

    private fun applyDiscount(itemCode: String, discount: Discount) {
        val line = _uiState.value.cart.firstOrNull { it.item.itemCode == itemCode } ?: return
        PricedLine.validateDiscount(discount, line.supplyAmount)?.let { error ->
            _uiState.update { it.copy(errorMessage = error) }
            return
        }
        updateLine(itemCode) { it.copy(discount = discount) }
    }

    fun onNoteChanged(note: String) {
        _uiState.update { it.copy(note = note) }
    }

    fun setCreditDays(days: Int) {
        _uiState.update { it.copy(creditDays = days) }
    }

    /**
     * "Create Invoice" from a quotation/proforma: loads its lines into the cart at the quoted
     * prices and discounts. Items since discontinued are skipped and reported.
     */
    fun loadFromDocument(documentId: Long) {
        if (_uiState.value.sourceDocumentId == documentId) return
        viewModelScope.launch {
            val doc = documentRepository.get(documentId)
            if (doc == null || doc.document.status != "OPEN") {
                _uiState.update { it.copy(errorMessage = "That document is no longer open for invoicing.") }
                return@launch
            }
            val nonVat = preferencesRepository.nonVatFlag.value
            val missing = mutableListOf<String>()
            val cart = doc.items.sortedBy { it.itemSequence }.mapNotNull { line ->
                val item = masterDataRepository.getItemByCode(line.itemCode)?.takeIf { it.isUsed }
                if (item == null) {
                    missing += line.itemName
                    null
                } else {
                    CartItem(
                        item = item,
                        quantity = line.quantity,
                        discount = if (line.discountAmount > 0.0) Discount.Fixed(line.discountAmount) else Discount.NONE,
                        priceOverride = line.unitPrice.takeIf { it != item.defaultUnitPrice },
                        taxTypeCode = if (nonVat) TaxType.NON_VAT else item.taxTypeCode
                    )
                }
            }
            val customer = doc.document.partyTin?.let { masterDataRepository.getCustomerByTin(it) }
            _uiState.update {
                it.copy(
                    cart = cart,
                    selectedCustomer = customer,
                    note = doc.document.note.orEmpty(),
                    sourceDocumentId = documentId,
                    sourceDocumentNumber = doc.document.displayNumber,
                    errorMessage = missing.takeIf { m -> m.isNotEmpty() }
                        ?.let { m -> "Not added (no longer sold): ${m.joinToString()}" }
                )
            }
        }
    }

    fun changePrice(itemCode: String, newPrice: Double) {
        if (newPrice <= 0.0) {
            _uiState.update { it.copy(errorMessage = "Price must be greater than zero.") }
            return
        }
        updateLine(itemCode) { line ->
            line.copy(priceOverride = newPrice.takeIf { it != line.item.defaultUnitPrice })
        }
    }

    private fun updateLine(itemCode: String, transform: (CartItem) -> CartItem) {
        _uiState.update { state ->
            state.copy(
                cart = state.cart.map { if (it.item.itemCode == itemCode) transform(it) else it },
                errorMessage = null
            )
        }
    }

    fun removeCartItem(itemCode: String) {
        _uiState.update { state ->
            state.copy(cart = state.cart.filterNot { it.item.itemCode == itemCode })
        }
    }

    fun selectCustomer(customer: CustomerEntity?) {
        _uiState.update { it.copy(selectedCustomer = customer) }
    }

    fun setPaymentType(code: String) {
        _uiState.update { it.copy(paymentTypeCode = code, errorMessage = null) }
    }

    fun onAmountReceivedChanged(input: String) {
        _uiState.update { it.copy(amountReceivedInput = input, errorMessage = null) }
    }

    /** "Cancel Sales": abandons the current, not-yet-issued sale. */
    fun clearCart() {
        _uiState.update {
            it.copy(
                cart = emptyList(),
                selectedCustomer = null,
                amountReceivedInput = "",
                paymentTypeCode = PaymentType.CASH,
                errorMessage = null
            )
        }
    }

    /** Dismisses the completed-sale dialog and starts the next sale. */
    fun startNewSale() {
        lastSale = null
        _uiState.value = SalesUiState()
    }

    fun checkout() {
        val currentState = _uiState.value
        // Re-entry guard: a double tap can land before the disabled button recomposes.
        if (currentState.isLoading) return
        if (currentState.cart.isEmpty()) {
            _uiState.update { it.copy(errorMessage = "Cart is empty. Add items before checkout.") }
            return
        }
        if (currentState.isCredit && currentState.selectedCustomer == null) {
            _uiState.update { it.copy(errorMessage = "Select the customer who will pay this credit sale later.") }
            return
        }
        if (!currentState.isPaymentSufficient) {
            _uiState.update {
                it.copy(errorMessage = String.format("Amount received is less than the total (KES %,.2f).", currentState.totalAmount))
            }
            return
        }

        val operator = authRepository.currentUser.value
        if (operator == null) {
            _uiState.update { it.copy(errorMessage = "No signed-in operator. Please sign in again.") }
            return
        }

        _uiState.update { it.copy(isLoading = true, errorMessage = null) }
        viewModelScope.launch {
            try {

                // Finalized atomically by TransactionRepository.processSale — this placeholder
                // is only used to keep the request payload shape valid before that point.
                val invoiceNumberPlaceholder = 0L
                val dateStr = LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyyMMddHHmmss"))

                val bands = TaxCalculationEngine.bandTotals(
                    currentState.cart.map { Triple(it.taxTypeCode, it.taxableAmount, it.taxAmount) }
                )

                val remoteItems = currentState.cart.mapIndexed { idx, cartItem ->
                    TrnsSalesSaveItem(
                        itemSeq = idx + 1,
                        itemCd = cartItem.item.itemCode,
                        itemClsCd = cartItem.item.itemClassificationCode,
                        itemNm = cartItem.item.itemName,
                        bcd = cartItem.item.barcode,
                        pkgUnitCd = cartItem.item.packagingUnitCode,
                        pkg = cartItem.item.packageQuantity,
                        qtyUnitCd = cartItem.item.quantityUnitCode,
                        qty = cartItem.quantity,
                        prc = cartItem.unitPrice,
                        splyAmt = cartItem.supplyAmount,
                        dcRt = cartItem.discountPercent,
                        dcAmt = cartItem.discountAmount,
                        isrccCd = null,
                        isrccNm = null,
                        isrcRt = 0.0,
                        isrcAmt = 0.0,
                        taxTyCd = cartItem.taxTypeCode,
                        taxblAmt = cartItem.taxableAmount,
                        taxAmt = cartItem.taxAmount,
                        totAmt = cartItem.lineTotal
                    )
                }

                val remoteReq = TrnsSalesSaveReq(
                    tin = operator.tin,
                    bhfId = operator.branchId,
                    invcNo = invoiceNumberPlaceholder,
                    orgInvcNo = 0L,
                    custTin = currentState.selectedCustomer?.customerTin,
                    custNm = currentState.selectedCustomer?.customerName ?: "Walk-in Customer",
                    salesTyCd = SalesType.NORMAL,
                    rcptTyCd = ReceiptType.SALE,
                    pmtTyCd = currentState.paymentTypeCode,
                    rfdRsnCd = null,
                    salesSttsCd = "02", // Approved/Complete
                    cfmDt = dateStr,
                    salesDt = dateStr.substring(0, 8),
                    stockRlsDt = dateStr,
                    cnclReqDt = null,
                    cnclDt = null,
                    rfdDt = null,
                    totItemCnt = currentState.cart.size.toDouble(),
                    taxblAmtA = bands.taxableFor(TaxType.EXEMPT),
                    taxblAmtB = bands.taxableFor(TaxType.STANDARD),
                    taxblAmtC = bands.taxableFor(TaxType.ZERO_RATED),
                    taxblAmtD = bands.taxableFor(TaxType.NON_VAT),
                    taxblAmtE = bands.taxableFor(TaxType.REDUCED),
                    taxRtA = TaxCalculationEngine.ratePercentFor(TaxType.EXEMPT),
                    taxRtB = TaxCalculationEngine.ratePercentFor(TaxType.STANDARD),
                    taxRtC = TaxCalculationEngine.ratePercentFor(TaxType.ZERO_RATED),
                    taxRtD = TaxCalculationEngine.ratePercentFor(TaxType.NON_VAT),
                    taxRtE = TaxCalculationEngine.ratePercentFor(TaxType.REDUCED),
                    taxAmtA = bands.taxFor(TaxType.EXEMPT),
                    taxAmtB = bands.taxFor(TaxType.STANDARD),
                    taxAmtC = bands.taxFor(TaxType.ZERO_RATED),
                    taxAmtD = bands.taxFor(TaxType.NON_VAT),
                    taxAmtE = bands.taxFor(TaxType.REDUCED),
                    totTaxblAmt = bands.totalTaxable,
                    totTaxAmt = bands.totalTax,
                    totAmt = currentState.totalAmount,
                    prchrAcptcYn = "N",
                    remark = currentState.note.trim().ifEmpty { null },
                    regrId = operator.id,
                    regrNm = operator.name,
                    modrId = operator.id,
                    modrNm = operator.name,
                    itemList = remoteItems
                )

                val lineItemEntities = currentState.cart.mapIndexed { idx, cartItem ->
                    TransactionItemEntity(
                        transactionId = 0L, // Assigned once the invoice number is finalized in TransactionRepository
                        itemSequence = idx + 1,
                        itemCode = cartItem.item.itemCode,
                        itemClassificationCode = cartItem.item.itemClassificationCode,
                        itemName = cartItem.item.itemName,
                        barcode = cartItem.item.barcode,
                        packagingUnitCode = cartItem.item.packagingUnitCode,
                        packageQuantity = cartItem.item.packageQuantity,
                        quantityUnitCode = cartItem.item.quantityUnitCode,
                        quantity = cartItem.quantity,
                        unitPrice = cartItem.unitPrice,
                        supplyAmount = cartItem.supplyAmount,
                        discountRate = cartItem.discountPercent,
                        discountAmount = cartItem.discountAmount,
                        taxTypeCode = cartItem.taxTypeCode,
                        taxableAmount = cartItem.taxableAmount,
                        taxAmount = cartItem.taxAmount,
                        totalAmount = cartItem.lineTotal
                    )
                }

                val extras = SaleExtras(
                    isPaid = !currentState.isCredit,
                    dueDate = if (currentState.isCredit) {
                        LocalDate.now().plusDays(currentState.creditDays.toLong()).format(DateTimeFormatter.BASIC_ISO_DATE)
                    } else null,
                    note = currentState.note.trim().ifEmpty { null }
                )
                val (transactionId, assignedInvoiceNumber) = transactionRepository.processSale(remoteReq, lineItemEntities, extras)
                currentState.sourceDocumentId?.let { documentRepository.markConverted(it, "INV $assignedInvoiceNumber") }
                lastSale = transactionRepository.getTransactionDetails(transactionId)

                _uiState.update {
                    SalesUiState(
                        lastCompletedInvoiceNumber = assignedInvoiceNumber,
                        completedSale = CompletedSale(
                            invoiceNumber = assignedInvoiceNumber,
                            totalAmount = currentState.totalAmount,
                            amountReceived = if (currentState.isCredit) 0.0 else currentState.amountReceived,
                            change = if (currentState.isCredit) 0.0 else currentState.change,
                            customerPhone = currentState.selectedCustomer?.telNo,
                            isCredit = currentState.isCredit
                        ),
                        isLoading = false
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isLoading = false,
                        errorMessage = "Sale checkout failed: ${e.toUserMessage()}"
                    )
                }
            }
        }
    }

    private fun receiptHeader(): ReceiptHeader = ReceiptHeader.from(
        registration = deviceRepository.registration.value,
        customAddress = preferencesRepository.customAddress.value,
        customPhone = preferencesRepository.customPhone.value,
        customEmail = preferencesRepository.customEmail.value
    )

    /** "Finished – Print Journal". */
    fun printLastReceipt() {
        val sale = lastSale
        val completed = _uiState.value.completedSale
        if (sale == null || completed == null) {
            _uiState.update { it.copy(printMessage = "Receipt is not available to print.") }
            return
        }
        viewModelScope.launch {
            _uiState.update { it.copy(isPrinting = true, printMessage = null) }
            val message = try {
                val journal = ReceiptJournal.build(
                    data = sale,
                    header = receiptHeader(),
                    registration = deviceRepository.registration.value,
                    isCopy = false,
                    amountReceived = completed.amountReceived.takeUnless { completed.isCredit }
                )
                printerService.printReceipt(journal, isReprint = false)
                "Receipt #${completed.invoiceNumber} sent to printer."
            } catch (e: Exception) {
                "Print failed: ${e.toUserMessage()}. Check the printer under Device Setting."
            }
            _uiState.update { it.copy(isPrinting = false, printMessage = message) }
        }
    }

    /** "Finished – Send SMS": text for the UI to hand to the SMS app, or null if there's no sale. */
    fun smsTextForLastReceipt(): String? = lastSale?.let { ReceiptJournal.buildSms(it, receiptHeader()) }
}
