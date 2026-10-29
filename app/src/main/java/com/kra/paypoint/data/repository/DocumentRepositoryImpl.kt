package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.CommercialDocumentDao
import com.kra.paypoint.data.local.entity.CommercialDocumentEntity
import com.kra.paypoint.data.local.entity.CommercialDocumentItemEntity
import com.kra.paypoint.data.local.entity.CommercialDocumentWithItems
import com.kra.paypoint.domain.model.DocumentLine
import com.kra.paypoint.domain.model.PricedLine
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.DocumentParty
import com.kra.paypoint.domain.repository.DocumentRepository
import kotlinx.coroutines.flow.Flow
import java.time.LocalDate
import java.time.format.DateTimeFormatter
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class DocumentRepositoryImpl @Inject constructor(
    private val documentDao: CommercialDocumentDao
) : DocumentRepository {

    override fun observe(docType: String): Flow<List<CommercialDocumentEntity>> = documentDao.observeByType(docType)

    override suspend fun get(id: Long): CommercialDocumentWithItems? = documentDao.getWithItems(id)

    override suspend fun save(
        docType: String,
        existingId: Long?,
        party: DocumentParty,
        dueDate: String?,
        note: String?,
        lines: List<DocumentLine>,
        operator: User
    ): Result<Long> = runCatching {
        require(party.name.isNotBlank()) { "Select or enter who this document is for." }
        require(lines.isNotEmpty()) { "Add at least one item." }
        lines.forEach { l ->
            require(l.line.quantity > 0.0) { "Quantity for ${l.itemName} must be greater than zero." }
            PricedLine.validateDiscount(l.line.discount, l.line.supplyAmount)?.let { throw IllegalArgumentException(it) }
        }

        val items = lines.mapIndexed { idx, l ->
            CommercialDocumentItemEntity(
                documentId = 0L,
                itemSequence = idx + 1,
                itemCode = l.itemCode,
                itemName = l.itemName,
                quantity = l.line.quantity,
                unitPrice = l.line.unitPrice,
                discountAmount = l.line.discountAmount,
                taxTypeCode = l.line.taxTypeCode,
                taxAmount = l.line.taxAmount,
                totalAmount = l.line.lineTotal
            )
        }
        val header = CommercialDocumentEntity(
            id = existingId ?: 0L,
            docType = docType,
            sequence = 0L,
            partyTin = party.tin?.trim()?.uppercase()?.takeIf { it.isNotEmpty() },
            partyName = party.name.trim(),
            partyPhone = party.phone?.trim()?.takeIf { it.isNotEmpty() },
            docDate = LocalDate.now().format(DateTimeFormatter.BASIC_ISO_DATE),
            dueDate = dueDate,
            note = note?.trim()?.takeIf { it.isNotEmpty() },
            totalAmount = PricedLine.round2(lines.sumOf { it.line.lineTotal }),
            totalTaxAmount = PricedLine.round2(lines.sumOf { it.line.taxAmount }),
            totalDiscount = PricedLine.round2(lines.sumOf { it.line.discountAmount }),
            createdBy = operator.name
        )

        if (existingId != null) {
            check(documentDao.replaceOpenDocument(header, items)) { "Only open documents can be edited." }
            existingId
        } else {
            documentDao.insertWithNextSequence(build = { seq -> header.copy(sequence = seq) }, items = items)
        }
    }

    override suspend fun markConverted(id: Long, reference: String) {
        documentDao.updateStatus(id, STATUS_CONVERTED, reference)
    }

    override suspend fun cancel(id: Long) {
        documentDao.updateStatus(id, STATUS_CANCELLED, null)
    }

    companion object {
        const val STATUS_OPEN = "OPEN"
        const val STATUS_CONVERTED = "CONVERTED"
        const val STATUS_CANCELLED = "CANCELLED"
    }
}
