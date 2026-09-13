package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.TransactionDao
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow

/**
 * In-memory [TransactionDao] used by repository/worker tests. Unlike a Mockito mock, this
 * actually runs the real `insertWithNextInvoiceNumber` default-method sequencing logic
 * (Mockito would stub that method away entirely, defeating the point of testing it), and
 * enforces the same "no overwrite on collision" behavior as the real Room-backed DAO.
 */
class FakeTransactionDao : TransactionDao {

    private val transactionsFlow = MutableStateFlow<List<TransactionEntity>>(emptyList())
    private val itemsByTransactionId = mutableMapOf<Long, MutableList<TransactionItemEntity>>()
    private var nextId = 1L

    val allInserted: List<TransactionEntity> get() = transactionsFlow.value

    override suspend fun insertTransaction(transaction: TransactionEntity): Long {
        if (transactionsFlow.value.any { it.invoiceNumber == transaction.invoiceNumber }) {
            throw IllegalStateException("UNIQUE constraint failed: transactions.invoiceNumber")
        }
        val id = if (transaction.id != 0L) transaction.id else nextId++
        transactionsFlow.value = transactionsFlow.value + transaction.copy(id = id)
        return id
    }

    override suspend fun insertTransactionItems(items: List<TransactionItemEntity>) {
        for (item in items) {
            itemsByTransactionId.getOrPut(item.transactionId) { mutableListOf() }.add(item)
        }
    }

    override suspend fun updatePayload(id: Long, payloadJson: String) {
        transactionsFlow.value = transactionsFlow.value.map {
            if (it.id == id) it.copy(payloadJson = payloadJson) else it
        }
    }

    override fun getPendingTransactions(): Flow<List<TransactionEntity>> =
        MutableStateFlow(transactionsFlow.value.filter { it.syncStatus == "PENDING" || it.syncStatus == "FAILED" })

    override suspend fun getPendingTransactionsList(): List<TransactionEntity> =
        transactionsFlow.value.filter { it.syncStatus == "PENDING" || it.syncStatus == "FAILED" }

    override fun getAllTransactions(): Flow<List<TransactionEntity>> = transactionsFlow

    override suspend fun getTransactionWithItems(id: Long): TransactionWithItems? {
        val tx = transactionsFlow.value.find { it.id == id } ?: return null
        return TransactionWithItems(tx, itemsByTransactionId[id].orEmpty())
    }

    override suspend fun getTransactionByInvoiceNumber(invoiceNumber: Long): TransactionWithItems? {
        val tx = transactionsFlow.value.find { it.invoiceNumber == invoiceNumber } ?: return null
        return TransactionWithItems(tx, itemsByTransactionId[tx.id].orEmpty())
    }

    override suspend fun updateSyncStatus(
        id: Long,
        status: String,
        attempts: Int,
        error: String?,
        syncedAt: Long?,
        receiptUrl: String?,
        qrCodeData: String?
    ) {
        transactionsFlow.value = transactionsFlow.value.map {
            if (it.id == id) {
                it.copy(
                    syncStatus = status,
                    syncAttempts = attempts,
                    lastError = error,
                    syncedAt = syncedAt ?: it.syncedAt,
                    receiptUrl = receiptUrl ?: it.receiptUrl,
                    qrCodeData = qrCodeData ?: it.qrCodeData
                )
            } else it
        }
    }

    override fun getPendingCount(): Flow<Int> =
        MutableStateFlow(transactionsFlow.value.count { it.syncStatus == "PENDING" || it.syncStatus == "FAILED" })

    override suspend fun getLastInvoiceNumber(): Long? =
        transactionsFlow.value.maxOfOrNull { it.invoiceNumber }
}
