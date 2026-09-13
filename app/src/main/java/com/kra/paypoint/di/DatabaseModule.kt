package com.kra.paypoint.di

import android.content.Context
import androidx.room.Room
import com.kra.paypoint.data.local.dao.CodeDao
import com.kra.paypoint.data.local.dao.CustomerDao
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.dao.StockMovementDao
import com.kra.paypoint.data.local.dao.TransactionDao
import com.kra.paypoint.data.local.dao.UserDao
import com.kra.paypoint.data.local.dao.ZReportDao
import com.kra.paypoint.data.local.database.ALL_MIGRATIONS
import com.kra.paypoint.data.local.database.PayPointDatabase
import com.kra.paypoint.data.local.database.RoomTransactionRunner
import com.kra.paypoint.data.local.database.TransactionRunner
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.android.qualifiers.ApplicationContext
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object DatabaseModule {

    @Provides
    @Singleton
    fun provideDatabase(
        @ApplicationContext context: Context
    ): PayPointDatabase {
        return Room.databaseBuilder(
            context,
            PayPointDatabase::class.java,
            "paypoint_etims.db"
        )
            // Deliberately no fallbackToDestructiveMigration(): a schema bump must never
            // silently wipe transactions still queued for eTIMS submission. Add a Migration
            // to Migrations.kt (and to ALL_MIGRATIONS) for every version bump instead.
            .addMigrations(*ALL_MIGRATIONS)
            .build()
    }

    @Provides
    fun provideItemDao(database: PayPointDatabase): ItemDao {
        return database.itemDao()
    }

    @Provides
    fun provideCustomerDao(database: PayPointDatabase): CustomerDao {
        return database.customerDao()
    }

    @Provides
    fun provideTransactionDao(database: PayPointDatabase): TransactionDao {
        return database.transactionDao()
    }

    @Provides
    fun provideCodeDao(database: PayPointDatabase): CodeDao {
        return database.codeDao()
    }

    @Provides
    fun provideZReportDao(database: PayPointDatabase): ZReportDao {
        return database.zReportDao()
    }

    @Provides
    fun provideStockMovementDao(database: PayPointDatabase): StockMovementDao {
        return database.stockMovementDao()
    }

    @Provides
    fun provideUserDao(database: PayPointDatabase): UserDao {
        return database.userDao()
    }

    @Provides
    @Singleton
    fun provideTransactionRunner(database: PayPointDatabase): TransactionRunner {
        return RoomTransactionRunner(database)
    }
}
