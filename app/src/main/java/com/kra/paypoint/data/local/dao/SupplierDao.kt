package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.kra.paypoint.data.local.entity.SupplierEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface SupplierDao {
    @Query("SELECT * FROM suppliers ORDER BY supplierName ASC")
    fun observeAll(): Flow<List<SupplierEntity>>

    @Query("SELECT * FROM suppliers WHERE isUsed = 1 AND (supplierName LIKE '%' || :query || '%' OR supplierTin LIKE '%' || :query || '%') ORDER BY supplierName ASC")
    fun search(query: String): Flow<List<SupplierEntity>>

    @Query("SELECT * FROM suppliers WHERE supplierTin = :tin LIMIT 1")
    suspend fun getByTin(tin: String): SupplierEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun upsert(supplier: SupplierEntity)
}
