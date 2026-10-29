package com.kra.paypoint.ui.screens.dashboard.viewmodel

import com.kra.paypoint.domain.exception.toUserMessage
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import kotlinx.coroutines.flow.combine
import com.kra.paypoint.domain.repository.TransactionRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import javax.inject.Inject

data class DashboardUiState(
    val operatorName: String = "Cashier",
    val isAdmin: Boolean = false,
    val isSyncing: Boolean = false,
    val syncMessage: String? = null
)

@HiltViewModel
class DashboardViewModel @Inject constructor(
    private val authRepository: AuthRepository,
    private val transactionRepository: TransactionRepository,
    masterDataRepository: MasterDataRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(DashboardUiState())
    val uiState: StateFlow<DashboardUiState> = _uiState.asStateFlow()

    val pendingSyncCount: StateFlow<Int> = transactionRepository.getPendingCount()
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), 0)

    /** Live "today at a glance" — recomputes whenever a sale is recorded or stock changes. */
    val home: StateFlow<HomeSnapshot> = combine(
        transactionRepository.getAllTransactions(),
        masterDataRepository.getAllItems()
    ) { transactions, items -> HomeSnapshot.from(transactions, items) }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), HomeSnapshot())

    init {
        viewModelScope.launch {
            authRepository.currentUser.collect { user ->
                _uiState.update {
                    it.copy(operatorName = user?.name ?: "Operator", isAdmin = user?.isAdmin == true)
                }
            }
        }
    }

    fun triggerSync() {
        viewModelScope.launch {
            _uiState.update { it.copy(isSyncing = true, syncMessage = null) }
            try {
                transactionRepository.triggerSync()
                delay(1000) // Brief animation interval for user feedback
                _uiState.update {
                    it.copy(
                        isSyncing = false,
                        syncMessage = "Sync triggered. Verifying connectivity..."
                    )
                }
            } catch (e: Exception) {
                _uiState.update {
                    it.copy(
                        isSyncing = false,
                        syncMessage = "Sync request failed: ${e.toUserMessage()}"
                    )
                }
            }
        }
    }

    fun logout() {
        viewModelScope.launch {
            authRepository.logout()
        }
    }
}
