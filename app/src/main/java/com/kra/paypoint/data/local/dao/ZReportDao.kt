package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import androidx.room.Transaction
import com.kra.paypoint.data.local.entity.ZReportEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface ZReportDao {
    // No onConflict override: a colliding zReportNumber should throw, not silently replace
    // (and delete) an existing fiscal Z-report — the same reasoning as TransactionDao.
    @Insert
    suspend fun insertZReport(report: ZReportEntity): Long

    @Query("SELECT * FROM z_reports ORDER BY zReportNumber DESC")
    fun getAllZReports(): Flow<List<ZReportEntity>>

    @Query("SELECT * FROM z_reports ORDER BY zReportNumber DESC LIMIT 1")
    suspend fun getLastZReport(): ZReportEntity?

    @Query("SELECT * FROM z_reports WHERE zReportNumber = :number LIMIT 1")
    suspend fun getZReportByNumber(number: Long): ZReportEntity?

    @Query("SELECT COALESCE(MAX(zReportNumber), 0) FROM z_reports")
    suspend fun getLastZReportNumber(): Long

    /**
     * Assigns the next Z-report number atomically (see TransactionDao.insertWithNextInvoiceNumber
     * for why this needs to be one Room transaction, and why [floorZReportNumber] — this
     * device's `lastZreportRptNo` from eTIMS registration — matters after a reinstall).
     */
    @Transaction
    suspend fun insertWithNextZReportNumber(
        floorZReportNumber: Long = 0L,
        buildEntity: suspend (Long) -> ZReportEntity
    ): ZReportEntity {
        val nextNumber = maxOf(getLastZReportNumber(), floorZReportNumber) + 1L
        val entity = buildEntity(nextNumber)
        insertZReport(entity)
        return entity
    }
}
