package com.kra.paypoint.domain.repository

import com.kra.paypoint.data.local.entity.StockMovementEntity
import kotlinx.coroutines.flow.Flow

interface InventoryRepository {
    fun getAllMovements(): Flow<List<StockMovementEntity>>

    suspend fun recordStockIn(
        itemCode: String,
        quantity: Double,
        reasonCode: String = "01",
        reasonDescription: String = "Purchase Delivery",
        remark: String? = null,
        operatorId: String = "SYSTEM"
    ): Result<Double>

    suspend fun recordStockOut(
        itemCode: String,
        quantity: Double,
        reasonCode: String = "02",
        reasonDescription: String = "Damaged Goods",
        remark: String? = null,
        operatorId: String = "SYSTEM"
    ): Result<Double>

    suspend fun recordAdjustment(
        itemCode: String,
        physicalCount: Double,
        remark: String? = null,
        operatorId: String = "SYSTEM"
    ): Result<Double>
}
