package com.kra.paypoint.data.remote.model.transaction

import com.google.gson.annotations.SerializedName

/**
 * eTIMS response envelope. Every submission endpoint returns a `resultCd` — "000" is the
 * only success code; anything else (including a network-level 200 with a business
 * rejection) means the invoice was NOT accepted and must stay PENDING/FAILED, not SYNCED.
 * The old integration ignored this entirely (`TransactionService` returned `Any`).
 */
data class TrnsSalesRes(
    @SerializedName("resultCd") val resultCd: String?,
    @SerializedName("resultMsg") val resultMsg: String?,
    @SerializedName("resultDt") val resultDt: String?,
    @SerializedName("data") val data: TrnsSalesResData?
) {
    val isSuccess: Boolean get() = resultCd == RESULT_SUCCESS

    companion object {
        const val RESULT_SUCCESS = "000"
    }
}

data class TrnsSalesResData(
    @SerializedName("rcptNo") val rcptNo: Long? = null,
    @SerializedName("intrlData") val intrlData: String? = null,
    @SerializedName("rcptSign") val rcptSign: String? = null,
    @SerializedName("totRcptNo") val totRcptNo: Long? = null,
    @SerializedName("sdcId") val sdcId: String? = null,
    @SerializedName("mrcNo") val mrcNo: String? = null
)
