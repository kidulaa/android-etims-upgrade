package com.kra.paypoint.data.local.database

import androidx.room.migration.Migration
import androidx.sqlite.db.SupportSQLiteDatabase

/**
 * Explicit migrations only, deliberately — a destructive fallback would drop unsynced
 * fiscal transactions and Z-reports still queued for eTIMS submission.
 */
val MIGRATION_4_5 = object : Migration(4, 5) {
    override fun migrate(db: SupportSQLiteDatabase) {
        db.execSQL(
            """
            CREATE TABLE IF NOT EXISTS `users` (
                `username` TEXT NOT NULL,
                `fullName` TEXT NOT NULL,
                `authorityCode` TEXT NOT NULL,
                `branchId` TEXT NOT NULL,
                `tin` TEXT NOT NULL,
                `passwordHash` TEXT NOT NULL,
                `passwordSalt` TEXT NOT NULL,
                `isActive` INTEGER NOT NULL,
                `createdAt` INTEGER NOT NULL,
                PRIMARY KEY(`username`)
            )
            """.trimIndent()
        )
    }
}

val ALL_MIGRATIONS = arrayOf(MIGRATION_4_5)
