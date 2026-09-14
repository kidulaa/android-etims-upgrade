package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import androidx.room.Transaction
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import kotlinx.coroutines.flow.Flow

data class SaleInsertResult(val transactionId: Long, val invoiceNumber: Long)

@Dao
interface TransactionDao {
    // No onConflict override: the default ABORT strategy means a colliding invoiceNumber
    // throws instead of silently overwriting (and deleting) an existing fiscal record.
    @Insert
    suspend fun insertTransaction(transaction: TransactionEntity): Long

    @Insert
    suspend fun insertTransactionItems(items: List<TransactionItemEntity>)

    /**
     * Assigns the next monotonic invoice number and inserts the transaction in the same Room
     * transaction, so the read-then-write can't race against another insert from this device.
     *
     * The number is `max(local MAX(invoiceNumber), floorInvoiceNumber) + 1`: [floorInvoiceNumber]
     * should be the device's `lastSaleInvcNo` from eTIMS device registration
     * (DeviceRegistration.lastSaleInvoiceNumber), so a reinstalled app with an empty local
     * table resumes numbering from where this device left off with KRA instead of restarting
     * at 1 and potentially reissuing a number already submitted — the same reason the legacy
     * till kept this floor in a separate persisted config, not just the transactions table.
     *
     * [buildEntity] receives the assigned number to finalize the row (and the payload that
     * will be sent to KRA) before it's written.
     */
    @Transaction
    suspend fun insertWithNextInvoiceNumber(
        floorInvoiceNumber: Long = 0L,
        buildEntity: suspend (Long) -> TransactionEntity,
        buildItems: suspend (Long) -> List<TransactionItemEntity>
    ): SaleInsertResult {
        val nextInvoiceNumber = maxOf(getLastInvoiceNumber() ?: 0L, floorInvoiceNumber) + 1L
        val entity = buildEntity(nextInvoiceNumber)
        val transactionId = insertTransaction(entity)
        val items = buildItems(transactionId)
        if (items.isNotEmpty()) {
            insertTransactionItems(items)
        }
        return SaleInsertResult(transactionId, nextInvoiceNumber)
    }

    @Query("UPDATE transactions SET payloadJson = :payloadJson WHERE id = :id")
    suspend fun updatePayload(id: Long, payloadJson: String)

    @Query("SELECT * FROM transactions WHERE syncStatus = 'PENDING' OR syncStatus = 'FAILED' ORDER BY invoiceNumber ASC")
    fun getPendingTransactions(): Flow<List<TransactionEntity>>

    @Query("SELECT * FROM transactions WHERE syncStatus = 'PENDING' OR syncStatus = 'FAILED' ORDER BY invoiceNumber ASC")
    suspend fun getPendingTransactionsList(): List<TransactionEntity>

    @Query("SELECT * FROM transactions ORDER BY createdAt DESC")
    fun getAllTransactions(): Flow<List<TransactionEntity>>

    @Transaction
    @Query("SELECT * FROM transactions ORDER BY createdAt DESC")
    suspend fun getAllTransactionsWithItems(): List<TransactionWithItems>

    @Transaction
    @Query("SELECT * FROM transactions WHERE id = :id LIMIT 1")
    suspend fun getTransactionWithItems(id: Long): TransactionWithItems?

    @Transaction
    @Query("SELECT * FROM transactions WHERE invoiceNumber = :invoiceNumber LIMIT 1")
    suspend fun getTransactionByInvoiceNumber(invoiceNumber: Long): TransactionWithItems?

    @Query("UPDATE transactions SET syncStatus = :status, syncAttempts = :attempts, lastError = :error, syncedAt = :syncedAt, receiptUrl = COALESCE(:receiptUrl, receiptUrl), qrCodeData = COALESCE(:qrCodeData, qrCodeData) WHERE id = :id")
    suspend fun updateSyncStatus(
        id: Long,
        status: String,
        attempts: Int,
        error: String? = null,
        syncedAt: Long? = null,
        receiptUrl: String? = null,
        qrCodeData: String? = null
    )

    @Query("SELECT COUNT(*) FROM transactions WHERE syncStatus = 'PENDING' OR syncStatus = 'FAILED'")
    fun getPendingCount(): Flow<Int>

    @Query("SELECT MAX(invoiceNumber) FROM transactions")
    suspend fun getLastInvoiceNumber(): Long?
}
