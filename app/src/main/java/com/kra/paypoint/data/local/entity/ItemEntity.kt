package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.Index
import androidx.room.PrimaryKey

@Entity(
    tableName = "items",
    indices = [
        Index(value = ["itemCode"], unique = true),
        Index(value = ["barcode"]),
        Index(value = ["itemName"])
    ]
)
data class ItemEntity(
    @PrimaryKey
    val itemCode: String,
    val itemClassificationCode: String,
    val itemTypeCode: String,
    val itemName: String,
    val description: String? = null,
    val barcode: String? = null,
    val packagingUnitCode: String = "NT",
    val packageQuantity: Double = 1.0,
    val quantityUnitCode: String = "U",
    val taxTypeCode: String = "B", // A: Exempt, B: 16% Standard, C: 0% Zero-rated, D: Non-VAT, E: 8%
    val defaultUnitPrice: Double = 0.0,
    val stockQuantity: Double = 0.0,
    val originNationCode: String = "KE",
    val isUsed: Boolean = true,
    val syncStatus: String = "SYNCED", // "PENDING" until accepted by eTIMS saveItem
    val updatedAt: Long = System.currentTimeMillis()
)
