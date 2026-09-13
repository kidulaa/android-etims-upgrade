package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import androidx.room.Update
import com.kra.paypoint.data.local.entity.CustomerEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface CustomerDao {
    @Query("SELECT * FROM customers WHERE isUsed = 1 ORDER BY customerName ASC")
    fun getAllCustomers(): Flow<List<CustomerEntity>>

    @Query("SELECT * FROM customers WHERE isUsed = 1 AND (customerName LIKE '%' || :query || '%' OR customerTin LIKE '%' || :query || '%' OR telNo LIKE '%' || :query || '%') ORDER BY customerName ASC")
    fun searchCustomers(query: String): Flow<List<CustomerEntity>>

    @Query("SELECT * FROM customers WHERE customerTin = :tin LIMIT 1")
    suspend fun getCustomerByTin(tin: String): CustomerEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertCustomer(customer: CustomerEntity)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertCustomers(customers: List<CustomerEntity>)

    @Update
    suspend fun updateCustomer(customer: CustomerEntity)

    @Query("SELECT * FROM customers WHERE syncStatus != 'SYNCED'")
    suspend fun getUnsyncedCustomers(): List<CustomerEntity>

    @Query("UPDATE customers SET syncStatus = :status WHERE customerTin = :tin")
    suspend fun updateCustomerSyncStatus(tin: String, status: String)

    @Query("SELECT COUNT(*) FROM customers WHERE isUsed = 1")
    fun getCustomerCount(): Flow<Int>
}
