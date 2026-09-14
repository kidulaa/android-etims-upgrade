package com.kra.paypoint.data.remote.api

import com.kra.paypoint.data.remote.model.transaction.TrnsSalesRes
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesSaveReq
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesReceiptReq
import com.kra.paypoint.data.remote.model.transaction.TrnsSalesReceiptRes
import retrofit2.http.Body
import retrofit2.http.POST

interface TransactionService {
    @POST("saveTrnsSales")
    suspend fun saveSalesTransaction(@Body request: TrnsSalesSaveReq): TrnsSalesRes

    @POST("saveTrnsSalesReceipt")
    suspend fun saveSalesReceipt(@Body request: TrnsSalesReceiptReq): TrnsSalesReceiptRes
}
