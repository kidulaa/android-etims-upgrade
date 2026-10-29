package com.kra.paypoint.data.repository

import com.kra.paypoint.domain.exception.toUserMessage
import android.util.Log
import com.google.gson.Gson
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.dao.PurchaseDao
import com.kra.paypoint.data.local.dao.SupplierDao
import com.kra.paypoint.data.local.entity.PurchaseEntity
import com.kra.paypoint.data.local.entity.PurchaseItemEntity
import com.kra.paypoint.data.local.entity.PurchaseWithItems
import com.kra.paypoint.data.local.entity.SupplierEntity
import com.kra.paypoint.data.remote.api.TransactionService
import com.kra.paypoint.data.remote.model.transaction.TrnsPurchaseItem
import com.kra.paypoint.data.remote.model.transaction.TrnsPurchaseSaveReq
import com.kra.paypoint.domain.model.EtimsCodes
import com.kra.paypoint.domain.model.EtimsCodes.TaxType
import com.kra.paypoint.domain.model.PricedLine
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.InventoryRepository
import com.kra.paypoint.domain.repository.PurchaseRepository
import com.kra.paypoint.domain.repository.PurchaseRequest
import com.kra.paypoint.domain.usecase.TaxCalculationEngine
import kotlinx.coroutines.flow.Flow
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class PurchaseRepositoryImpl @Inject constructor(
    private val supplierDao: SupplierDao,
    private val purchaseDao: PurchaseDao,
    private val itemDao: ItemDao,
    private val inventoryRepository: InventoryRepository,
    private val deviceRepository: DeviceRepository,
    private val transactionService: TransactionService,
    private val gson: Gson
) : PurchaseRepository {

    override fun observeSuppliers(): Flow<List<SupplierEntity>> = supplierDao.observeAll()

    override fun searchSuppliers(query: String): Flow<List<SupplierEntity>> = supplierDao.search(query)

    override suspend fun saveSupplier(supplier: SupplierEntity): Result<Unit> = runCatching {
        val pin = supplier.supplierTin.trim().uppercase()
        require(EtimsCodes.isValidKraPin(pin)) { "Enter the supplier's valid KRA PIN (e.g. P051234567Z)." }
        require(supplier.supplierName.isNotBlank()) { "Supplier name is required." }
        supplierDao.upsert(
            supplier.copy(
                supplierTin = pin,
                supplierName = supplier.supplierName.trim(),
                telNo = supplier.telNo?.trim()?.takeIf { it.isNotEmpty() },
                email = supplier.email?.trim()?.takeIf { it.isNotEmpty() },
                updatedAt = System.currentTimeMillis()
            )
        )
    }

    override suspend fun observeSuppliersOnce(tin: String): SupplierEntity? = supplierDao.getByTin(tin)

    override fun observePurchases(): Flow<List<PurchaseEntity>> = purchaseDao.observeAll()

    override suspend fun getPurchase(id: Long): PurchaseWithItems? = purchaseDao.getWithItems(id)

    override suspend fun recordPurchase(request: PurchaseRequest, operator: User): Result<Long> = runCatching {
        val supplierInvoice = request.supplierInvoiceNumber.trim()
        require(supplierInvoice.isNotEmpty()) { "Enter the supplier's invoice number." }
        require(request.lines.isNotEmpty()) { "Add at least one item." }
        require(purchaseDao.countBySupplierInvoice(request.supplier.supplierTin, supplierInvoice) == 0) {
            "Invoice $supplierInvoice from ${request.supplier.supplierName} is already recorded."
        }
        request.lines.forEach { l ->
            require(l.line.quantity > 0.0) { "Quantity for ${l.itemName} must be greater than zero." }
            PricedLine.validateDiscount(l.line.discount, l.line.supplyAmount)?.let { throw IllegalArgumentException(it) }
        }

        val items = request.lines.map { l ->
            itemDao.getItemByCode(l.itemCode) ?: throw IllegalArgumentException("${l.itemName} is not a registered item.")
        }
        val bands = TaxCalculationEngine.bandTotals(request.lines.map { Triple(it.line.taxTypeCode, it.line.taxableAmount, it.line.taxAmount) })
        val now = LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyyMMddHHmmss"))

        val remoteItems = request.lines.mapIndexed { idx, l ->
            val item = items[idx]
            TrnsPurchaseItem(
                itemSeq = idx + 1,
                itemCd = item.itemCode,
                itemClsCd = item.itemClassificationCode,
                itemNm = item.itemName,
                bcd = item.barcode,
                pkgUnitCd = item.packagingUnitCode,
                pkg = item.packageQuantity,
                qtyUnitCd = item.quantityUnitCode,
                qty = l.line.quantity,
                prc = l.line.unitPrice,
                splyAmt = l.line.supplyAmount,
                dcRt = l.line.discountRate,
                dcAmt = l.line.discountAmount,
                taxblAmt = l.line.taxableAmount,
                taxTyCd = l.line.taxTypeCode,
                taxAmt = l.line.taxAmount,
                totAmt = l.line.lineTotal
            )
        }
        val registration = deviceRepository.registration.value
        val baseReq = TrnsPurchaseSaveReq(
            tin = operator.tin,
            bhfId = operator.branchId,
            invcNo = 0L,
            spplrTin = request.supplier.supplierTin,
            spplrBhfId = request.supplier.branchId,
            spplrNm = request.supplier.supplierName,
            spplrInvcNo = supplierInvoice,
            pmtTyCd = request.paymentTypeCode,
            cfmDt = now,
            pchsDt = request.purchaseDate,
            wrhsDt = if (request.restockInventory) now else null,
            totItemCnt = request.lines.size,
            taxblAmtA = bands.taxableFor(TaxType.EXEMPT),
            taxblAmtB = bands.taxableFor(TaxType.STANDARD),
            taxblAmtC = bands.taxableFor(TaxType.ZERO_RATED),
            taxblAmtD = bands.taxableFor(TaxType.NON_VAT),
            taxblAmtE = bands.taxableFor(TaxType.REDUCED),
            taxRtA = TaxCalculationEngine.ratePercentFor(TaxType.EXEMPT),
            taxRtB = TaxCalculationEngine.ratePercentFor(TaxType.STANDARD),
            taxRtC = TaxCalculationEngine.ratePercentFor(TaxType.ZERO_RATED),
            taxRtD = TaxCalculationEngine.ratePercentFor(TaxType.NON_VAT),
            taxRtE = TaxCalculationEngine.ratePercentFor(TaxType.REDUCED),
            taxAmtA = bands.taxFor(TaxType.EXEMPT),
            taxAmtB = bands.taxFor(TaxType.STANDARD),
            taxAmtC = bands.taxFor(TaxType.ZERO_RATED),
            taxAmtD = bands.taxFor(TaxType.NON_VAT),
            taxAmtE = bands.taxFor(TaxType.REDUCED),
            totTaxblAmt = bands.totalTaxable,
            totTaxAmt = bands.totalTax,
            totAmt = PricedLine.round2(request.lines.sumOf { it.line.lineTotal }),
            remark = request.note?.trim()?.takeIf { it.isNotEmpty() },
            regrId = operator.id,
            regrNm = operator.name,
            modrId = operator.id,
            modrNm = operator.name,
            itemList = remoteItems
        )

        val lineEntities = request.lines.mapIndexed { idx, l ->
            PurchaseItemEntity(
                purchaseId = 0L,
                itemSequence = idx + 1,
                itemCode = l.itemCode,
                itemName = l.itemName,
                quantity = l.line.quantity,
                unitPrice = l.line.unitPrice,
                supplyAmount = l.line.supplyAmount,
                discountAmount = l.line.discountAmount,
                taxTypeCode = l.line.taxTypeCode,
                taxableAmount = l.line.taxableAmount,
                taxAmount = l.line.taxAmount,
                totalAmount = l.line.lineTotal
            )
        }

        val result = purchaseDao.insertWithNextNumber(
            floorPurchaseNumber = registration?.lastPurchaseInvoiceNumber ?: 0L,
            build = { number ->
                PurchaseEntity(
                    purchaseNumber = number,
                    supplierTin = request.supplier.supplierTin,
                    supplierName = request.supplier.supplierName,
                    supplierInvoiceNumber = supplierInvoice,
                    purchaseDate = request.purchaseDate,
                    paymentTypeCode = request.paymentTypeCode,
                    note = baseReq.remark,
                    totalTaxableAmount = baseReq.totTaxblAmt,
                    totalTaxAmount = baseReq.totTaxAmt,
                    totalAmount = baseReq.totAmt,
                    restockedInventory = request.restockInventory,
                    sourceLpoId = request.sourceLpoId,
                    payloadJson = gson.toJson(baseReq.copy(invcNo = number)),
                    createdBy = operator.name
                )
            },
            items = lineEntities
        )

        if (request.restockInventory) {
            request.lines.forEachIndexed { idx, l ->
                if (items[idx].itemTypeCode == ITEM_TYPE_SERVICE) return@forEachIndexed // services carry no stock
                inventoryRepository.recordStockIn(
                    itemCode = l.itemCode,
                    quantity = l.line.quantity,
                    reasonCode = "01",
                    reasonDescription = "Purchase (${request.supplier.supplierName} inv $supplierInvoice)",
                    operatorId = operator.id
                ).onFailure { Log.w(TAG, "Stock-in failed for ${l.itemCode}: ${it.message}") }
            }
        }
        result.purchaseId
    }

    override suspend fun pushPendingPurchases(): Int {
        var accepted = 0
        for (purchase in purchaseDao.getUnsynced()) {
            val error = try {
                val payload = gson.fromJson(purchase.payloadJson, TrnsPurchaseSaveReq::class.java)
                val response = transactionService.savePurchaseTransaction(payload)
                if (response.isSuccess) null else "Rejected by eTIMS: ${response.resultCd} ${response.resultMsg.orEmpty()}".trim()
            } catch (e: Exception) {
                e.toUserMessage()
            }
            purchaseDao.updateSyncStatus(purchase.id, if (error == null) "SYNCED" else "FAILED", error)
            if (error == null) accepted++
        }
        return accepted
    }

    private companion object {
        const val TAG = "PurchaseRepository"
        const val ITEM_TYPE_SERVICE = "3"
    }
}
