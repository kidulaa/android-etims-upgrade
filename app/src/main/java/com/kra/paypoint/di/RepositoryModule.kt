package com.kra.paypoint.di

import com.kra.paypoint.data.local.device.DeviceConfigStore
import com.kra.paypoint.data.local.device.DeviceConfigStoreImpl
import com.kra.paypoint.data.repository.AuthRepositoryImpl
import com.kra.paypoint.data.repository.DeviceRepositoryImpl
import com.kra.paypoint.data.repository.InventoryRepositoryImpl
import com.kra.paypoint.data.repository.MasterDataRepositoryImpl
import com.kra.paypoint.data.repository.TransactionRepositoryImpl
import com.kra.paypoint.data.repository.ZReportRepositoryImpl
import com.kra.paypoint.domain.repository.AuthRepository
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.InventoryRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import com.kra.paypoint.domain.repository.TransactionRepository
import com.kra.paypoint.domain.repository.ZReportRepository
import dagger.Binds
import dagger.Module
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
abstract class RepositoryModule {

    @Binds
    @Singleton
    abstract fun bindAuthRepository(
        impl: AuthRepositoryImpl
    ): AuthRepository

    @Binds
    @Singleton
    abstract fun bindTransactionRepository(
        impl: TransactionRepositoryImpl
    ): TransactionRepository

    @Binds
    @Singleton
    abstract fun bindMasterDataRepository(
        impl: MasterDataRepositoryImpl
    ): MasterDataRepository

    @Binds
    @Singleton
    abstract fun bindZReportRepository(
        impl: ZReportRepositoryImpl
    ): ZReportRepository

    @Binds
    @Singleton
    abstract fun bindInventoryRepository(
        impl: InventoryRepositoryImpl
    ): InventoryRepository

    @Binds
    @Singleton
    abstract fun bindDeviceRepository(
        impl: DeviceRepositoryImpl
    ): DeviceRepository

    @Binds
    @Singleton
    abstract fun bindDeviceConfigStore(
        impl: DeviceConfigStoreImpl
    ): DeviceConfigStore
}
