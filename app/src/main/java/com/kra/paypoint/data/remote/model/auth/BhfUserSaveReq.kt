package com.kra.paypoint.data.remote.model.auth

import com.google.gson.annotations.SerializedName

data class BhfUserSaveReq(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    @SerializedName("userId") val userId: String,
    @SerializedName("userNm") val userNm: String,
    @SerializedName("pwd") val pwd: String?,
    @SerializedName("adrs") val adrs: String?,
    @SerializedName("cntc") val cntc: String?,
    @SerializedName("authCd") val authCd: String,
    @SerializedName("remark") val remark: String?,
    @SerializedName("useYn") val useYn: String,
    @SerializedName("regrId") val regrId: String?,
    @SerializedName("regrNm") val regrNm: String?,
    @SerializedName("modrId") val modrId: String?,
    @SerializedName("modrNm") val modrNm: String?
)
