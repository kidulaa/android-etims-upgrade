package com.kra.paypoint.data.remote.model.transaction

import com.google.gson.annotations.SerializedName

/**
 * Request model for saving transaction receipt details to eTIMS.
 * Mirrors the `saveTrnsSalesReceipt` API specification.
 */
data class TrnsSalesReceiptReq(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    @SerializedName("invcNo") val invcNo: Long,
    @SerializedName("orgInvcNo") val orgInvcNo: Long,
    @SerializedName("curRcptNo") val curRcptNo: Long,
    @SerializedName("totRcptNo") val totRcptNo: Long,
    @SerializedName("custTin") val custTin: String?,
    @SerializedName("custMblNo") val custMblNo: String?,
    @SerializedName("rptNo") val rptNo: Long,
    @SerializedName("rcptPbctDt") val rcptPbctDt: String,
    @SerializedName("intrlData") val intrlData: String?,
    @SerializedName("rcptSign") val rcptSign: String?,
    @SerializedName("jrnl") val jrnl: String?,
    @SerializedName("trdeNm") val trdeNm: String?,
    @SerializedName("adrs") val adrs: String?,
    @SerializedName("topMsg") val topMsg: String?,
    @SerializedName("btmMsg") val btmMsg: String?,
    @SerializedName("prchrAcptcYn") val prchrAcptcYn: String = "N",
    @SerializedName("regrId") val regrId: String,
    @SerializedName("regrNm") val regrNm: String,
    @SerializedName("modrId") val modrId: String,
    @SerializedName("modrNm") val modrNm: String
)

data class TrnsSalesReceiptRes(
    @SerializedName("resultCd") val resultCd: String,
    @SerializedName("resultMsg") val resultMsg: String,
    @SerializedName("resultDt") val resultDt: String,
    @SerializedName("data") val data: Any? = null
) {
    val isSuccess: Boolean get() = resultCd == "000"
}
