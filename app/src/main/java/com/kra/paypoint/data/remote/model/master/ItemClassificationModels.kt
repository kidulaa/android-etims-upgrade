package com.kra.paypoint.data.remote.model.master

import com.google.gson.annotations.SerializedName

data class ItemClsListReq(
    val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    val lastReqDt: String
)

data class ItemClsListRes(
    @SerializedName("resultCd") val resultCd: String?,
    @SerializedName("resultMsg") val resultMsg: String?,
    @SerializedName("resultDt") val resultDt: String?,
    @SerializedName("data") val data: ItemClsData?
) {
    val isSuccess: Boolean get() = resultCd == "000"
}

data class ItemClsData(
    @SerializedName("itemClsList") val itemClsList: List<ItemClassification>
)

data class ItemClassification(
    @SerializedName("itemClsCd") val itemClsCd: String,
    @SerializedName("itemClsNm") val itemClsNm: String,
    @SerializedName("itemClsLvl") val itemClsLvl: Int?,
    @SerializedName("taxTyCd") val taxTyCd: String?,
    @SerializedName("useYn") val useYn: String?
)
