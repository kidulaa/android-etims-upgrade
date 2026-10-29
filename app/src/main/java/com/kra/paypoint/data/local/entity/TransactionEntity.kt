package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.Index
import androidx.room.PrimaryKey

@Entity(
    tableName = "transactions",
    indices = [
        Index(value = ["invoiceNumber"], unique = true),
        Index(value = ["syncStatus"]),
        Index(value = ["salesDate"]),
        Index(value = ["customerTin"])
    ]
)
data class TransactionEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val invoiceNumber: Long,
    val originalInvoiceNumber: Long = 0L,
    val customerTin: String? = null,
    val customerName: String? = null,
    val salesTypeCode: String = "N", // N: Normal
    val receiptTypeCode: String = "S", // S: Sale, R: Credit Note
    val paymentTypeCode: String = "01", // See EtimsCodes.PaymentType
    /** False for a credit sale until "Mark as paid"; always true for immediate payment. */
    val isPaid: Boolean = true,
    val dueDate: String? = null, // yyyyMMdd, credit sales only
    val note: String? = null,
    val totalItemCount: Double = 0.0,
    val taxableAmountA: Double = 0.0,
    val taxableAmountB: Double = 0.0,
    val taxableAmountC: Double = 0.0,
    val taxableAmountD: Double = 0.0,
    val taxableAmountE: Double = 0.0,
    val taxAmountA: Double = 0.0,
    val taxAmountB: Double = 0.0,
    val taxAmountC: Double = 0.0,
    val taxAmountD: Double = 0.0,
    val taxAmountE: Double = 0.0,
    val totalTaxableAmount: Double = 0.0,
    val totalTaxAmount: Double = 0.0,
    val totalAmount: Double = 0.0,
    val salesDate: String, // yyyyMMddHHmmss
    val receiptUrl: String? = null,
    val qrCodeData: String? = null,
    val syncStatus: String = "PENDING", // "PENDING", "SYNCING", "SYNCED", "FAILED"
    val syncAttempts: Int = 0,
    val lastError: String? = null,
    val payloadJson: String, // Serialized TrnsSalesSaveReq JSON
    val createdAt: Long = System.currentTimeMillis(),
    val syncedAt: Long? = null
)

/**
 * A credit note always references the invoice it reverses. Keyed off that rather than the
 * type codes because rows written before the KRA code fix stored sales as rcptTyCd "R" and
 * credit notes as salesTyCd "C".
 */
val TransactionEntity.isCreditNote: Boolean get() = originalInvoiceNumber > 0L
