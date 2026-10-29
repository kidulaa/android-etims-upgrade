package com.kra.paypoint.data.remote.model.transaction

import com.google.gson.annotations.SerializedName

/** eTIMS `insertTrnsPurchase` — a purchase invoice received from a supplier. */
data class TrnsPurchaseSaveReq(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    @SerializedName("invcNo") val invcNo: Long,
    @SerializedName("orgInvcNo") val orgInvcNo: Long = 0L,
    @SerializedName("spplrTin") val spplrTin: String?,
    @SerializedName("spplrBhfId") val spplrBhfId: String?,
    @SerializedName("spplrNm") val spplrNm: String?,
    @SerializedName("spplrInvcNo") val spplrInvcNo: String?,
    @SerializedName("regTyCd") val regTyCd: String = "M", // M: manual entry
    @SerializedName("pchsTyCd") val pchsTyCd: String = "N", // N: normal
    @SerializedName("rcptTyCd") val rcptTyCd: String = "P", // P: purchase
    @SerializedName("pmtTyCd") val pmtTyCd: String,
    @SerializedName("pchsSttsCd") val pchsSttsCd: String = "02", // 02: approved
    @SerializedName("cfmDt") val cfmDt: String,
    @SerializedName("pchsDt") val pchsDt: String,
    @SerializedName("wrhsDt") val wrhsDt: String?,
    @SerializedName("cnclReqDt") val cnclReqDt: String? = null,
    @SerializedName("cnclDt") val cnclDt: String? = null,
    @SerializedName("rfdDt") val rfdDt: String? = null,
    @SerializedName("totItemCnt") val totItemCnt: Int,
    @SerializedName("taxblAmtA") val taxblAmtA: Double,
    @SerializedName("taxblAmtB") val taxblAmtB: Double,
    @SerializedName("taxblAmtC") val taxblAmtC: Double,
    @SerializedName("taxblAmtD") val taxblAmtD: Double,
    @SerializedName("taxblAmtE") val taxblAmtE: Double,
    @SerializedName("taxRtA") val taxRtA: Int,
    @SerializedName("taxRtB") val taxRtB: Int,
    @SerializedName("taxRtC") val taxRtC: Int,
    @SerializedName("taxRtD") val taxRtD: Int,
    @SerializedName("taxRtE") val taxRtE: Int,
    @SerializedName("taxAmtA") val taxAmtA: Double,
    @SerializedName("taxAmtB") val taxAmtB: Double,
    @SerializedName("taxAmtC") val taxAmtC: Double,
    @SerializedName("taxAmtD") val taxAmtD: Double,
    @SerializedName("taxAmtE") val taxAmtE: Double,
    @SerializedName("totTaxblAmt") val totTaxblAmt: Double,
    @SerializedName("totTaxAmt") val totTaxAmt: Double,
    @SerializedName("totAmt") val totAmt: Double,
    @SerializedName("remark") val remark: String?,
    @SerializedName("regrId") val regrId: String,
    @SerializedName("regrNm") val regrNm: String,
    @SerializedName("modrId") val modrId: String,
    @SerializedName("modrNm") val modrNm: String,
    @SerializedName("itemList") val itemList: List<TrnsPurchaseItem>
)

data class TrnsPurchaseItem(
    @SerializedName("itemSeq") val itemSeq: Int,
    @SerializedName("itemCd") val itemCd: String,
    @SerializedName("itemClsCd") val itemClsCd: String,
    @SerializedName("itemNm") val itemNm: String,
    @SerializedName("bcd") val bcd: String?,
    @SerializedName("spplrItemClsCd") val spplrItemClsCd: String? = null,
    @SerializedName("spplrItemCd") val spplrItemCd: String? = null,
    @SerializedName("spplrItemNm") val spplrItemNm: String? = null,
    @SerializedName("pkgUnitCd") val pkgUnitCd: String,
    @SerializedName("pkg") val pkg: Double,
    @SerializedName("qtyUnitCd") val qtyUnitCd: String,
    @SerializedName("qty") val qty: Double,
    @SerializedName("prc") val prc: Double,
    @SerializedName("splyAmt") val splyAmt: Double,
    @SerializedName("dcRt") val dcRt: Double,
    @SerializedName("dcAmt") val dcAmt: Double,
    @SerializedName("taxblAmt") val taxblAmt: Double,
    @SerializedName("taxTyCd") val taxTyCd: String,
    @SerializedName("taxAmt") val taxAmt: Double,
    @SerializedName("totAmt") val totAmt: Double,
    @SerializedName("itemExprDt") val itemExprDt: String? = null
)
