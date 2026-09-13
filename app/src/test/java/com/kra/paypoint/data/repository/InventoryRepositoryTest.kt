package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.dao.StockMovementDao
import com.kra.paypoint.data.local.database.TransactionRunner
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.StockMovementEntity
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.`when`
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify
import org.mockito.kotlin.argumentCaptor
import org.mockito.kotlin.whenever

/** Runs the block inline with no real DB transaction — sufficient for unit tests, since the
 *  atomicity guarantee itself lives in Room/[com.kra.paypoint.data.local.database.RoomTransactionRunner]. */
private class FakeTransactionRunner : TransactionRunner {
    override suspend fun <T> run(block: suspend () -> T): T = block()
}

class InventoryRepositoryTest {

    private lateinit var itemDao: ItemDao
    private lateinit var stockMovementDao: StockMovementDao
    private lateinit var repository: InventoryRepositoryImpl

    private val testItem = ItemEntity(
        itemCode = "ITM-100",
        itemClassificationCode = "5020",
        itemTypeCode = "1",
        itemName = "Mineral Water 1L",
        defaultUnitPrice = 100.0,
        stockQuantity = 25.0,
        isUsed = true
    )

    @Before
    fun setup() {
        itemDao = mock(ItemDao::class.java)
        stockMovementDao = mock(StockMovementDao::class.java)
        repository = InventoryRepositoryImpl(FakeTransactionRunner(), itemDao, stockMovementDao)
    }

    @Test
    fun `recordStockIn increments stock atomically and inserts movement record`() = runTest {
        `when`(itemDao.getItemByCode("ITM-100")).thenReturn(testItem)

        val captor = argumentCaptor<StockMovementEntity>()
        whenever(stockMovementDao.insertMovement(captor.capture())).thenReturn(1L)

        val result = repository.recordStockIn("ITM-100", 15.0, "01", "Delivery", "PO-123", "ADMIN")

        assertTrue(result.isSuccess)
        assertEquals(40.0, result.getOrNull()!!, 0.001)

        // Atomic relative update (stockQuantity = stockQuantity + delta), not a read-modify-write
        // "set to computed value" — that's the race the old implementation had.
        verify(itemDao).increaseStock("ITM-100", 15.0)

        val capturedMovement = captor.firstValue
        assertEquals("STOCK_IN", capturedMovement.movementType)
        assertEquals(25.0, capturedMovement.previousStock, 0.001)
        assertEquals(40.0, capturedMovement.newStock, 0.001)
        assertEquals(15.0, capturedMovement.quantity, 0.001)
    }

    @Test
    fun `recordStockOut decrements stock when sufficient inventory available`() = runTest {
        `when`(itemDao.getItemByCode("ITM-100")).thenReturn(testItem)
        `when`(itemDao.decreaseStockIfSufficient("ITM-100", 5.0)).thenReturn(1)

        val captor = argumentCaptor<StockMovementEntity>()
        whenever(stockMovementDao.insertMovement(captor.capture())).thenReturn(2L)

        val result = repository.recordStockOut("ITM-100", 5.0, "02", "Damage", "Broken cap", "ADMIN")

        assertTrue(result.isSuccess)
        assertEquals(20.0, result.getOrNull()!!, 0.001)

        val capturedMovement = captor.firstValue
        assertEquals("STOCK_OUT", capturedMovement.movementType)
        assertEquals(25.0, capturedMovement.previousStock, 0.001)
        assertEquals(20.0, capturedMovement.newStock, 0.001)
        assertEquals(5.0, capturedMovement.quantity, 0.001)
    }

    @Test
    fun `recordStockOut fails when the conditional decrement affects zero rows (insufficient or raced-away stock)`() = runTest {
        `when`(itemDao.getItemByCode("ITM-100")).thenReturn(testItem)
        // The DB-level conditional UPDATE is the actual source of truth for sufficiency, not
        // the earlier read — this covers both "genuinely insufficient" and "another writer
        // took the last units between the read and the write".
        `when`(itemDao.decreaseStockIfSufficient("ITM-100", 30.0)).thenReturn(0)

        val result = repository.recordStockOut("ITM-100", 30.0, "02", "Damage", null, "ADMIN")

        assertTrue(result.isFailure)
    }

    @Test
    fun `recordAdjustment sets exact physical count`() = runTest {
        `when`(itemDao.getItemByCode("ITM-100")).thenReturn(testItem)

        val captor = argumentCaptor<StockMovementEntity>()
        whenever(stockMovementDao.insertMovement(captor.capture())).thenReturn(3L)

        val result = repository.recordAdjustment("ITM-100", 28.0, "Physical Recount", "ADMIN")

        assertTrue(result.isSuccess)
        assertEquals(28.0, result.getOrNull()!!, 0.001)

        verify(itemDao).setStockQuantity("ITM-100", 28.0)

        val capturedMovement = captor.firstValue
        assertEquals("ADJUSTMENT", capturedMovement.movementType)
        assertEquals(25.0, capturedMovement.previousStock, 0.001)
        assertEquals(28.0, capturedMovement.newStock, 0.001)
    }
}
