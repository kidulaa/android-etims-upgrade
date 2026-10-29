package com.kra.paypoint.domain.repository

import com.kra.paypoint.data.local.entity.PurchaseEntity
import com.kra.paypoint.data.local.entity.PurchaseWithItems
import com.kra.paypoint.data.local.entity.SupplierEntity
import com.kra.paypoint.domain.model.DocumentLine
import com.kra.paypoint.domain.model.auth.User
import kotlinx.coroutines.flow.Flow

data class PurchaseRequest(
    val supplier: SupplierEntity,
    val supplierInvoiceNumber: String,
    val purchaseDate: String, // yyyyMMdd
    val paymentTypeCode: String,
    val note: String?,
    val lines: List<DocumentLine>,
    val restockInventory: Boolean,
    val sourceLpoId: Long? = null
)

interface PurchaseRepository {
    fun observeSuppliers(): Flow<List<SupplierEntity>>
    fun searchSuppliers(query: String): Flow<List<SupplierEntity>>
    suspend fun saveSupplier(supplier: SupplierEntity): Result<Unit>
    suspend fun observeSuppliersOnce(tin: String): SupplierEntity?

    fun observePurchases(): Flow<List<PurchaseEntity>>
    suspend fun getPurchase(id: Long): PurchaseWithItems?

    /**
     * Records a supplier's invoice under the next purchase number, books goods into stock when
     * [PurchaseRequest.restockInventory], and queues it for eTIMS. Returns the purchase id.
     */
    suspend fun recordPurchase(request: PurchaseRequest, operator: User): Result<Long>

    /** Sends PENDING/FAILED purchases to eTIMS; returns how many were accepted. */
    suspend fun pushPendingPurchases(): Int
}
