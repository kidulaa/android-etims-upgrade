package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.kra.paypoint.data.local.entity.ZReportEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface ZReportDao {
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertZReport(report: ZReportEntity): Long

    @Query("SELECT * FROM z_reports ORDER BY zReportNumber DESC")
    fun getAllZReports(): Flow<List<ZReportEntity>>

    @Query("SELECT * FROM z_reports ORDER BY zReportNumber DESC LIMIT 1")
    suspend fun getLastZReport(): ZReportEntity?

    @Query("SELECT * FROM z_reports WHERE zReportNumber = :number LIMIT 1")
    suspend fun getZReportByNumber(number: Long): ZReportEntity?

    @Query("SELECT COALESCE(MAX(zReportNumber), 0) + 1 FROM z_reports")
    suspend fun getNextZReportNumber(): Long
}
