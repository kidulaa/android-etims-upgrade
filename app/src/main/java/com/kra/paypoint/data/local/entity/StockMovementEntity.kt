package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.Index
import androidx.room.PrimaryKey

@Entity(
    tableName = "stock_movements",
    indices = [
        Index(value = ["itemCode"]),
        Index(value = ["movementType"]),
        Index(value = ["timestamp"])
    ]
)
data class StockMovementEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val itemCode: String,
    val itemName: String,
    val movementType: String, // "STOCK_IN", "STOCK_OUT", "ADJUSTMENT"
    val quantity: Double,
    val previousStock: Double,
    val newStock: Double,
    val reasonCode: String = "01", // "01": Delivery/Intake, "02": Damage, "03": Expired, "04": Physical Recount
    val reasonDescription: String = "Purchase Delivery",
    val remark: String? = null,
    val operatorId: String = "SYSTEM",
    val timestamp: Long = System.currentTimeMillis(),
    val syncStatus: String = "PENDING"
)
