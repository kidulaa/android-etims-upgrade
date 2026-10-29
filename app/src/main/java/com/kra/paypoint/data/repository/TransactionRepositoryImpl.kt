package com.kra.paypoint.data.repository

import android.content.Context
import com.google.gson.Gson
import com.kra.paypoint.data.local.dao.TransactionDao
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.TransactionWithItems
import com.kra.paypoint.domain.exception.EmptyTransactionException
import com.kra.paypoint.domain.exception.InvalidOriginalTransactionException
import com.kra.paypoint.domain.model.EtimsCodes
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveItem
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.InventoryRepository
import com.kra.paypoint.domain.repository.RefundItemParam
import com.kra.paypoint.domain.repository.SaleExtras
import com.kra.paypoint.domain.repository.SignedReceipt
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.domain.usecase.TaxCalculationEngine
import com.kra.paypoint.worker.SyncManager
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.Flow
import android.util.Log
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class TransactionRepositoryImpl @Inject constructor(
    private val transactionDao: TransactionDao,
    private val inventoryRepository: InventoryRepository,
    private val deviceRepository: DeviceRepository,
    private val gson: Gson,
    @ApplicationContext private val context: Context
) : TransactionRepository {

    /**
     * Signs the receipt if this device has completed eTIMS registration; otherwise proceeds
     * unsigned rather than blocking the sale outright — offline-first means a till without a
     * live KRA connection yet must still be able to record a sale locally. An unsigned
     * PENDING transaction is a visible, honest state; silently fabricating a signature would
     * not be.
     */
    private suspend fun trySign(receiptData: String): SignedReceipt? =
        deviceRepository.signReceipt(receiptData).getOrNull()

    /**
     * A failure to *schedule* a background sync attempt (e.g. WorkManager unavailable) must
     * never fail a sale that's already durably recorded in the local DB -- the periodic
     * 15-minute sync will still pick it up. Log and move on.
     */
    private fun safeTriggerSync() {
        try {
            SyncManager.triggerImmediateSync(context)
        } catch (e: Exception) {
            Log.w("TransactionRepository", "Could not schedule an immediate sync attempt: ${e.message}")
        }
    }

    override fun getPendingTransactions(): Flow<List<TransactionEntity>> {
        return transactionDao.getPendingTransactions()
    }

    override fun getAllTransactions(): Flow<List<TransactionEntity>> {
        return transactionDao.getAllTransactions()
    }

    override fun getPendingCount(): Flow<Int> {
        return transactionDao.getPendingCount()
    }

    override suspend fun getTransactionDetails(id: Long): TransactionWithItems? {
        return transactionDao.getTransactionWithItems(id)
    }

    override suspend fun processSale(
        request: TrnsSalesSaveReq,
        lineItems: List<TransactionItemEntity>,
        extras: SaleExtras
    ): Pair<Long, Long> {
        // The invoice number is assigned atomically inside the DAO transaction (last local
        // number + 1, mirroring the legacy till's sequencing) rather than by the caller, so
        // two near-simultaneous checkouts on this device can never compute the same number.
        // floorInvoiceNumber guards a reinstalled/empty-DB device against reusing a number
        // already sent to KRA under a prior install.
        val result = transactionDao.insertWithNextInvoiceNumber(
            floorInvoiceNumber = deviceRepository.registration.value?.lastSaleInvoiceNumber ?: 0L,
            buildEntity = { invoiceNumber ->
                val receiptData = "${request.tin}|${request.bhfId}|$invoiceNumber|${request.salesDt}|${request.totAmt}"
                val signed = trySign(receiptData)
                val finalRequest = request.copy(
                    invcNo = invoiceNumber,
                    rcptSign = signed?.receiptSignature,
                    intrlData = signed?.internalData
                )
                TransactionEntity(
                    invoiceNumber = invoiceNumber,
                    originalInvoiceNumber = finalRequest.orgInvcNo,
                    customerTin = finalRequest.custTin,
                    customerName = finalRequest.custNm,
                    salesTypeCode = finalRequest.salesTyCd,
                    receiptTypeCode = finalRequest.rcptTyCd,
                    paymentTypeCode = finalRequest.pmtTyCd,
                    isPaid = extras.isPaid,
                    dueDate = extras.dueDate,
                    note = extras.note,
                    totalItemCount = finalRequest.totItemCnt,
                    taxableAmountA = finalRequest.taxblAmtA,
                    taxableAmountB = finalRequest.taxblAmtB,
                    taxableAmountC = finalRequest.taxblAmtC,
                    taxableAmountD = finalRequest.taxblAmtD,
                    taxableAmountE = finalRequest.taxblAmtE,
                    taxAmountA = finalRequest.taxAmtA,
                    taxAmountB = finalRequest.taxAmtB,
                    taxAmountC = finalRequest.taxAmtC,
                    taxAmountD = finalRequest.taxAmtD,
                    taxAmountE = finalRequest.taxAmtE,
                    totalTaxableAmount = finalRequest.totTaxblAmt,
                    totalTaxAmount = finalRequest.totTaxAmt,
                    totalAmount = finalRequest.totAmt,
                    salesDate = finalRequest.salesDt,
                    syncStatus = "PENDING",
                    // QR payload = TIN + branch + signature, matching the URL pattern the
                    // legacy printer heads embedded on the fiscal receipt.
                    qrCodeData = signed?.let { "${finalRequest.tin}${finalRequest.bhfId}${it.receiptSignature}" },
                    payloadJson = gson.toJson(finalRequest),
                    createdAt = System.currentTimeMillis()
                )
            },
            buildItems = { transactionId -> lineItems.map { it.copy(transactionId = transactionId) } }
        )

        deductSoldStock(lineItems, result.invoiceNumber, request.regrId ?: "SYSTEM")
        safeTriggerSync()
        return result.transactionId to result.invoiceNumber
    }

    /**
     * The sale is already a durable fiscal record by now, so stock is best-effort: services
     * carry no stock and a till may legitimately sell ahead of a delivery being booked —
     * neither may undo or block an issued invoice.
     */
    private suspend fun deductSoldStock(lineItems: List<TransactionItemEntity>, invoiceNumber: Long, operatorId: String) {
        for (line in lineItems) {
            runCatching {
                inventoryRepository.recordStockOut(
                    itemCode = line.itemCode,
                    quantity = line.quantity,
                    reasonCode = "11",
                    reasonDescription = "Sale (Inv #$invoiceNumber)",
                    operatorId = operatorId
                ).getOrThrow()
            }.onFailure { Log.i("TransactionRepository", "No stock deducted for ${line.itemCode}: ${it.message}") }
        }
    }

    override suspend fun markPaid(transactionId: Long) {
        transactionDao.markPaid(transactionId)
    }

    override suspend fun getTransactionByInvoiceNumber(invoiceNumber: Long): TransactionWithItems? {
        return transactionDao.getTransactionByInvoiceNumber(invoiceNumber)
    }

    override suspend fun issueCreditNote(
        originalTransaction: TransactionEntity,
        itemsToRefund: List<RefundItemParam>,
        reasonCode: String,
        reasonDescription: String,
        restockInventory: Boolean,
        operatorId: String,
        operatorName: String
    ): Result<Long> {
        return try {
            if (itemsToRefund.isEmpty()) {
                return Result.failure(EmptyTransactionException("Cannot issue Credit Note with zero items"))
            }

            val now = LocalDateTime.now()
            val dateStr = now.format(DateTimeFormatter.ofPattern("yyyyMMddHHmmss"))
            val salesDt = dateStr.substring(0, 8)

            val bands = TaxCalculationEngine.bandTotals(
                itemsToRefund.map {
                    val split = TaxCalculationEngine.split(it.totalAmount, it.taxRateClassificationCode)
                    Triple(it.taxRateClassificationCode, split.taxableAmount.toDouble(), split.taxAmount.toDouble())
                }
            )
            val totTaxblAmt = bands.totalTaxable
            val totTaxAmt = bands.totalTax
            val totAmt = itemsToRefund.sumOf { it.totalAmount }
            val totItemCnt = itemsToRefund.sumOf { it.quantity }

            val remoteItems = itemsToRefund.mapIndexed { idx, item ->
                val split = TaxCalculationEngine.split(item.totalAmount, item.taxRateClassificationCode)
                TrnsSalesSaveItem(
                    itemSeq = idx + 1,
                    itemCd = item.itemCode,
                    itemClsCd = item.itemClassificationCode,
                    itemNm = item.itemName,
                    bcd = item.barcode,
                    pkgUnitCd = item.packagingUnitCode,
                    pkg = item.packageQuantity,
                    qtyUnitCd = item.quantityUnitCode,
                    qty = item.quantity,
                    prc = item.unitPrice,
                    splyAmt = split.taxableAmount.toDouble(),
                    dcRt = 0.0,
                    dcAmt = 0.0,
                    isrccCd = null,
                    isrccNm = null,
                    isrcRt = 0.0,
                    isrcAmt = 0.0,
                    taxTyCd = item.taxRateClassificationCode,
                    taxblAmt = split.taxableAmount.toDouble(),
                    taxAmt = split.taxAmount.toDouble(),
                    totAmt = item.totalAmount
                )
            }

            // orgInvcNo/invcNo=0L here are placeholders finalized once insertWithNextInvoiceNumber
            // assigns the real, atomically-sequenced number below.
            // The seller on a credit note is the same taxpayer/branch that issued the original
            // invoice — never the customer — so take it from the payload filed with KRA, falling
            // back to this device's registration. Gson ignores Kotlin nullability, hence String?.
            val originalReq = runCatching { gson.fromJson(originalTransaction.payloadJson, TrnsSalesSaveReq::class.java) }.getOrNull()
            val originalTin: String? = originalReq?.tin
            val originalBhfId: String? = originalReq?.bhfId
            val registration = deviceRepository.registration.value
            val sellerTin = originalTin?.takeIf { it.isNotBlank() } ?: registration?.tin
                ?: throw IllegalStateException("Cannot determine the seller PIN for invoice #${originalTransaction.invoiceNumber}.")
            val sellerBhfId = originalBhfId?.takeIf { it.isNotBlank() } ?: registration?.branchId ?: "00"
            val baseReq = TrnsSalesSaveReq(
                tin = sellerTin,
                bhfId = sellerBhfId,
                invcNo = 0L,
                orgInvcNo = originalTransaction.invoiceNumber,
                custTin = originalTransaction.customerTin,
                custNm = originalTransaction.customerName ?: "Walk-in Customer",
                salesTyCd = EtimsCodes.SalesType.NORMAL,
                rcptTyCd = EtimsCodes.ReceiptType.CREDIT_NOTE,
                pmtTyCd = originalTransaction.paymentTypeCode,
                rfdRsnCd = reasonCode,
                salesSttsCd = "02",
                cfmDt = dateStr,
                salesDt = salesDt,
                stockRlsDt = dateStr,
                cnclReqDt = null,
                cnclDt = null,
                rfdDt = dateStr,
                totItemCnt = totItemCnt,
                taxblAmtA = bands.taxableFor("A"),
                taxblAmtB = bands.taxableFor("B"),
                taxblAmtC = bands.taxableFor("C"),
                taxblAmtD = bands.taxableFor("D"),
                taxblAmtE = bands.taxableFor("E"),
                taxRtA = TaxCalculationEngine.ratePercentFor("A"),
                taxRtB = TaxCalculationEngine.ratePercentFor("B"),
                taxRtC = TaxCalculationEngine.ratePercentFor("C"),
                taxRtD = TaxCalculationEngine.ratePercentFor("D"),
                taxRtE = TaxCalculationEngine.ratePercentFor("E"),
                taxAmtA = bands.taxFor("A"),
                taxAmtB = bands.taxFor("B"),
                taxAmtC = bands.taxFor("C"),
                taxAmtD = bands.taxFor("D"),
                taxAmtE = bands.taxFor("E"),
                totTaxblAmt = totTaxblAmt,
                totTaxAmt = totTaxAmt,
                totAmt = totAmt,
                prchrAcptcYn = "N",
                remark = "Refund for Inv #${originalTransaction.invoiceNumber} - $reasonDescription",
                regrId = operatorId,
                regrNm = operatorName,
                modrId = operatorId,
                modrNm = operatorName,
                itemList = remoteItems
            )

            val lineItemEntities = itemsToRefund.mapIndexed { idx, item ->
                val split = TaxCalculationEngine.split(item.totalAmount, item.taxRateClassificationCode)
                TransactionItemEntity(
                    transactionId = 0L,
                    itemSequence = idx + 1,
                    itemCode = item.itemCode,
                    itemClassificationCode = item.itemClassificationCode,
                    itemName = item.itemName,
                    barcode = item.barcode,
                    packagingUnitCode = item.packagingUnitCode,
                    packageQuantity = item.packageQuantity,
                    quantityUnitCode = item.quantityUnitCode,
                    quantity = item.quantity,
                    unitPrice = item.unitPrice,
                    supplyAmount = split.taxableAmount.toDouble(),
                    discountRate = 0.0,
                    discountAmount = 0.0,
                    taxTypeCode = item.taxRateClassificationCode,
                    taxableAmount = split.taxableAmount.toDouble(),
                    taxAmount = split.taxAmount.toDouble(),
                    totalAmount = item.totalAmount
                )
            }

            val insertResult = transactionDao.insertWithNextInvoiceNumber(
                floorInvoiceNumber = deviceRepository.registration.value?.lastSaleInvoiceNumber ?: 0L,
                buildEntity = { invoiceNumber ->
                    val receiptData = "${baseReq.tin}|${baseReq.bhfId}|$invoiceNumber|${baseReq.salesDt}|${baseReq.totAmt}"
                    val signed = trySign(receiptData)
                    val finalReq = baseReq.copy(
                        invcNo = invoiceNumber,
                        rcptSign = signed?.receiptSignature,
                        intrlData = signed?.internalData
                    )
                    TransactionEntity(
                        invoiceNumber = invoiceNumber,
                        originalInvoiceNumber = originalTransaction.invoiceNumber,
                        customerTin = originalTransaction.customerTin,
                        customerName = originalTransaction.customerName,
                        salesTypeCode = EtimsCodes.SalesType.NORMAL,
                        receiptTypeCode = EtimsCodes.ReceiptType.CREDIT_NOTE,
                        paymentTypeCode = originalTransaction.paymentTypeCode,
                        totalItemCount = totItemCnt,
                        taxableAmountA = bands.taxableFor("A"),
                        taxableAmountB = bands.taxableFor("B"),
                        taxableAmountC = bands.taxableFor("C"),
                        taxableAmountD = bands.taxableFor("D"),
                        taxableAmountE = bands.taxableFor("E"),
                        taxAmountA = bands.taxFor("A"),
                        taxAmountB = bands.taxFor("B"),
                        taxAmountC = bands.taxFor("C"),
                        taxAmountD = bands.taxFor("D"),
                        taxAmountE = bands.taxFor("E"),
                        totalTaxableAmount = totTaxblAmt,
                        totalTaxAmount = totTaxAmt,
                        totalAmount = totAmt,
                        salesDate = dateStr,
                        syncStatus = "PENDING",
                        qrCodeData = signed?.let { "${finalReq.tin}${finalReq.bhfId}${it.receiptSignature}" },
                        payloadJson = gson.toJson(finalReq),
                        createdAt = System.currentTimeMillis()
                    )
                },
                buildItems = { transactionId -> lineItemEntities.map { it.copy(transactionId = transactionId) } }
            )

            if (restockInventory) {
                for (item in itemsToRefund) {
                    inventoryRepository.recordStockIn(
                        itemCode = item.itemCode,
                        quantity = item.quantity,
                        reasonCode = "01",
                        reasonDescription = "Refund Return (Inv #${originalTransaction.invoiceNumber})",
                        remark = "Credit Note #${insertResult.invoiceNumber} - $reasonDescription",
                        operatorId = operatorId
                    )
                }
            }

            safeTriggerSync()

            Result.success(insertResult.transactionId)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    override suspend fun triggerSync() {
        SyncManager.triggerImmediateSync(context)
    }
}
