package com.kra.paypoint.data.remote.api

import com.kra.paypoint.data.remote.model.transaction.TrnsSalesRes
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import retrofit2.http.Body
import retrofit2.http.POST

interface TransactionService {
    @POST("saveTrnsSalesOsdc")
    suspend fun saveSalesTransaction(@Body request: TrnsSalesSaveReq): TrnsSalesRes
}
