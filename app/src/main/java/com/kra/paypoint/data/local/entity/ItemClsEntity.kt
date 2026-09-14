package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "item_classifications")
data class ItemClsEntity(
    @PrimaryKey val code: String,
    val name: String,
    val level: Int,
    val taxTypeCode: String?,
    val isUsed: Boolean = true
)
