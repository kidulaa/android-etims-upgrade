package com.kra.paypoint.data.local.entity

import androidx.room.Entity
import androidx.room.PrimaryKey

/**
 * A locally-provisioned operator account. eTIMS has no cloud "login" endpoint — cashier
 * authentication is local-first so the till keeps working without connectivity — so the
 * salted/hashed credential is the source of truth for [com.kra.paypoint.data.repository.AuthRepositoryImpl].
 */
@Entity(tableName = "users")
data class UserEntity(
    @PrimaryKey val username: String,
    val fullName: String,
    val authorityCode: String,
    val branchId: String,
    val tin: String,
    val deviceSerial: String = "",
    val passwordHash: String,
    val passwordSalt: String,
    val isActive: Boolean = true,
    val createdAt: Long = System.currentTimeMillis()
)
