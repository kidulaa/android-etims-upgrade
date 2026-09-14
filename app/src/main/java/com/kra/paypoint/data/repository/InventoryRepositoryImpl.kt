package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.dao.StockMovementDao
import com.kra.paypoint.data.local.database.TransactionRunner
import com.kra.paypoint.data.local.entity.StockMovementEntity
import com.kra.paypoint.domain.exception.InsufficientStockException
import com.kra.paypoint.domain.exception.ItemNotFoundException
import com.kra.paypoint.domain.repository.InventoryRepository
import kotlinx.coroutines.flow.Flow
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class InventoryRepositoryImpl @Inject constructor(
    private val transactionRunner: TransactionRunner,
    private val itemDao: ItemDao,
    private val stockMovementDao: StockMovementDao
) : InventoryRepository {

    override fun getAllMovements(): Flow<List<StockMovementEntity>> {
        return stockMovementDao.getAllMovements()
    }

    override suspend fun recordStockIn(
        itemCode: String,
        quantity: Double,
        reasonCode: String,
        reasonDescription: String,
        remark: String?,
        operatorId: String
    ): Result<Double> {
        if (quantity <= 0.0) {
            return Result.failure(IllegalArgumentException("Quantity must be greater than zero."))
        }

        return runCatching {
            transactionRunner.run {
                val item = itemDao.getItemByCode(itemCode)
                    ?: throw ItemNotFoundException(itemCode)

                val previousStock = item.stockQuantity
                itemDao.increaseStock(itemCode, quantity)
                val newStock = previousStock + quantity

                stockMovementDao.insertMovement(
                    StockMovementEntity(
                        itemCode = itemCode,
                        itemName = item.itemName,
                        movementType = "STOCK_IN",
                        quantity = quantity,
                        previousStock = previousStock,
                        newStock = newStock,
                        reasonCode = reasonCode,
                        reasonDescription = reasonDescription,
                        remark = remark,
                        operatorId = operatorId
                    )
                )
                newStock
            }
        }
    }

    override suspend fun recordStockOut(
        itemCode: String,
        quantity: Double,
        reasonCode: String,
        reasonDescription: String,
        remark: String?,
        operatorId: String
    ): Result<Double> {
        if (quantity <= 0.0) {
            return Result.failure(IllegalArgumentException("Quantity must be greater than zero."))
        }

        return runCatching {
            transactionRunner.run {
                val item = itemDao.getItemByCode(itemCode)
                    ?: throw ItemNotFoundException(itemCode)

                // Conditional on the live stockQuantity value in the same statement, so a
                // second concurrent stock-out can't both read "enough stock" and both
                // succeed, driving the count negative.
                val rowsUpdated = itemDao.decreaseStockIfSufficient(itemCode, quantity)
                if (rowsUpdated == 0) {
                    throw InsufficientStockException(
                        itemCode = itemCode,
                        requested = quantity,
                        available = item.stockQuantity
                    )
                }

                val previousStock = item.stockQuantity
                val newStock = previousStock - quantity

                stockMovementDao.insertMovement(
                    StockMovementEntity(
                        itemCode = itemCode,
                        itemName = item.itemName,
                        movementType = "STOCK_OUT",
                        quantity = quantity,
                        previousStock = previousStock,
                        newStock = newStock,
                        reasonCode = reasonCode,
                        reasonDescription = reasonDescription,
                        remark = remark,
                        operatorId = operatorId
                    )
                )
                newStock
            }
        }
    }

    override suspend fun recordAdjustment(
        itemCode: String,
        physicalCount: Double,
        remark: String?,
        operatorId: String
    ): Result<Double> {
        if (physicalCount < 0.0) {
            return Result.failure(IllegalArgumentException("Physical inventory count cannot be negative."))
        }

        return runCatching {
            transactionRunner.run {
                val item = itemDao.getItemByCode(itemCode)
                    ?: throw ItemNotFoundException(itemCode)

                val previousStock = item.stockQuantity
                itemDao.setStockQuantity(itemCode, physicalCount)

                stockMovementDao.insertMovement(
                    StockMovementEntity(
                        itemCode = itemCode,
                        itemName = item.itemName,
                        movementType = "ADJUSTMENT",
                        quantity = physicalCount,
                        previousStock = previousStock,
                        newStock = physicalCount,
                        reasonCode = "04",
                        reasonDescription = "Physical Recount",
                        remark = remark,
                        operatorId = operatorId
                    )
                )
                physicalCount
            }
        }
    }
}
