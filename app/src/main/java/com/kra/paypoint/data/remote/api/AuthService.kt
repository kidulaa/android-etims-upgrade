package com.kra.paypoint.data.remote.api

import com.kra.paypoint.data.remote.model.auth.BhfUserSaveReq
import com.kra.paypoint.data.remote.model.auth.BhfUserSaveRes
import retrofit2.http.Body
import retrofit2.http.POST

interface AuthService {
    @POST("saveBhfUser")
    suspend fun saveBhfUser(@Body request: BhfUserSaveReq): BhfUserSaveRes
}
