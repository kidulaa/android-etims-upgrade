package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.Index
import androidx.room.PrimaryKey

/** A business this taxpayer buys from — the counterparty on LPOs and purchase invoices. */
@Entity(
    tableName = "suppliers",
    indices = [Index(value = ["supplierName"])]
)
data class SupplierEntity(
    @PrimaryKey
    val supplierTin: String,
    val supplierName: String,
    val branchId: String = "00",
    val telNo: String? = null,
    val email: String? = null,
    val isUsed: Boolean = true,
    val updatedAt: Long = System.currentTimeMillis()
)
