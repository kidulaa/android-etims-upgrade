package com.kra.paypoint.domain.repository

import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import kotlinx.coroutines.flow.Flow

data class RefundItemParam(
    val itemCode: String,
    val itemClassificationCode: String = "5020",
    val itemName: String,
    val barcode: String? = null,
    val packagingUnitCode: String = "NT",
    val packageQuantity: Double = 1.0,
    val quantityUnitCode: String = "U",
    val quantity: Double,
    val unitPrice: Double,
    val taxRateClassificationCode: String = "B",
    val totalAmount: Double = quantity * unitPrice
)

/** Sale details that live only on this device (not in the KRA payload). */
data class SaleExtras(
    /** False for a credit sale ("pay later") until it's marked paid. */
    val isPaid: Boolean = true,
    val dueDate: String? = null, // yyyyMMdd
    val note: String? = null
)

interface TransactionRepository {
    fun getPendingTransactions(): Flow<List<TransactionEntity>>
    fun getAllTransactions(): Flow<List<TransactionEntity>>
    fun getPendingCount(): Flow<Int>
    suspend fun getTransactionDetails(id: Long): TransactionWithItems?
    suspend fun getTransactionByInvoiceNumber(invoiceNumber: Long): TransactionWithItems?
    /** Returns the locally-assigned (transactionId, invoiceNumber) pair for the recorded sale. */
    suspend fun processSale(
        request: TrnsSalesSaveReq,
        lineItems: List<TransactionItemEntity>,
        extras: SaleExtras = SaleExtras()
    ): Pair<Long, Long>

    /** Settles a credit sale. */
    suspend fun markPaid(transactionId: Long)
    suspend fun issueCreditNote(
        originalTransaction: TransactionEntity,
        itemsToRefund: List<RefundItemParam>,
        reasonCode: String,
        reasonDescription: String,
        restockInventory: Boolean,
        operatorId: String = "SYSTEM",
        operatorName: String = "Cashier"
    ): Result<Long>
    suspend fun triggerSync()
}

