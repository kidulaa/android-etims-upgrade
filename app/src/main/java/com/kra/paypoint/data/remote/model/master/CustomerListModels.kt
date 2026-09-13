package com.kra.paypoint.data.remote.model.master

import com.google.gson.annotations.SerializedName

data class CustomerListReq(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String
)

data class CustomerListRes(
    @SerializedName("resultCd") val resultCd: String?,
    @SerializedName("resultMsg") val resultMsg: String?,
    @SerializedName("resultDt") val resultDt: String?,
    @SerializedName("data") val data: CustomerListData?
) {
    val isSuccess: Boolean get() = resultCd == "000"
}

data class CustomerListData(
    @SerializedName("custList") val custList: List<RemoteCustomer> = emptyList()
)

data class RemoteCustomer(
    @SerializedName("custNo") val custNo: String? = null,
    @SerializedName("custTin") val custTin: String,
    @SerializedName("custNm") val custNm: String,
    @SerializedName("custBhfId") val custBhfId: String? = null,
    @SerializedName("adrs") val adrs: String? = null,
    @SerializedName("telNo") val telNo: String? = null,
    @SerializedName("email") val email: String? = null,
    @SerializedName("useYn") val useYn: String? = null
)
