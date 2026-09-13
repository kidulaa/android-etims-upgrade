package com.kra.paypoint.di

import android.content.Context
import com.kra.paypoint.hardware.printer.BluetoothPrinterManager
import com.kra.paypoint.hardware.printer.PrinterService
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.android.qualifiers.ApplicationContext
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object HardwareModule {

    @Provides
    @Singleton
    fun providePrinterService(
        @ApplicationContext context: Context
    ): PrinterService {
        return BluetoothPrinterManager(context)
    }
}
