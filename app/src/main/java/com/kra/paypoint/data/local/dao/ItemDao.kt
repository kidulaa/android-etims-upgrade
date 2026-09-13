package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import androidx.room.Update
import com.kra.paypoint.data.local.entity.ItemEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface ItemDao {
    @Query("SELECT * FROM items WHERE isUsed = 1 ORDER BY itemName ASC")
    fun getAllItems(): Flow<List<ItemEntity>>

    @Query("SELECT * FROM items WHERE isUsed = 1 AND (itemName LIKE '%' || :query || '%' OR itemCode LIKE '%' || :query || '%' OR barcode = :query) ORDER BY itemName ASC")
    fun searchItems(query: String): Flow<List<ItemEntity>>

    @Query("SELECT * FROM items WHERE itemCode = :itemCode LIMIT 1")
    suspend fun getItemByCode(itemCode: String): ItemEntity?

    @Query("SELECT * FROM items WHERE barcode = :barcode LIMIT 1")
    suspend fun getItemByBarcode(barcode: String): ItemEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertItem(item: ItemEntity)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertItems(items: List<ItemEntity>)

    @Update
    suspend fun updateItem(item: ItemEntity)

    @Query("SELECT COUNT(*) FROM items WHERE isUsed = 1")
    fun getItemCount(): Flow<Int>

    /** Absolute set — used only for a physical-recount adjustment, where "the new value" IS the input. */
    @Query("UPDATE items SET stockQuantity = :newQuantity WHERE itemCode = :itemCode")
    suspend fun setStockQuantity(itemCode: String, newQuantity: Double)

    /** Atomic relative increase — race-free because SQLite computes the delta server-side, not from a stale read. */
    @Query("UPDATE items SET stockQuantity = stockQuantity + :delta WHERE itemCode = :itemCode")
    suspend fun increaseStock(itemCode: String, delta: Double)

    /**
     * Atomic conditional decrease: only applies if enough stock is currently on hand.
     * Returns the number of rows updated (0 means insufficient stock — checked against the
     * live value, not a value read moments earlier by a possibly-concurrent caller).
     */
    @Query("UPDATE items SET stockQuantity = stockQuantity - :quantity WHERE itemCode = :itemCode AND stockQuantity >= :quantity")
    suspend fun decreaseStockIfSufficient(itemCode: String, quantity: Double): Int
}
