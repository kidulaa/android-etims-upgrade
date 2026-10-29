package com.kra.paypoint.domain.repository

import com.kra.paypoint.data.local.entity.CommercialDocumentEntity
import com.kra.paypoint.data.local.entity.CommercialDocumentWithItems
import com.kra.paypoint.domain.model.DocumentLine
import com.kra.paypoint.domain.model.auth.User
import kotlinx.coroutines.flow.Flow

/** Who a quotation/proforma is for, or which supplier an LPO goes to. */
data class DocumentParty(val tin: String?, val name: String, val phone: String?)

/** Quotations, proforma invoices and LPOs — local, non-fiscal documents (see CommercialDocumentEntity). */
interface DocumentRepository {
    fun observe(docType: String): Flow<List<CommercialDocumentEntity>>
    suspend fun get(id: Long): CommercialDocumentWithItems?

    /** Creates a new numbered document, or replaces an OPEN one when [existingId] is set. Returns its id. */
    suspend fun save(
        docType: String,
        existingId: Long?,
        party: DocumentParty,
        dueDate: String?,
        note: String?,
        lines: List<DocumentLine>,
        operator: User
    ): Result<Long>

    /** Marks a document as turned into [reference] (an invoice or purchase number); it can't be edited afterwards. */
    suspend fun markConverted(id: Long, reference: String)

    suspend fun cancel(id: Long)
}
