package com.kra.paypoint.data.local.entity

import androidx.room.Embedded
import androidx.room.Entity
import androidx.room.ForeignKey
import androidx.room.Index
import androidx.room.PrimaryKey
import androidx.room.Relation

/** A supplier's invoice recorded by this taxpayer and reported to eTIMS (`insertTrnsPurchase`). */
@Entity(
    tableName = "purchases",
    indices = [
        Index(value = ["purchaseNumber"], unique = true),
        Index(value = ["syncStatus"]),
        Index(value = ["supplierTin"])
    ]
)
data class PurchaseEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val purchaseNumber: Long,
    val supplierTin: String,
    val supplierName: String,
    val supplierInvoiceNumber: String,
    val purchaseDate: String, // yyyyMMdd
    val paymentTypeCode: String = "01",
    val note: String? = null,
    val totalTaxableAmount: Double = 0.0,
    val totalTaxAmount: Double = 0.0,
    val totalAmount: Double = 0.0,
    val restockedInventory: Boolean = true,
    val sourceLpoId: Long? = null,
    val syncStatus: String = "PENDING", // PENDING, SYNCED, FAILED
    val syncAttempts: Int = 0,
    val lastError: String? = null,
    val payloadJson: String,
    val createdBy: String = "",
    val createdAt: Long = System.currentTimeMillis()
)

@Entity(
    tableName = "purchase_items",
    foreignKeys = [
        ForeignKey(
            entity = PurchaseEntity::class,
            parentColumns = ["id"],
            childColumns = ["purchaseId"],
            onDelete = ForeignKey.CASCADE
        )
    ],
    indices = [Index(value = ["purchaseId"]), Index(value = ["itemCode"])]
)
data class PurchaseItemEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val purchaseId: Long,
    val itemSequence: Int,
    val itemCode: String,
    val itemName: String,
    val quantity: Double,
    val unitPrice: Double,
    val supplyAmount: Double,
    val discountAmount: Double = 0.0,
    val taxTypeCode: String,
    val taxableAmount: Double,
    val taxAmount: Double,
    val totalAmount: Double
)

data class PurchaseWithItems(
    @Embedded val purchase: PurchaseEntity,
    @Relation(parentColumn = "id", entityColumn = "purchaseId")
    val items: List<PurchaseItemEntity>
)
