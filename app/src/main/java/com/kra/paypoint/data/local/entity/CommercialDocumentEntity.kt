package com.kra.paypoint.data.local.entity

import androidx.room.Embedded
import androidx.room.Entity
import androidx.room.ForeignKey
import androidx.room.Index
import androidx.room.PrimaryKey
import androidx.room.Relation

/**
 * A non-fiscal commercial document: a quotation or proforma to a customer, or a local
 * purchase order (LPO) to a supplier. None of these are reported to KRA — they become fiscal
 * only once converted into a sales invoice or a purchase invoice.
 */
@Entity(
    tableName = "commercial_documents",
    indices = [
        Index(value = ["docType", "sequence"], unique = true),
        Index(value = ["partyName"])
    ]
)
data class CommercialDocumentEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val docType: String, // see DocumentType
    val sequence: Long,
    val partyTin: String? = null,
    val partyName: String,
    val partyPhone: String? = null,
    val docDate: String, // yyyyMMdd
    val dueDate: String? = null, // yyyyMMdd
    val note: String? = null,
    val status: String = "OPEN", // OPEN, CONVERTED, CANCELLED
    val convertedReference: String? = null, // invoice / purchase number it became
    val totalAmount: Double = 0.0,
    val totalTaxAmount: Double = 0.0,
    val totalDiscount: Double = 0.0,
    val createdBy: String = "",
    val createdAt: Long = System.currentTimeMillis()
) {
    val displayNumber: String get() = DocumentType.prefix(docType) + sequence.toString().padStart(6, '0')
}

object DocumentType {
    const val QUOTATION = "QUOTATION"
    const val PROFORMA = "PROFORMA"
    const val LPO = "LPO"

    fun prefix(type: String): String = when (type) {
        QUOTATION -> "QUO"
        PROFORMA -> "PRO"
        LPO -> "LPO"
        else -> "DOC"
    }

    fun label(type: String): String = when (type) {
        QUOTATION -> "Quotation"
        PROFORMA -> "Proforma Invoice"
        LPO -> "Local Purchase Order"
        else -> type
    }
}

@Entity(
    tableName = "commercial_document_items",
    foreignKeys = [
        ForeignKey(
            entity = CommercialDocumentEntity::class,
            parentColumns = ["id"],
            childColumns = ["documentId"],
            onDelete = ForeignKey.CASCADE
        )
    ],
    indices = [Index(value = ["documentId"])]
)
data class CommercialDocumentItemEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val documentId: Long,
    val itemSequence: Int,
    val itemCode: String,
    val itemName: String,
    val quantity: Double,
    val unitPrice: Double,
    val discountAmount: Double = 0.0,
    val taxTypeCode: String,
    val taxAmount: Double = 0.0,
    val totalAmount: Double
)

data class CommercialDocumentWithItems(
    @Embedded val document: CommercialDocumentEntity,
    @Relation(parentColumn = "id", entityColumn = "documentId")
    val items: List<CommercialDocumentItemEntity>
)
