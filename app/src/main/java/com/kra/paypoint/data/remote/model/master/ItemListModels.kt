package com.kra.paypoint.data.remote.model.master

import com.google.gson.annotations.SerializedName

data class ItemListReq(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String
)

data class ItemListRes(
    @SerializedName("resultCd") val resultCd: String?,
    @SerializedName("resultMsg") val resultMsg: String?,
    @SerializedName("resultDt") val resultDt: String?,
    @SerializedName("data") val data: ItemListData?
) {
    val isSuccess: Boolean get() = resultCd == "000"
}

data class ItemListData(
    @SerializedName("itemList") val itemList: List<RemoteItem> = emptyList()
)

data class RemoteItem(
    @SerializedName("itemCd") val itemCd: String,
    @SerializedName("itemClsCd") val itemClsCd: String,
    @SerializedName("itemTyCd") val itemTyCd: String,
    @SerializedName("itemNm") val itemNm: String,
    @SerializedName("bcd") val bcd: String? = null,
    @SerializedName("pkgUnitCd") val pkgUnitCd: String? = null,
    @SerializedName("qtyUnitCd") val qtyUnitCd: String? = null,
    @SerializedName("taxTyCd") val taxTyCd: String? = null,
    @SerializedName("dftPrc") val dftPrc: Double? = null,
    @SerializedName("useYn") val useYn: String? = null
)
