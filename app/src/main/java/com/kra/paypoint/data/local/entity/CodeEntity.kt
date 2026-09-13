package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.Index
import androidx.room.PrimaryKey

@Entity(
    tableName = "system_codes",
    indices = [
        Index(value = ["classCode", "detailCode"], unique = true)
    ]
)
data class CodeEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val classCode: String,      // e.g. "TAX_TYPE", "PMT_TYPE", "UNIT"
    val detailCode: String,     // e.g. "A", "B", "01", "NT"
    val detailName: String,     // e.g. "Standard Rate 16%", "Cash"
    val numericValue: Double? = null, // e.g. 16.0 for tax rate A
    val isUsed: Boolean = true,
    val sortOrder: Int = 0
)
