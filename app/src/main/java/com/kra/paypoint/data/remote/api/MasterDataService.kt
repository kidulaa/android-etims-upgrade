package com.kra.paypoint.data.remote.api

import com.kra.paypoint.data.remote.model.master.BhfCustSaveReq
import com.kra.paypoint.data.remote.model.master.ItemSaveReq
import retrofit2.http.Body
import retrofit2.http.POST

interface MasterDataService {
    @POST("saveItem")
    suspend fun saveItem(@Body request: ItemSaveReq): Any

    @POST("saveBhfCustomer")
    suspend fun saveBhfCustomer(@Body request: BhfCustSaveReq): Any
}
