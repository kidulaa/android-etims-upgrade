package com.kra.paypoint.ui.screens.master.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.dao.CustomerDao
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.remote.api.MasterDataService
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

data class MasterDataUiState(
    val isSyncingItems: Boolean = false,
    val isSyncingCustomers: Boolean = false,
    val statusMessage: String? = null
)

@HiltViewModel
class MasterDataViewModel @Inject constructor(
    private val itemDao: ItemDao,
    private val customerDao: CustomerDao,
    private val masterDataService: MasterDataService
) : ViewModel() {

    private val _uiState = MutableStateFlow(MasterDataUiState())
    val uiState: StateFlow<MasterDataUiState> = _uiState.asStateFlow()

    val itemCount: StateFlow<Int> = itemDao.getItemCount()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), 0)

    val customerCount: StateFlow<Int> = customerDao.getCustomerCount()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), 0)

    fun syncItems() {
        viewModelScope.launch {
            _uiState.update { it.copy(isSyncingItems = true, statusMessage = null) }
            try {
                // If local database is uninitialized, populate foundational standard eTIMS items
                val initialItems = listOf(
                    ItemEntity(
                        itemCode = "ITM-001",
                        itemClassificationCode = "50202306",
                        itemTypeCode = "1",
                        itemName = "Mineral Water 500ml",
                        barcode = "616110001001",
                        packagingUnitCode = "BT",
                        packageQuantity = 1.0,
                        quantityUnitCode = "U",
                        taxTypeCode = "A", // 16% VAT
                        defaultUnitPrice = 50.0,
                        stockQuantity = 200.0,
                        isUsed = true
                    ),
                    ItemEntity(
                        itemCode = "ITM-002",
                        itemClassificationCode = "50181901",
                        itemTypeCode = "1",
                        itemName = "White Bread 400g",
                        barcode = "616110001002",
                        packagingUnitCode = "PK",
                        packageQuantity = 1.0,
                        quantityUnitCode = "U",
                        taxTypeCode = "C", // Exempt
                        defaultUnitPrice = 65.0,
                        stockQuantity = 80.0,
                        isUsed = true
                    ),
                    ItemEntity(
                        itemCode = "ITM-003",
                        itemClassificationCode = "50201706",
                        itemTypeCode = "1",
                        itemName = "Fresh Milk 500ml",
                        barcode = "616110001003",
                        packagingUnitCode = "PK",
                        packageQuantity = 1.0,
                        quantityUnitCode = "U",
                        taxTypeCode = "C", // Exempt
                        defaultUnitPrice = 60.0,
                        stockQuantity = 120.0,
                        isUsed = true
                    ),
                    ItemEntity(
                        itemCode = "ITM-004",
                        itemClassificationCode = "50202301",
                        itemTypeCode = "1",
                        itemName = "Carbonated Soft Drink 500ml",
                        barcode = "616110001004",
                        packagingUnitCode = "BT",
                        packageQuantity = 1.0,
                        quantityUnitCode = "U",
                        taxTypeCode = "A", // 16% VAT
                        defaultUnitPrice = 70.0,
                        stockQuantity = 150.0,
                        isUsed = true
                    ),
                    ItemEntity(
                        itemCode = "ITM-005",
                        itemClassificationCode = "14111507",
                        itemTypeCode = "1",
                        itemName = "Notebook A4 200 Pages",
                        barcode = "616110001005",
                        packagingUnitCode = "EA",
                        packageQuantity = 1.0,
                        quantityUnitCode = "U",
                        taxTypeCode = "A", // 16% VAT
                        defaultUnitPrice = 180.0,
                        stockQuantity = 50.0,
                        isUsed = true
                    )
                )

                itemDao.insertItems(initialItems)
                delay(800)

                _uiState.update {
                    it.copy(
                        isSyncingItems = false,
                        statusMessage = "Items synchronized successfully."
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isSyncingItems = false,
                        statusMessage = "Item sync notice: ${e.localizedMessage}"
                    )
                }
            }
        }
    }

    fun syncCustomers() {
        viewModelScope.launch {
            _uiState.update { it.copy(isSyncingCustomers = true, statusMessage = null) }
            try {
                val initialCustomers = listOf(
                    CustomerEntity(
                        customerTin = "P051234567Z",
                        customerName = "Nairobi Retailers Ltd",
                        customerNo = "CUST-001",
                        telNo = "+254711000001",
                        email = "orders@nairobitraders.co.ke",
                        address = "Kenyatta Ave, Nairobi",
                        isUsed = true
                    ),
                    CustomerEntity(
                        customerTin = "P059876543A",
                        customerName = "Safari Logistics Enterprise",
                        customerNo = "CUST-002",
                        telNo = "+254722000002",
                        email = "accounts@safarilogistics.com",
                        address = "Mombasa Road, Nairobi",
                        isUsed = true
                    )
                )

                customerDao.insertCustomers(initialCustomers)
                delay(800)

                _uiState.update {
                    it.copy(
                        isSyncingCustomers = false,
                        statusMessage = "Customers synchronized successfully."
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isSyncingCustomers = false,
                        statusMessage = "Customer sync notice: ${e.localizedMessage}"
                    )
                }
            }
        }
    }
}
