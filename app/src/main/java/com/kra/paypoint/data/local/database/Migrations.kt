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

val MIGRATION_5_6 = object : Migration(5, 6) {
    override fun migrate(db: SupportSQLiteDatabase) {
        db.execSQL(
            """
            CREATE TABLE IF NOT EXISTS `item_classifications` (
                `code` TEXT NOT NULL,
                `name` TEXT NOT NULL,
                `level` INTEGER NOT NULL,
                `taxTypeCode` TEXT,
                `isUsed` INTEGER NOT NULL,
                PRIMARY KEY(`code`)
            )
            """.trimIndent()
        )
        // Add deviceSerial to users table
        db.execSQL("ALTER TABLE `users` ADD COLUMN `deviceSerial` TEXT NOT NULL DEFAULT ''")
    }
}

val ALL_MIGRATIONS = arrayOf(MIGRATION_4_5, MIGRATION_5_6)
