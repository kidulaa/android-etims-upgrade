package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.Index
import androidx.room.PrimaryKey

@Entity(
    tableName = "customers",
    indices = [
        Index(value = ["customerTin"], unique = true),
        Index(value = ["customerName"]),
        Index(value = ["telNo"])
    ]
)
data class CustomerEntity(
    @PrimaryKey
    val customerTin: String,
    val branchId: String = "00",
    val customerNo: String = "",
    val customerName: String,
    val customerType: String = "INDIVIDUAL", // "INDIVIDUAL" or "CORPORATE" (non-individual)
    val nationalId: String? = null,
    val address: String? = null,
    val telNo: String? = null,
    val email: String? = null,
    val isUsed: Boolean = true,
    val syncStatus: String = "SYNCED", // "PENDING", "SYNCED", "FAILED"
    val updatedAt: Long = System.currentTimeMillis()
)
