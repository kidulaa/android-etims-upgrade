package com.kra.paypoint.data.remote.model.master

import com.google.gson.annotations.SerializedName

data class BhfCustSaveReq(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    @SerializedName("custNo") val custNo: String,
    @SerializedName("custTin") val custTin: String?,
    @SerializedName("custNid") val custNid: String?,
    @SerializedName("custNm") val custNm: String,
    @SerializedName("custBhfId") val custBhfId: String?,
    @SerializedName("adrs") val adrs: String?,
    @SerializedName("telNo") val telNo: String?,
    @SerializedName("email") val email: String?,
    @SerializedName("faxNo") val faxNo: String?,
    @SerializedName("useYn") val useYn: String,
    @SerializedName("remark") val remark: String?,
    @SerializedName("regrId") val regrId: String?,
    @SerializedName("regrNm") val regrNm: String?,
    @SerializedName("modrId") val modrId: String?,
    @SerializedName("modrNm") val modrNm: String?
)
