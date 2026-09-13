package com.kra.paypoint.data.local.database

import androidx.room.withTransaction
import javax.inject.Inject
import javax.inject.Singleton

/**
 * Thin seam around [androidx.room.withTransaction] so repositories that need atomic
 * multi-DAO operations (e.g. inventory: read stock, write stock, log the movement) don't
 * depend on a concrete [PayPointDatabase] directly — a fake implementation in tests can run
 * the block without needing a real (Robolectric-backed) Room database.
 */
interface TransactionRunner {
    suspend fun <T> run(block: suspend () -> T): T
}

@Singleton
class RoomTransactionRunner @Inject constructor(
    private val database: PayPointDatabase
) : TransactionRunner {
    override suspend fun <T> run(block: suspend () -> T): T = database.withTransaction(block)
}
