package com.kra.paypoint.data.remote.model.auth

import com.google.gson.annotations.SerializedName

data class DeviceInitRes(
    @SerializedName("resultCd") val resultCd: String?,
    @SerializedName("resultMsg") val resultMsg: String?,
    @SerializedName("resultDt") val resultDt: String?,
    @SerializedName("data") val data: DeviceInitData?
) {
    val isSuccess: Boolean get() = resultCd == "000"
}

/** Mirrors the legacy `InitInfoVO` payload — branch identity, SDC identity, signing keys, and
 *  the counters a fresh/reinstalled device must resume numbering from (never reuse a number
 *  already sent to KRA). */
data class DeviceInitData(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    @SerializedName("sdcId") val sdcId: String?,
    @SerializedName("mrcNo") val mrcNo: String?,
    @SerializedName("signKey") val signKey: String,
    @SerializedName("intrlKey") val intrlKey: String,
    @SerializedName("cmcKey") val cmcKey: String,
    
    // Additional System Setting Fields
    @SerializedName("taxprNm") val taxprNm: String?,
    @SerializedName("bsnsActv") val bsnsActv: String?,
    @SerializedName("bhfNm") val bhfNm: String?,
    @SerializedName("mgrNm") val mgrNm: String?,
    @SerializedName("mgrTelNo") val mgrTelNo: String?,
    @SerializedName("mgrEmail") val mgrEmail: String?,
    @SerializedName("locDesc") val locDesc: String?,
    
    @SerializedName("lastSaleInvcNo") val lastSaleInvcNo: Long = 0L,
    @SerializedName("lastPchsInvcNo") val lastPchsInvcNo: Long = 0L,
    @SerializedName("lastRcptNo") val lastRcptNo: Long = 0L,
    @SerializedName("lastZreportRptNo") val lastZreportRptNo: Long = 0L
)
