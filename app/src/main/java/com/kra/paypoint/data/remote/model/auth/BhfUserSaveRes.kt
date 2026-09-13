package com.kra.paypoint.data.remote.model.auth

import com.google.gson.annotations.SerializedName

data class BhfUserSaveRes(
    @SerializedName("resultCd") val resultCd: String,
    @SerializedName("resultMsg") val resultMsg: String?,
    @SerializedName("resultDt") val resultDt: String?,
    @SerializedName("data") val data: Any?
)
