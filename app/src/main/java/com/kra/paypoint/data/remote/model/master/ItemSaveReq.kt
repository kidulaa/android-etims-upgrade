package com.kra.paypoint.data.remote.model.master

import com.google.gson.annotations.SerializedName

data class ItemSaveReq(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    @SerializedName("itemCd") val itemCd: String,
    @SerializedName("itemClsCd") val itemClsCd: String,
    @SerializedName("itemTyCd") val itemTyCd: String,
    @SerializedName("itemNm") val itemNm: String,
    @SerializedName("itemStdNm") val itemStdNm: String?,
    @SerializedName("orgnNatCd") val orgnNatCd: String?,
    @SerializedName("pkgUnitCd") val pkgUnitCd: String,
    @SerializedName("qtyUnitCd") val qtyUnitCd: String,
    @SerializedName("taxTyCd") val taxTyCd: String,
    @SerializedName("btchNo") val btchNo: String?,
    @SerializedName("bcd") val bcd: String?,
    @SerializedName("dftPrc") val dftPrc: Double,
    @SerializedName("grpPrcL1") val grpPrcL1: Double?,
    @SerializedName("grpPrcL2") val grpPrcL2: Double?,
    @SerializedName("grpPrcL3") val grpPrcL3: Double?,
    @SerializedName("grpPrcL4") val grpPrcL4: Double?,
    @SerializedName("grpPrcL5") val grpPrcL5: Double?,
    @SerializedName("addInfo") val addInfo: String?,
    @SerializedName("sftyQty") val sftyQty: Double?,
    @SerializedName("useYn") val useYn: String,
    @SerializedName("regBhfId") val regBhfId: String?,
    @SerializedName("rraModYn") val rraModYn: String?,
    @SerializedName("isrcAplcbYn") val isrcAplcbYn: String?,
    @SerializedName("batchNum") val batchNum: String?,
    @SerializedName("regrId") val regrId: String?,
    @SerializedName("regrNm") val regrNm: String?,
    @SerializedName("modrId") val modrId: String?,
    @SerializedName("modrNm") val modrNm: String?
)
