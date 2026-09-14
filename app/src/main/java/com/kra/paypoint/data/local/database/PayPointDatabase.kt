package com.kra.paypoint.data.local.database

import androidx.room.Database
import androidx.room.RoomDatabase
import com.kra.paypoint.data.local.dao.CodeDao
import com.kra.paypoint.data.local.dao.CustomerDao
import com.kra.paypoint.data.local.dao.ItemClsDao
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.dao.StockMovementDao
import com.kra.paypoint.data.local.dao.TransactionDao
import com.kra.paypoint.data.local.dao.UserDao
import com.kra.paypoint.data.local.dao.ZReportDao
import com.kra.paypoint.data.local.entity.CodeEntity
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemClsEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.local.entity.StockMovementEntity
import com.kra.paypoint.data.local.entity.TransactionEntity
import com.kra.paypoint.data.local.entity.TransactionItemEntity
import com.kra.paypoint.data.local.entity.UserEntity
import com.kra.paypoint.data.local.entity.ZReportEntity

@Database(
    entities = [
        ItemEntity::class,
        CustomerEntity::class,
        TransactionEntity::class,
        TransactionItemEntity::class,
        CodeEntity::class,
        ItemClsEntity::class,
        ZReportEntity::class,
        StockMovementEntity::class,
        UserEntity::class
    ],
    version = 6,
    exportSchema = true
)
abstract class PayPointDatabase : RoomDatabase() {
    abstract fun itemDao(): ItemDao
    abstract fun customerDao(): CustomerDao
    abstract fun transactionDao(): TransactionDao
    abstract fun codeDao(): CodeDao
    abstract fun itemClsDao(): ItemClsDao
    abstract fun zReportDao(): ZReportDao
    abstract fun stockMovementDao(): StockMovementDao
    abstract fun userDao(): UserDao
}
