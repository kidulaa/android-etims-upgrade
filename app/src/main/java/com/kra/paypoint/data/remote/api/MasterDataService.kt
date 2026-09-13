package com.kra.paypoint.data.remote.api

import com.kra.paypoint.data.remote.model.master.BhfCustSaveReq
import com.kra.paypoint.data.remote.model.master.CustomerListReq
import com.kra.paypoint.data.remote.model.master.CustomerListRes
import com.kra.paypoint.data.remote.model.master.ItemListReq
import com.kra.paypoint.data.remote.model.master.ItemListRes
import com.kra.paypoint.data.remote.model.master.ItemSaveReq
import retrofit2.http.Body
import retrofit2.http.POST

interface MasterDataService {
    /** Pulls this branch's item catalog from eTIMS to seed/refresh the local POS. */
    @POST("selectItemList")
    suspend fun selectItemList(@Body request: ItemListReq): ItemListRes

    /** Pulls this branch's registered customers from eTIMS. */
    @POST("selectCustomerList")
    suspend fun selectCustomerList(@Body request: CustomerListReq): CustomerListRes

    @POST("saveItem")
    suspend fun saveItem(@Body request: ItemSaveReq): Any

    @POST("saveBhfCustomer")
    suspend fun saveBhfCustomer(@Body request: BhfCustSaveReq): Any
}
