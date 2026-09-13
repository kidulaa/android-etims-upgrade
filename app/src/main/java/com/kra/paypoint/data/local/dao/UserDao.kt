package com.kra.paypoint.data.local.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import com.kra.paypoint.data.local.entity.UserEntity

@Dao
interface UserDao {
    @Query("SELECT * FROM users WHERE username = :username AND isActive = 1 LIMIT 1")
    suspend fun getByUsername(username: String): UserEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun upsert(user: UserEntity)

    @Query("SELECT COUNT(*) FROM users")
    suspend fun countUsers(): Int

    @Query("SELECT * FROM users WHERE isActive = 1 ORDER BY fullName ASC")
    suspend fun getAllActive(): List<UserEntity>

    @Query("UPDATE users SET isActive = 0 WHERE username = :username")
    suspend fun deactivate(username: String)
}
