package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.kra.paypoint.data.local.entity.CodeEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface CodeDao {
    @Query("SELECT * FROM system_codes WHERE classCode = :classCode AND isUsed = 1 ORDER BY sortOrder ASC")
    fun getCodesByClass(classCode: String): Flow<List<CodeEntity>>

    @Query("SELECT * FROM system_codes WHERE classCode = :classCode AND detailCode = :detailCode LIMIT 1")
    suspend fun getCode(classCode: String, detailCode: String): CodeEntity?

    @Query("SELECT * FROM system_codes WHERE classCode = 'TAX_TYPE' AND isUsed = 1")
    suspend fun getTaxTypes(): List<CodeEntity>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertCodes(codes: List<CodeEntity>)
}
