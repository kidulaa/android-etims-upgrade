package com.kra.paypoint.ui.screens.inventory.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.StockMovementEntity
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.InventoryRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

data class InventoryUiState(
    val selectedItem: ItemEntity? = null,
    val isProcessing: Boolean = false,
    val feedbackMessage: String? = null,
    val isError: Boolean = false,
    val searchQuery: String = ""
)

@HiltViewModel
class InventoryViewModel @Inject constructor(
    private val masterDataRepository: MasterDataRepository,
    private val inventoryRepository: InventoryRepository,
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(InventoryUiState())
    val uiState: StateFlow<InventoryUiState> = _uiState.asStateFlow()

    private val allItems: StateFlow<List<ItemEntity>> =
        masterDataRepository.getAllItems()
            .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    val filteredItems: StateFlow<List<ItemEntity>> =
        combine(allItems, _uiState) { list, state ->
            if (state.searchQuery.isBlank()) list
            else list.filter {
                it.itemName.contains(state.searchQuery, ignoreCase = true) ||
                it.itemCode.contains(state.searchQuery, ignoreCase = true) ||
                (it.barcode != null && it.barcode.contains(state.searchQuery))
            }
        }.stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    val recentMovements: StateFlow<List<StockMovementEntity>> =
        inventoryRepository.getAllMovements()
            .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), emptyList())

    fun selectItem(item: ItemEntity?) {
        _uiState.update { it.copy(selectedItem = item, feedbackMessage = null) }
    }

    fun setSearchQuery(query: String) {
        _uiState.update { it.copy(searchQuery = query) }
    }

    fun stockIn(quantity: Double, remark: String? = null) {
        val item = _uiState.value.selectedItem ?: return
        viewModelScope.launch {
            _uiState.update { it.copy(isProcessing = true, feedbackMessage = null) }
            val operatorId = authRepository.currentUser.value?.id ?: "OPERATOR"
            val result = inventoryRepository.recordStockIn(
                itemCode = item.itemCode,
                quantity = quantity,
                reasonCode = "01",
                reasonDescription = "Purchase Intake",
                remark = remark,
                operatorId = operatorId
            )
            result.fold(
                onSuccess = { newStock ->
                    _uiState.update {
                        it.copy(
                            isProcessing = false,
                            selectedItem = null,
                            feedbackMessage = "Stock-In recorded. New inventory for '${item.itemName}': $newStock units.",
                            isError = false
                        )
                    }
                },
                onFailure = { err ->
                    _uiState.update {
                        it.copy(
                            isProcessing = false,
                            feedbackMessage = "Stock-In failed: ${err.localizedMessage}",
                            isError = true
                        )
                    }
                }
            )
        }
    }

    fun stockOut(
        quantity: Double,
        reasonCode: String = "02",
        reasonDescription: String = "Damaged Goods",
        remark: String? = null
    ) {
        val item = _uiState.value.selectedItem ?: return
        viewModelScope.launch {
            _uiState.update { it.copy(isProcessing = true, feedbackMessage = null) }
            val operatorId = authRepository.currentUser.value?.id ?: "OPERATOR"
            val result = inventoryRepository.recordStockOut(
                itemCode = item.itemCode,
                quantity = quantity,
                reasonCode = reasonCode,
                reasonDescription = reasonDescription,
                remark = remark,
                operatorId = operatorId
            )
            result.fold(
                onSuccess = { newStock ->
                    _uiState.update {
                        it.copy(
                            isProcessing = false,
                            selectedItem = null,
                            feedbackMessage = "Stock-Out recorded. Remaining stock for '${item.itemName}': $newStock units.",
                            isError = false
                        )
                    }
                },
                onFailure = { err ->
                    _uiState.update {
                        it.copy(
                            isProcessing = false,
                            feedbackMessage = "Stock-Out failed: ${err.localizedMessage}",
                            isError = true
                        )
                    }
                }
            )
        }
    }

    fun adjustStock(physicalCount: Double, remark: String? = null) {
        val item = _uiState.value.selectedItem ?: return
        viewModelScope.launch {
            _uiState.update { it.copy(isProcessing = true, feedbackMessage = null) }
            val operatorId = authRepository.currentUser.value?.id ?: "OPERATOR"
            val result = inventoryRepository.recordAdjustment(
                itemCode = item.itemCode,
                physicalCount = physicalCount,
                remark = remark,
                operatorId = operatorId
            )
            result.fold(
                onSuccess = { adjustedCount ->
                    _uiState.update {
                        it.copy(
                            isProcessing = false,
                            selectedItem = null,
                            feedbackMessage = "Inventory recount saved. Stock for '${item.itemName}' adjusted to: $adjustedCount units.",
                            isError = false
                        )
                    }
                },
                onFailure = { err ->
                    _uiState.update {
                        it.copy(
                            isProcessing = false,
                            feedbackMessage = "Adjustment failed: ${err.localizedMessage}",
                            isError = true
                        )
                    }
                }
            )
        }
    }

    fun clearFeedback() {
        _uiState.update { it.copy(feedbackMessage = null) }
    }
}
