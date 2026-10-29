package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import androidx.room.Transaction
import com.kra.paypoint.data.local.entity.PurchaseEntity
import com.kra.paypoint.data.local.entity.PurchaseItemEntity
import com.kra.paypoint.data.local.entity.PurchaseWithItems
import kotlinx.coroutines.flow.Flow

data class PurchaseInsertResult(val purchaseId: Long, val purchaseNumber: Long)

@Dao
interface PurchaseDao {
    @Insert
    suspend fun insertPurchase(purchase: PurchaseEntity): Long

    @Insert
    suspend fun insertItems(items: List<PurchaseItemEntity>)

    @Query("SELECT MAX(purchaseNumber) FROM purchases")
    suspend fun getLastPurchaseNumber(): Long?

    /** Same race-free sequencing as sales invoices, floored at the device's KRA purchase counter. */
    @Transaction
    suspend fun insertWithNextNumber(
        floorPurchaseNumber: Long,
        build: (purchaseNumber: Long) -> PurchaseEntity,
        items: List<PurchaseItemEntity>
    ): PurchaseInsertResult {
        val next = maxOf(getLastPurchaseNumber() ?: 0L, floorPurchaseNumber) + 1
        val id = insertPurchase(build(next))
        insertItems(items.map { it.copy(purchaseId = id) })
        return PurchaseInsertResult(id, next)
    }

    @Query("SELECT * FROM purchases ORDER BY createdAt DESC")
    fun observeAll(): Flow<List<PurchaseEntity>>

    @Transaction
    @Query("SELECT * FROM purchases WHERE id = :id LIMIT 1")
    suspend fun getWithItems(id: Long): PurchaseWithItems?

    @Query("SELECT * FROM purchases WHERE syncStatus != 'SYNCED' ORDER BY purchaseNumber ASC")
    suspend fun getUnsynced(): List<PurchaseEntity>

    @Query("UPDATE purchases SET syncStatus = :status, syncAttempts = syncAttempts + 1, lastError = :error WHERE id = :id")
    suspend fun updateSyncStatus(id: Long, status: String, error: String?)

    @Query("SELECT COUNT(*) FROM purchases WHERE supplierTin = :supplierTin AND supplierInvoiceNumber = :supplierInvoiceNumber")
    suspend fun countBySupplierInvoice(supplierTin: String, supplierInvoiceNumber: String): Int
}
