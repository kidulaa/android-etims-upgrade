package com.kra.paypoint.data.remote.model.master

import com.google.gson.annotations.SerializedName

data class SelectCodeListReq(
    val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    val lastReqDt: String
)

data class SelectCodeListRes(
    @SerializedName("resultCd") val resultCd: String,
    val resultMsg: String,
    val resultDt: String,
    val data: ReferenceCodeData?
) {
    val isSuccess: Boolean get() = resultCd == "000"
}

data class ReferenceCodeData(
    @SerializedName("clsList") val clsList: List<CodeCategory>
)

data class CodeCategory(
    val cdCls: String,
    val cdClsNm: String,
    val dtlList: List<ReferenceCode>
)

data class ReferenceCode(
    val cd: String,
    val cdNm: String,
    val cdDesc: String?,
    val useYn: String
)
