package com.kra.paypoint.data.remote.api

import com.kra.paypoint.data.remote.model.master.*
import retrofit2.http.Body
import retrofit2.http.POST

interface MasterDataService {
    /** Pulls this branch's item catalog from eTIMS to seed/refresh the local POS. */
    @POST("selectItemList")
    suspend fun selectItemList(@Body request: ItemListReq): ItemListRes

    /** Pulls this branch's registered customers from eTIMS. */
    @POST("selectCustomerList")
    suspend fun selectCustomerList(@Body request: CustomerListReq): CustomerListRes

    /** Pulls reference code lists (tax types, unit codes, etc.) from eTIMS. */
    @POST("selectCodeList")
    suspend fun selectCodeList(@Body request: SelectCodeListReq): SelectCodeListRes

    /** Pulls item classification standards from eTIMS. */
    @POST("selectItemClsList")
    suspend fun selectItemClsList(@Body request: ItemClsListReq): ItemClsListRes

    @POST("saveItem")
    suspend fun saveItem(@Body request: ItemSaveReq): Any

    @POST("saveBhfCustomer")
    suspend fun saveBhfCustomer(@Body request: BhfCustSaveReq): Any
}
