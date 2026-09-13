package com.kra.paypoint

import android.app.Application
import com.kra.paypoint.worker.SyncManager
import dagger.hilt.android.HiltAndroidApp

@HiltAndroidApp
class PayPointApplication : Application() {

    override fun onCreate() {
        super.onCreate()
        // Initialize background synchronization queue for offline eTIMS transactions
        SyncManager.schedulePeriodicSync(this)
    }
}
