package com.kra.paypoint.data.remote.model.master

import com.google.gson.annotations.SerializedName

/** Envelope returned by eTIMS master-data save endpoints (`saveItem`, `saveBhfCustomer`). */
data class MasterSaveRes(
    @SerializedName("resultCd") val resultCd: String?,
    @SerializedName("resultMsg") val resultMsg: String?,
    @SerializedName("resultDt") val resultDt: String?
) {
    val isSuccess: Boolean get() = resultCd == "000"
}
