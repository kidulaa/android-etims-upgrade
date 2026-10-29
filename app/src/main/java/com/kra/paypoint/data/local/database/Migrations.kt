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

val MIGRATION_6_7 = object : Migration(6, 7) {
    override fun migrate(db: SupportSQLiteDatabase) {
        db.execSQL("ALTER TABLE `users` ADD COLUMN `telNo` TEXT")
        db.execSQL("ALTER TABLE `items` ADD COLUMN `originNationCode` TEXT NOT NULL DEFAULT 'KE'")
        db.execSQL("ALTER TABLE `items` ADD COLUMN `syncStatus` TEXT NOT NULL DEFAULT 'SYNCED'")
        db.execSQL("ALTER TABLE `customers` ADD COLUMN `customerType` TEXT NOT NULL DEFAULT 'INDIVIDUAL'")
    }
}

/**
 * v8: purchases (suppliers, LPOs, purchase invoices), non-fiscal sales documents
 * (quotations, proformas), credit sales and item descriptions. Table DDL is copied verbatim
 * from the exported schema so Room's open-time validation matches exactly.
 */
val MIGRATION_7_8 = object : Migration(7, 8) {
    override fun migrate(db: SupportSQLiteDatabase) {
        db.execSQL("CREATE TABLE IF NOT EXISTS `suppliers` (`supplierTin` TEXT NOT NULL, `supplierName` TEXT NOT NULL, `branchId` TEXT NOT NULL, `telNo` TEXT, `email` TEXT, `isUsed` INTEGER NOT NULL, `updatedAt` INTEGER NOT NULL, PRIMARY KEY(`supplierTin`))")
        db.execSQL("CREATE INDEX IF NOT EXISTS `index_suppliers_supplierName` ON `suppliers` (`supplierName`)")
        db.execSQL("CREATE TABLE IF NOT EXISTS `commercial_documents` (`id` INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, `docType` TEXT NOT NULL, `sequence` INTEGER NOT NULL, `partyTin` TEXT, `partyName` TEXT NOT NULL, `partyPhone` TEXT, `docDate` TEXT NOT NULL, `dueDate` TEXT, `note` TEXT, `status` TEXT NOT NULL, `convertedReference` TEXT, `totalAmount` REAL NOT NULL, `totalTaxAmount` REAL NOT NULL, `totalDiscount` REAL NOT NULL, `createdBy` TEXT NOT NULL, `createdAt` INTEGER NOT NULL)")
        db.execSQL("CREATE UNIQUE INDEX IF NOT EXISTS `index_commercial_documents_docType_sequence` ON `commercial_documents` (`docType`, `sequence`)")
        db.execSQL("CREATE INDEX IF NOT EXISTS `index_commercial_documents_partyName` ON `commercial_documents` (`partyName`)")
        db.execSQL("CREATE TABLE IF NOT EXISTS `commercial_document_items` (`id` INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, `documentId` INTEGER NOT NULL, `itemSequence` INTEGER NOT NULL, `itemCode` TEXT NOT NULL, `itemName` TEXT NOT NULL, `quantity` REAL NOT NULL, `unitPrice` REAL NOT NULL, `discountAmount` REAL NOT NULL, `taxTypeCode` TEXT NOT NULL, `taxAmount` REAL NOT NULL, `totalAmount` REAL NOT NULL, FOREIGN KEY(`documentId`) REFERENCES `commercial_documents`(`id`) ON UPDATE NO ACTION ON DELETE CASCADE )")
        db.execSQL("CREATE INDEX IF NOT EXISTS `index_commercial_document_items_documentId` ON `commercial_document_items` (`documentId`)")
        db.execSQL("CREATE TABLE IF NOT EXISTS `purchases` (`id` INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, `purchaseNumber` INTEGER NOT NULL, `supplierTin` TEXT NOT NULL, `supplierName` TEXT NOT NULL, `supplierInvoiceNumber` TEXT NOT NULL, `purchaseDate` TEXT NOT NULL, `paymentTypeCode` TEXT NOT NULL, `note` TEXT, `totalTaxableAmount` REAL NOT NULL, `totalTaxAmount` REAL NOT NULL, `totalAmount` REAL NOT NULL, `restockedInventory` INTEGER NOT NULL, `sourceLpoId` INTEGER, `syncStatus` TEXT NOT NULL, `syncAttempts` INTEGER NOT NULL, `lastError` TEXT, `payloadJson` TEXT NOT NULL, `createdBy` TEXT NOT NULL, `createdAt` INTEGER NOT NULL)")
        db.execSQL("CREATE UNIQUE INDEX IF NOT EXISTS `index_purchases_purchaseNumber` ON `purchases` (`purchaseNumber`)")
        db.execSQL("CREATE INDEX IF NOT EXISTS `index_purchases_syncStatus` ON `purchases` (`syncStatus`)")
        db.execSQL("CREATE INDEX IF NOT EXISTS `index_purchases_supplierTin` ON `purchases` (`supplierTin`)")
        db.execSQL("CREATE TABLE IF NOT EXISTS `purchase_items` (`id` INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, `purchaseId` INTEGER NOT NULL, `itemSequence` INTEGER NOT NULL, `itemCode` TEXT NOT NULL, `itemName` TEXT NOT NULL, `quantity` REAL NOT NULL, `unitPrice` REAL NOT NULL, `supplyAmount` REAL NOT NULL, `discountAmount` REAL NOT NULL, `taxTypeCode` TEXT NOT NULL, `taxableAmount` REAL NOT NULL, `taxAmount` REAL NOT NULL, `totalAmount` REAL NOT NULL, FOREIGN KEY(`purchaseId`) REFERENCES `purchases`(`id`) ON UPDATE NO ACTION ON DELETE CASCADE )")
        db.execSQL("CREATE INDEX IF NOT EXISTS `index_purchase_items_purchaseId` ON `purchase_items` (`purchaseId`)")
        db.execSQL("CREATE INDEX IF NOT EXISTS `index_purchase_items_itemCode` ON `purchase_items` (`itemCode`)")
        db.execSQL("ALTER TABLE `items` ADD COLUMN `description` TEXT")
        db.execSQL("ALTER TABLE `transactions` ADD COLUMN `isPaid` INTEGER NOT NULL DEFAULT 1")
        db.execSQL("ALTER TABLE `transactions` ADD COLUMN `dueDate` TEXT")
        db.execSQL("ALTER TABLE `transactions` ADD COLUMN `note` TEXT")
        db.execSQL("ALTER TABLE `z_reports` ADD COLUMN `creditAmount` REAL NOT NULL DEFAULT 0")
    }
}

val ALL_MIGRATIONS = arrayOf(MIGRATION_4_5, MIGRATION_5_6, MIGRATION_6_7, MIGRATION_7_8)
