package com.kra.paypoint.data.remote.model.transaction

import com.google.gson.annotations.SerializedName

data class TrnsSalesSaveReq(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    @SerializedName("invcNo") val invcNo: Long,
    @SerializedName("orgInvcNo") val orgInvcNo: Long,
    @SerializedName("custTin") val custTin: String?,
    @SerializedName("custNm") val custNm: String?,
    @SerializedName("salesTyCd") val salesTyCd: String,
    @SerializedName("rcptTyCd") val rcptTyCd: String,
    @SerializedName("pmtTyCd") val pmtTyCd: String,
    @SerializedName("rfdRsnCd") val rfdRsnCd: String?,
    @SerializedName("salesSttsCd") val salesSttsCd: String,
    @SerializedName("cfmDt") val cfmDt: String?,
    @SerializedName("salesDt") val salesDt: String,
    @SerializedName("stockRlsDt") val stockRlsDt: String?,
    @SerializedName("cnclReqDt") val cnclReqDt: String?,
    @SerializedName("cnclDt") val cnclDt: String?,
    @SerializedName("rfdDt") val rfdDt: String?,
    @SerializedName("totItemCnt") val totItemCnt: Double,
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
    @SerializedName("prchrAcptcYn") val prchrAcptcYn: String,
    @SerializedName("remark") val remark: String?,
    @SerializedName("regrId") val regrId: String?,
    @SerializedName("regrNm") val regrNm: String?,
    @SerializedName("modrId") val modrId: String?,
    @SerializedName("modrNm") val modrNm: String?,
    @SerializedName("itemList") val itemList: List<TrnsSalesSaveItem>,
    // Computed client-side once the invoice number is known — see FiscalSignature and
    // DeviceRepository.signReceipt. Null only if this device hasn't completed eTIMS
    // registration yet (see TransactionRepositoryImpl.processSale).
    @SerializedName("rcptSign") val rcptSign: String? = null,
    @SerializedName("intrlData") val intrlData: String? = null
)
