package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import androidx.room.Transaction
import com.kra.paypoint.data.local.entity.CommercialDocumentEntity
import com.kra.paypoint.data.local.entity.CommercialDocumentItemEntity
import com.kra.paypoint.data.local.entity.CommercialDocumentWithItems
import kotlinx.coroutines.flow.Flow

@Dao
interface CommercialDocumentDao {
    @Insert
    suspend fun insertDocument(document: CommercialDocumentEntity): Long

    @Insert
    suspend fun insertItems(items: List<CommercialDocumentItemEntity>)

    @Query("DELETE FROM commercial_document_items WHERE documentId = :documentId")
    suspend fun deleteItems(documentId: Long)

    @Query("SELECT MAX(sequence) FROM commercial_documents WHERE docType = :docType")
    suspend fun getLastSequence(docType: String): Long?

    /** Numbers each document type independently (QUO000001, PRO000001, LPO000001…), race-free. */
    @Transaction
    suspend fun insertWithNextSequence(
        build: (sequence: Long) -> CommercialDocumentEntity,
        items: List<CommercialDocumentItemEntity>
    ): Long {
        val draft = build(0L)
        val sequence = (getLastSequence(draft.docType) ?: 0L) + 1
        val id = insertDocument(build(sequence))
        insertItems(items.map { it.copy(documentId = id) })
        return id
    }

    @Query("UPDATE commercial_documents SET partyTin = :partyTin, partyName = :partyName, partyPhone = :partyPhone, dueDate = :dueDate, note = :note, totalAmount = :totalAmount, totalTaxAmount = :totalTaxAmount, totalDiscount = :totalDiscount WHERE id = :id AND status = 'OPEN'")
    suspend fun updateHeader(
        id: Long, partyTin: String?, partyName: String, partyPhone: String?, dueDate: String?, note: String?,
        totalAmount: Double, totalTaxAmount: Double, totalDiscount: Double
    ): Int

    /** Replaces an OPEN document's header and lines; converted/cancelled documents are frozen. */
    @Transaction
    suspend fun replaceOpenDocument(document: CommercialDocumentEntity, items: List<CommercialDocumentItemEntity>): Boolean {
        val updated = updateHeader(
            document.id, document.partyTin, document.partyName, document.partyPhone, document.dueDate, document.note,
            document.totalAmount, document.totalTaxAmount, document.totalDiscount
        )
        if (updated == 0) return false
        deleteItems(document.id)
        insertItems(items.map { it.copy(documentId = document.id) })
        return true
    }

    @Query("SELECT * FROM commercial_documents WHERE docType = :docType ORDER BY createdAt DESC")
    fun observeByType(docType: String): Flow<List<CommercialDocumentEntity>>

    @Transaction
    @Query("SELECT * FROM commercial_documents WHERE id = :id LIMIT 1")
    suspend fun getWithItems(id: Long): CommercialDocumentWithItems?

    @Query("UPDATE commercial_documents SET status = :status, convertedReference = :reference WHERE id = :id")
    suspend fun updateStatus(id: Long, status: String, reference: String?)
}
