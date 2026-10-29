package com.kra.paypoint.data.repository

import com.google.gson.Gson
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.dao.PurchaseDao
import com.kra.paypoint.data.local.dao.PurchaseInsertResult
import com.kra.paypoint.data.local.dao.SupplierDao
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.PurchaseEntity
import com.kra.paypoint.data.local.entity.SupplierEntity
import com.kra.paypoint.data.remote.api.TransactionService
import com.kra.paypoint.data.remote.model.transaction.TrnsPurchaseSaveReq
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesRes
import com.kra.paypoint.domain.model.Discount
import com.kra.paypoint.domain.model.DocumentLine
import com.kra.paypoint.domain.model.PricedLine
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.InventoryRepository
import com.kra.paypoint.domain.repository.PurchaseRequest
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.mock
import org.mockito.Mockito.never
import org.mockito.kotlin.any
import org.mockito.kotlin.anyOrNull
import org.mockito.kotlin.eq
import org.mockito.kotlin.verify
import org.mockito.kotlin.whenever

class PurchaseRepositoryImplTest {

    private lateinit var supplierDao: SupplierDao
    private lateinit var itemDao: ItemDao
    private lateinit var inventory: InventoryRepository
    private lateinit var service: TransactionService
    private val gson = Gson()
    private lateinit var fakeDao: FakePurchaseDao
    private lateinit var repository: PurchaseRepositoryImpl

    private val admin = User("00000", "Dunford", User.ROLE_ADMIN, "00", "P012345678X")
    private val supplier = SupplierEntity(supplierTin = "P051234567Z", supplierName = "Supply Ltd")
    private val flour = ItemEntity(itemCode = "KE2BGKG0000001", itemClassificationCode = "5020", itemTypeCode = "2", itemName = "Flour", taxTypeCode = "B")
    private val delivery = ItemEntity(itemCode = "KE3NTU0000001", itemClassificationCode = "8000", itemTypeCode = "3", itemName = "Delivery", taxTypeCode = "B")

    @Before
    fun setup() {
        supplierDao = mock(SupplierDao::class.java)
        itemDao = mock(ItemDao::class.java)
        inventory = mock(InventoryRepository::class.java)
        service = mock(TransactionService::class.java)
        val device = mock(DeviceRepository::class.java)
        whenever(device.registration).thenReturn(MutableStateFlow(null))
        fakeDao = FakePurchaseDao()
        repository = PurchaseRepositoryImpl(supplierDao, fakeDao, itemDao, inventory, device, service, gson)
    }

    private fun line(item: ItemEntity, qty: Double, price: Double) =
        DocumentLine(item.itemCode, item.itemName, PricedLine(price, qty, Discount.NONE, item.taxTypeCode))

    @Test
    fun `recording a purchase numbers it, files supplier details and stocks goods but not services`() = runTest {
        whenever(itemDao.getItemByCode(flour.itemCode)).thenReturn(flour)
        whenever(itemDao.getItemByCode(delivery.itemCode)).thenReturn(delivery)
        whenever(inventory.recordStockIn(any(), any(), any(), any(), anyOrNull(), any())).thenReturn(Result.success(10.0))

        val id = repository.recordPurchase(
            PurchaseRequest(supplier, "INV-77", "20261028", "01", null, listOf(line(flour, 10.0, 116.0), line(delivery, 1.0, 232.0)), restockInventory = true),
            admin
        ).getOrThrow()

        val saved = fakeDao.saved.single { it.id == id }
        assertEquals(1L, saved.purchaseNumber)
        assertEquals(1392.0, saved.totalAmount, 0.001)
        assertEquals(192.0, saved.totalTaxAmount, 0.001)
        val payload = gson.fromJson(saved.payloadJson, TrnsPurchaseSaveReq::class.java)
        assertEquals(1L, payload.invcNo)
        assertEquals("P051234567Z", payload.spplrTin)
        assertEquals("INV-77", payload.spplrInvcNo)
        assertEquals("P012345678X", payload.tin)
        assertEquals(2, payload.totItemCnt)
        verify(inventory).recordStockIn(eq(flour.itemCode), eq(10.0), eq("01"), any(), anyOrNull(), eq("00000"))
        verify(inventory, never()).recordStockIn(eq(delivery.itemCode), any(), any(), any(), anyOrNull(), any())
    }

    @Test
    fun `the same supplier invoice cannot be recorded twice`() = runTest {
        fakeDao.duplicateCount = 1
        val result = repository.recordPurchase(
            PurchaseRequest(supplier, "INV-77", "20261028", "01", null, listOf(line(flour, 1.0, 10.0)), true),
            admin
        )
        assertTrue(result.isFailure)
    }

    @Test
    fun `pending purchases are pushed and marked by outcome`() = runTest {
        fakeDao.unsynced = listOf(
            PurchaseEntity(id = 1, purchaseNumber = 1, supplierTin = "P051234567Z", supplierName = "S", supplierInvoiceNumber = "A", purchaseDate = "20261028", payloadJson = "{}"),
            PurchaseEntity(id = 2, purchaseNumber = 2, supplierTin = "P051234567Z", supplierName = "S", supplierInvoiceNumber = "B", purchaseDate = "20261028", payloadJson = "{}")
        )
        whenever(service.savePurchaseTransaction(any()))
            .thenReturn(TrnsSalesRes("000", "OK", null, null))
            .thenReturn(TrnsSalesRes("910", "Invalid", null, null))

        assertEquals(1, repository.pushPendingPurchases())
        assertEquals(listOf(1L to "SYNCED", 2L to "FAILED"), fakeDao.statusUpdates)
    }
}

private class FakePurchaseDao : PurchaseDao {
    val saved = mutableListOf<PurchaseEntity>()
    var duplicateCount = 0
    var unsynced: List<PurchaseEntity> = emptyList()
    val statusUpdates = mutableListOf<Pair<Long, String>>()

    override suspend fun insertPurchase(purchase: PurchaseEntity): Long {
        val id = saved.size + 1L
        saved += purchase.copy(id = id)
        return id
    }
    override suspend fun insertItems(items: List<com.kra.paypoint.data.local.entity.PurchaseItemEntity>) {}
    override suspend fun getLastPurchaseNumber(): Long? = saved.maxOfOrNull { it.purchaseNumber }
    override suspend fun insertWithNextNumber(
        floorPurchaseNumber: Long,
        build: (purchaseNumber: Long) -> PurchaseEntity,
        items: List<com.kra.paypoint.data.local.entity.PurchaseItemEntity>
    ): PurchaseInsertResult {
        val next = maxOf(getLastPurchaseNumber() ?: 0L, floorPurchaseNumber) + 1
        return PurchaseInsertResult(insertPurchase(build(next)), next)
    }
    override fun observeAll() = kotlinx.coroutines.flow.flowOf(saved.toList())
    override suspend fun getWithItems(id: Long) = null
    override suspend fun getUnsynced(): List<PurchaseEntity> = unsynced
    override suspend fun updateSyncStatus(id: Long, status: String, error: String?) { statusUpdates += id to status }
    override suspend fun countBySupplierInvoice(supplierTin: String, supplierInvoiceNumber: String): Int = duplicateCount
}
