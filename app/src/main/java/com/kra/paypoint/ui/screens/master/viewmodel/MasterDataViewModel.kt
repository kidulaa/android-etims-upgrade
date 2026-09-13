package com.kra.paypoint.ui.screens.master.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.data.local.dao.CustomerDao
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import dagger.hilt.android.lifecycle.HiltViewModel
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
    itemDao: ItemDao,
    customerDao: CustomerDao,
    private val masterDataRepository: MasterDataRepository,
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(MasterDataUiState())
    val uiState: StateFlow<MasterDataUiState> = _uiState.asStateFlow()

    val itemCount: StateFlow<Int> = itemDao.getItemCount()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), 0)

    val customerCount: StateFlow<Int> = customerDao.getCustomerCount()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), 0)

    /** eTIMS device registration owns tin/bhfId; without it there's nothing to sync against. */
    private fun requireBranch(): Pair<String, String>? {
        val user = authRepository.currentUser.value ?: return null
        return user.tin to user.branchId
    }

    fun syncItems() {
        val branch = requireBranch()
        if (branch == null) {
            _uiState.update { it.copy(statusMessage = "Sign in before syncing the item catalog.") }
            return
        }
        val (tin, bhfId) = branch

        viewModelScope.launch {
            _uiState.update { it.copy(isSyncingItems = true, statusMessage = null) }
            val result = masterDataRepository.syncItemsFromEtims(tin, bhfId)
            _uiState.update {
                it.copy(
                    isSyncingItems = false,
                    statusMessage = result.fold(
                        onSuccess = { count -> "Synced $count item(s) from eTIMS." },
                        onFailure = { e -> "Item sync failed: ${e.localizedMessage}" }
                    )
                )
            }
        }
    }

    fun syncCustomers() {
        val branch = requireBranch()
        if (branch == null) {
            _uiState.update { it.copy(statusMessage = "Sign in before syncing customers.") }
            return
        }
        val (tin, bhfId) = branch

        viewModelScope.launch {
            _uiState.update { it.copy(isSyncingCustomers = true, statusMessage = null) }
            val result = masterDataRepository.syncCustomersFromEtims(tin, bhfId)
            _uiState.update {
                it.copy(
                    isSyncingCustomers = false,
                    statusMessage = result.fold(
                        onSuccess = { count -> "Synced $count customer(s) from eTIMS." },
                        onFailure = { e -> "Customer sync failed: ${e.localizedMessage}" }
                    )
                )
            }
        }
    }
}
