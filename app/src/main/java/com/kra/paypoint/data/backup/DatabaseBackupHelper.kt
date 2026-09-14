package com.kra.paypoint.data.backup

import android.content.Context
import android.util.Log
import com.google.gson.GsonBuilder
import com.kra.paypoint.data.local.dao.TransactionDao
import com.kra.paypoint.data.local.entity.TransactionWithItems
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import java.io.File
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class DatabaseBackupHelper @Inject constructor(
    @ApplicationContext private val context: Context,
    private val transactionDao: TransactionDao
) {
    private val gson = GsonBuilder().setPrettyPrinting().create()
    private val backupDir = File(context.getExternalFilesDir(null), "database_backups").apply {
        if (!exists()) mkdirs()
    }

    /**
     * Backs up the current transactions to a JSON file.
     */
    suspend fun backupDatabase() {
        withContext(Dispatchers.IO) {
            try {
                val timestamp = System.currentTimeMillis()
                val transactions = transactionDao.getAllTransactionsWithItems()
                
                if (transactions.isEmpty()) {
                    Log.d("Backup", "No transactions to backup.")
                    return@withContext
                }

                val backupFile = File(backupDir, "transactions_$timestamp.json")
                backupFile.writeText(gson.toJson(transactions))

                Log.d("Backup", "Database backed up to ${backupFile.absolutePath}")

                // Keep only the last 5 backups
                cleanupOldBackups(5)
            } catch (e: Exception) {
                Log.e("Backup", "Failed to backup database", e)
            }
        }
    }

    private fun cleanupOldBackups(maxBackups: Int) {
        val allBackups = backupDir.listFiles { file ->
            file.name.startsWith("transactions_") && file.name.endsWith(".json")
        }?.toList() ?: return

        if (allBackups.size > maxBackups) {
            allBackups.sortedBy { it.name }
                .take(allBackups.size - maxBackups)
                .forEach { it.delete() }
        }
    }
}
