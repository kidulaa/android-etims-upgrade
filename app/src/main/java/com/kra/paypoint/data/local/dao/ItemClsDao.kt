package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.kra.paypoint.data.local.entity.ItemClsEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface ItemClsDao {
    @Query("SELECT * FROM item_classifications WHERE isUsed = 1 ORDER BY name ASC")
    fun getAllActive(): Flow<List<ItemClsEntity>>

    @Query("SELECT * FROM item_classifications WHERE code = :code LIMIT 1")
    suspend fun getByCode(code: String): ItemClsEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAll(list: List<ItemClsEntity>)
    
    @Query("SELECT COUNT(*) FROM item_classifications")
    suspend fun count(): Int
}
