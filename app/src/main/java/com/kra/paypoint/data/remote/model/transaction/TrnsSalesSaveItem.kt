package com.kra.paypoint.data.remote.model.transaction

import com.google.gson.annotations.SerializedName

data class TrnsSalesSaveItem(
    @SerializedName("itemSeq") val itemSeq: Int,
    @SerializedName("itemCd") val itemCd: String,
    @SerializedName("itemClsCd") val itemClsCd: String,
    @SerializedName("itemNm") val itemNm: String,
    @SerializedName("bcd") val bcd: String?,
    @SerializedName("pkgUnitCd") val pkgUnitCd: String,
    @SerializedName("pkg") val pkg: Double,
    @SerializedName("qtyUnitCd") val qtyUnitCd: String,
    @SerializedName("qty") val qty: Double,
    @SerializedName("prc") val prc: Double,
    @SerializedName("splyAmt") val splyAmt: Double,
    @SerializedName("dcRt") val dcRt: Double,
    @SerializedName("dcAmt") val dcAmt: Double,
    @SerializedName("isrccCd") val isrccCd: String?,
    @SerializedName("isrccNm") val isrccNm: String?,
    @SerializedName("isrcRt") val isrcRt: Double,
    @SerializedName("isrcAmt") val isrcAmt: Double,
    @SerializedName("taxTyCd") val taxTyCd: String,
    @SerializedName("taxblAmt") val taxblAmt: Double,
    @SerializedName("taxAmt") val taxAmt: Double,
    @SerializedName("totAmt") val totAmt: Double
)
