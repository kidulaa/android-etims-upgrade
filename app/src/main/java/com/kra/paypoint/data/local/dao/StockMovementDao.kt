package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.kra.paypoint.data.local.entity.StockMovementEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface StockMovementDao {
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertMovement(movement: StockMovementEntity): Long

    @Query("SELECT * FROM stock_movements ORDER BY timestamp DESC")
    fun getAllMovements(): Flow<List<StockMovementEntity>>

    @Query("SELECT * FROM stock_movements WHERE itemCode = :itemCode ORDER BY timestamp DESC")
    fun getMovementsForItem(itemCode: String): Flow<List<StockMovementEntity>>

    @Query("SELECT * FROM stock_movements WHERE syncStatus = 'PENDING'")
    suspend fun getPendingMovements(): List<StockMovementEntity>

    @Query("UPDATE stock_movements SET syncStatus = :status WHERE id = :id")
    suspend fun updateSyncStatus(id: Long, status: String)
}
