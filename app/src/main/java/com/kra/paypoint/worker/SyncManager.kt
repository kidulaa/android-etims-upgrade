package com.kra.paypoint.worker

import android.content.Context
import androidx.work.BackoffPolicy
import androidx.work.Constraints
import androidx.work.ExistingPeriodicWorkPolicy
import androidx.work.ExistingWorkPolicy
import androidx.work.NetworkType
import androidx.work.OneTimeWorkRequestBuilder
import androidx.work.PeriodicWorkRequestBuilder
import androidx.work.WorkManager
import java.util.concurrent.TimeUnit

object SyncManager {

    private val networkConstraints = Constraints.Builder()
        .setRequiredNetworkType(NetworkType.CONNECTED)
        .build()

    /**
     * Schedules periodic background synchronization every 15 minutes
     * when the device has active network connectivity.
     */
    fun schedulePeriodicSync(context: Context) {
        val periodicRequest = PeriodicWorkRequestBuilder<SyncWorker>(
            15, TimeUnit.MINUTES,
            5, TimeUnit.MINUTES // Flex interval
        )
            .setConstraints(networkConstraints)
            .setBackoffCriteria(
                BackoffPolicy.EXPONENTIAL,
                30, TimeUnit.SECONDS
            )
            .build()

        WorkManager.getInstance(context).enqueueUniquePeriodicWork(
            SyncWorker.WORK_NAME_PERIODIC,
            ExistingPeriodicWorkPolicy.KEEP,
            periodicRequest
        )
    }

    /**
     * Triggers an immediate one-time sync attempt when network is connected.
     */
    fun triggerImmediateSync(context: Context) {
        val oneTimeRequest = OneTimeWorkRequestBuilder<SyncWorker>()
            .setConstraints(networkConstraints)
            .setBackoffCriteria(
                BackoffPolicy.EXPONENTIAL,
                15, TimeUnit.SECONDS
            )
            .build()

        WorkManager.getInstance(context).enqueueUniqueWork(
            SyncWorker.WORK_NAME_ONETIME,
            ExistingWorkPolicy.REPLACE,
            oneTimeRequest
        )
    }
}
