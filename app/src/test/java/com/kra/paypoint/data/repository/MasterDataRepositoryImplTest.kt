package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.CodeDao
import com.kra.paypoint.data.local.dao.CustomerDao
import com.kra.paypoint.data.local.dao.ItemClsDao
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.remote.api.MasterDataService
import com.kra.paypoint.data.remote.model.master.ItemListData
import com.kra.paypoint.data.remote.model.master.ItemListRes
import com.kra.paypoint.data.remote.model.master.MasterSaveRes
import com.kra.paypoint.data.remote.model.master.RemoteItem
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.InventoryRepository
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNull
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.mock
import org.mockito.Mockito.never
import org.mockito.kotlin.any
import org.mockito.kotlin.anyOrNull
import org.mockito.kotlin.argumentCaptor
import org.mockito.kotlin.eq
import org.mockito.kotlin.verify
import org.mockito.kotlin.whenever
import java.io.IOException

class MasterDataRepositoryImplTest {

    private lateinit var itemDao: ItemDao
    private lateinit var customerDao: CustomerDao
    private lateinit var service: MasterDataService
    private lateinit var inventoryRepository: InventoryRepository
    private lateinit var repository: MasterDataRepositoryImpl

    private val admin = User(id = "00000", name = "Dunford", authorityCode = User.ROLE_ADMIN, branchId = "00", tin = "P012345678X")

    private val oils = ItemEntity(
        itemCode = "KE2CTU0000001",
        itemClassificationCode = "1218160100",
        itemTypeCode = "2",
        itemName = "Oils",
        packagingUnitCode = "CT",
        taxTypeCode = "B",
        defaultUnitPrice = 7000.0
    )

    @Before
    fun setup() {
        itemDao = mock(ItemDao::class.java)
        customerDao = mock(CustomerDao::class.java)
        service = mock(MasterDataService::class.java)
        inventoryRepository = mock(InventoryRepository::class.java)
        repository = MasterDataRepositoryImpl(
            itemDao, customerDao, mock(CodeDao::class.java), mock(ItemClsDao::class.java), service, inventoryRepository
        )
    }

    private fun remote(code: String, name: String) = RemoteItem(itemCd = code, itemClsCd = "1218160100", itemTyCd = "2", itemNm = name, taxTyCd = "B", dftPrc = 7500.0)

    @Test
    fun `item sync keeps local stock levels and does not clobber unsynced local edits`() = runTest {
        whenever(service.selectItemList(any())).thenReturn(
            ItemListRes("000", "OK", null, ItemListData(listOf(remote("SYNCED1", "Remote Oils"), remote("PEND1", "Remote Name"), remote("NEW1", "New"))))
        )
        whenever(itemDao.getItemByCode("SYNCED1")).thenReturn(oils.copy(itemCode = "SYNCED1", stockQuantity = 42.0))
        whenever(itemDao.getItemByCode("PEND1")).thenReturn(oils.copy(itemCode = "PEND1", syncStatus = "PENDING"))
        whenever(itemDao.getItemByCode("NEW1")).thenReturn(null)

        val result = repository.syncItemsFromEtims("P012345678X", "00")

        assertEquals(3, result.getOrThrow())
        val captor = argumentCaptor<List<ItemEntity>>()
        verify(itemDao).insertItems(captor.capture())
        val written = captor.firstValue.associateBy { it.itemCode }
        assertEquals(setOf("SYNCED1", "NEW1"), written.keys)
        assertEquals(42.0, written.getValue("SYNCED1").stockQuantity, 0.0)
        assertEquals("Remote Oils", written.getValue("SYNCED1").itemName)
        assertEquals(0.0, written.getValue("NEW1").stockQuantity, 0.0)
    }

    @Test
    fun `registerItem books beginning stock as a movement and marks it synced when eTIMS accepts`() = runTest {
        whenever(itemDao.getItemByCode(oils.itemCode)).thenReturn(null)
        whenever(inventoryRepository.recordStockIn(any(), any(), any(), any(), anyOrNull(), any())).thenReturn(Result.success(100.0))
        whenever(service.saveItem(any())).thenReturn(MasterSaveRes("000", "OK", null))

        val outcome = repository.registerItem(oils.copy(stockQuantity = 999.0), beginningStock = 100.0, operator = admin).getOrThrow()

        assertTrue(outcome.registeredWithEtims)
        val inserted = argumentCaptor<ItemEntity>()
        verify(itemDao).insertItem(inserted.capture())
        assertEquals(0.0, inserted.firstValue.stockQuantity, 0.0) // opening balance only via the movement
        verify(inventoryRepository).recordStockIn(eq(oils.itemCode), eq(100.0), eq("01"), eq("Beginning Stock"), anyOrNull(), eq("00000"))
        verify(itemDao).updateItemSyncStatus(oils.itemCode, "SYNCED")
    }

    @Test
    fun `registerItem stays pending when eTIMS is unreachable`() = runTest {
        whenever(itemDao.getItemByCode(oils.itemCode)).thenReturn(null)
        whenever(service.saveItem(any())).thenThrow(RuntimeException(IOException("offline")))

        val outcome = repository.registerItem(oils, beginningStock = 0.0, operator = admin).getOrThrow()

        assertFalse(outcome.registeredWithEtims)
        verify(itemDao).updateItemSyncStatus(oils.itemCode, "PENDING")
        verify(inventoryRepository, never()).recordStockIn(any(), any(), any(), any(), anyOrNull(), any())
    }

    @Test
    fun `registerItem rejects a duplicate item code`() = runTest {
        whenever(itemDao.getItemByCode(oils.itemCode)).thenReturn(oils)

        val result = repository.registerItem(oils, beginningStock = 0.0, operator = admin)

        assertTrue(result.isFailure)
        verify(itemDao, never()).insertItem(any())
    }

    @Test
    fun `updateItem never overwrites stock from the form`() = runTest {
        whenever(itemDao.getItemByCode(oils.itemCode)).thenReturn(oils.copy(stockQuantity = 37.0))
        whenever(service.saveItem(any())).thenReturn(MasterSaveRes("000", "OK", null))

        repository.updateItem(oils.copy(stockQuantity = 0.0, defaultUnitPrice = 7200.0), admin).getOrThrow()

        val updated = argumentCaptor<ItemEntity>()
        verify(itemDao).updateItem(updated.capture())
        assertEquals(37.0, updated.firstValue.stockQuantity, 0.0)
        assertEquals(7200.0, updated.firstValue.defaultUnitPrice, 0.0)
    }

    @Test
    fun `registerCustomer normalises the PIN and rejects an individual with a company PIN`() = runTest {
        whenever(service.saveBhfCustomer(any())).thenReturn(MasterSaveRes("000", "OK", null))

        val ok = repository.registerCustomer(CustomerEntity(customerTin = " a009723435x ", customerName = "Brian"), admin)
        assertNull(ok.getOrThrow().pushError)
        val saved = argumentCaptor<CustomerEntity>()
        verify(customerDao).insertCustomer(saved.capture())
        assertEquals("A009723435X", saved.firstValue.customerTin)

        val bad = repository.registerCustomer(CustomerEntity(customerTin = "P051234567Z", customerName = "Acme Ltd"), admin)
        assertTrue(bad.isFailure)
    }
}
