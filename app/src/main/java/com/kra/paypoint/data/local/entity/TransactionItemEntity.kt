package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.ForeignKey
import androidx.room.Index
import androidx.room.PrimaryKey

@Entity(
    tableName = "transaction_items",
    foreignKeys = [
        ForeignKey(
            entity = TransactionEntity::class,
            parentColumns = ["id"],
            childColumns = ["transactionId"],
            onDelete = ForeignKey.CASCADE
        )
    ],
    indices = [
        Index(value = ["transactionId"]),
        Index(value = ["itemCode"])
    ]
)
data class TransactionItemEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val transactionId: Long,
    val itemSequence: Int,
    val itemCode: String,
    val itemClassificationCode: String,
    val itemName: String,
    val barcode: String? = null,
    val packagingUnitCode: String = "NT",
    val packageQuantity: Double = 1.0,
    val quantityUnitCode: String = "U",
    val quantity: Double,
    val unitPrice: Double,
    val supplyAmount: Double,
    val discountRate: Double = 0.0,
    val discountAmount: Double = 0.0,
    val taxTypeCode: String,
    val taxableAmount: Double,
    val taxAmount: Double,
    val totalAmount: Double
)
