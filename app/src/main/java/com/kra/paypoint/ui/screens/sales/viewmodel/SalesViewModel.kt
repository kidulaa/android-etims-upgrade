package com.kra.paypoint.ui.screens.sales.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveItem
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.domain.usecase.TaxCalculationEngine
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import javax.inject.Inject

data class CartItem(
    val item: ItemEntity,
    val quantity: Double = 1.0,
    val discountPercent: Double = 0.0
) {
    val unitPrice: Double get() = item.defaultUnitPrice
    val lineTotal: Double get() = unitPrice * quantity * (1.0 - (discountPercent / 100.0))

    private val split get() = TaxCalculationEngine.split(lineTotal, item.taxTypeCode)
    val taxableAmount: Double get() = split.taxableAmount.toDouble()
    val taxAmount: Double get() = split.taxAmount.toDouble()
}

data class SalesUiState(
    val cart: List<CartItem> = emptyList(),
    val selectedCustomer: CustomerEntity? = null,
    val paymentTypeCode: String = "01", // 01: Cash, 02: Card, 03: Mobile Money
    val searchQuery: String = "",
    val isLoading: Boolean = false,
    val lastCompletedInvoiceNumber: Long? = null,
    val errorMessage: String? = null
) {
    val totalAmount: Double get() = cart.sumOf { it.lineTotal }
    val totalTaxAmount: Double get() = cart.sumOf { it.taxAmount }
    val totalTaxableAmount: Double get() = cart.sumOf { it.taxableAmount }
    val itemCount: Int get() = cart.size
}

@HiltViewModel
class SalesViewModel @Inject constructor(
    private val transactionRepository: TransactionRepository,
    private val masterDataRepository: MasterDataRepository,
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(SalesUiState())
    val uiState: StateFlow<SalesUiState> = _uiState.asStateFlow()

    val availableItems: StateFlow<List<ItemEntity>> = masterDataRepository.getAllItems()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    val pendingSyncCount: StateFlow<Int> = transactionRepository.getPendingCount()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), 0)

    fun addItemToCart(item: ItemEntity, quantity: Double = 1.0) {
        _uiState.update { state ->
            val existingIndex = state.cart.indexOfFirst { it.item.itemCode == item.itemCode }
            val updatedCart = if (existingIndex >= 0) {
                state.cart.toMutableList().apply {
                    val existing = this[existingIndex]
                    this[existingIndex] = existing.copy(quantity = existing.quantity + quantity)
                }
            } else {
                state.cart + CartItem(item = item, quantity = quantity)
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

    fun removeCartItem(itemCode: String) {
        _uiState.update { state ->
            state.copy(cart = state.cart.filterNot { it.item.itemCode == itemCode })
        }
    }

    fun selectCustomer(customer: CustomerEntity?) {
        _uiState.update { it.copy(selectedCustomer = customer) }
    }

    fun setPaymentType(code: String) {
        _uiState.update { it.copy(paymentTypeCode = code) }
    }

    fun clearCart() {
        _uiState.update { it.copy(cart = emptyList(), selectedCustomer = null, errorMessage = null) }
    }

    fun checkout() {
        val currentState = _uiState.value
        if (currentState.cart.isEmpty()) {
            _uiState.update { it.copy(errorMessage = "Cart is empty. Add items before checkout.") }
            return
        }

        val operator = authRepository.currentUser.value
        if (operator == null) {
            _uiState.update { it.copy(errorMessage = "No signed-in operator. Please sign in again.") }
            return
        }

        viewModelScope.launch {
            try {
                _uiState.update { it.copy(isLoading = true, errorMessage = null) }

                // Finalized atomically by TransactionRepository.processSale — this placeholder
                // is only used to keep the request payload shape valid before that point.
                val invoiceNumberPlaceholder = 0L
                val dateStr = LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyyMMddHHmmss"))

                // Group taxes by category
                val taxA = currentState.cart.filter { it.item.taxTypeCode == "A" }
                val taxB = currentState.cart.filter { it.item.taxTypeCode == "B" }
                val taxC = currentState.cart.filter { it.item.taxTypeCode == "C" }
                val taxD = currentState.cart.filter { it.item.taxTypeCode == "D" }
                val taxE = currentState.cart.filter { it.item.taxTypeCode == "E" }

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
                        splyAmt = cartItem.taxableAmount,
                        dcRt = cartItem.discountPercent,
                        dcAmt = 0.0,
                        isrccCd = null,
                        isrccNm = null,
                        isrcRt = 0.0,
                        isrcAmt = 0.0,
                        taxTyCd = cartItem.item.taxTypeCode,
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
                    salesTyCd = "N",
                    rcptTyCd = "R",
                    pmtTyCd = currentState.paymentTypeCode,
                    rfdRsnCd = null,
                    salesSttsCd = "02", // Approved/Complete
                    cfmDt = dateStr,
                    salesDt = dateStr.substring(0, 8),
                    stockRlsDt = dateStr,
                    cnclReqDt = null,
                    cnclDt = null,
                    rfdDt = null,
                    totItemCnt = currentState.cart.sumOf { it.quantity },
                    taxblAmtA = taxA.sumOf { it.taxableAmount },
                    taxblAmtB = taxB.sumOf { it.taxableAmount },
                    taxblAmtC = taxC.sumOf { it.taxableAmount },
                    taxblAmtD = taxD.sumOf { it.taxableAmount },
                    taxblAmtE = taxE.sumOf { it.taxableAmount },
                    taxRtA = 16,
                    taxRtB = 0,
                    taxRtC = 0,
                    taxRtD = 0,
                    taxRtE = 8,
                    taxAmtA = taxA.sumOf { it.taxAmount },
                    taxAmtB = taxB.sumOf { it.taxAmount },
                    taxAmtC = taxC.sumOf { it.taxAmount },
                    taxAmtD = taxD.sumOf { it.taxAmount },
                    taxAmtE = taxE.sumOf { it.taxAmount },
                    totTaxblAmt = currentState.totalTaxableAmount,
                    totTaxAmt = currentState.totalTaxAmount,
                    totAmt = currentState.totalAmount,
                    prchrAcptcYn = "N",
                    remark = null,
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
                        supplyAmount = cartItem.taxableAmount,
                        discountRate = cartItem.discountPercent,
                        discountAmount = 0.0,
                        taxTypeCode = cartItem.item.taxTypeCode,
                        taxableAmount = cartItem.taxableAmount,
                        taxAmount = cartItem.taxAmount,
                        totalAmount = cartItem.lineTotal
                    )
                }

                val (_, assignedInvoiceNumber) = transactionRepository.processSale(remoteReq, lineItemEntities)

                _uiState.update {
                    SalesUiState(
                        lastCompletedInvoiceNumber = assignedInvoiceNumber,
                        isLoading = false
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isLoading = false,
                        errorMessage = "Sale checkout failed: ${e.localizedMessage}"
                    )
                }
            }
        }
    }
}
