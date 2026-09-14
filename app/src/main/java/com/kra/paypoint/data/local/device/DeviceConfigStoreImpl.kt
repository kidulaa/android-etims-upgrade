package com.kra.paypoint.data.local.device

import android.content.Context
import android.content.SharedPreferences
import androidx.security.crypto.EncryptedSharedPreferences
import androidx.security.crypto.MasterKey
import com.kra.paypoint.data.remote.model.auth.DeviceInitData
import dagger.hilt.android.qualifiers.ApplicationContext
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class DeviceConfigStoreImpl @Inject constructor(
    @ApplicationContext context: Context
) : DeviceConfigStore {

    private val prefs: SharedPreferences by lazy {
        val masterKey = MasterKey.Builder(context)
            .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
            .build()
        EncryptedSharedPreferences.create(
            context,
            "etims_device_config",
            masterKey,
            EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
            EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
        )
    }

    override fun save(data: DeviceInitData) {
        prefs.edit()
            .putString(KEY_TIN, data.tin)
            .putString(KEY_BHF_ID, data.bhfId)
            .putString(KEY_SDC_ID, data.sdcId.orEmpty())
            .putString(KEY_MRC_NO, data.mrcNo.orEmpty())
            .putString(KEY_TAXPR_NM, data.taxprNm)
            .putString(KEY_BSNS_ACTV, data.bsnsActv)
            .putString(KEY_BHF_NM, data.bhfNm)
            .putString(KEY_MGR_NM, data.mgrNm)
            .putString(KEY_MGR_TEL_NO, data.mgrTelNo)
            .putString(KEY_MGR_EMAIL, data.mgrEmail)
            .putString(KEY_LOC_DESC, data.locDesc)
            .putString(KEY_SIGN_KEY, data.signKey)
            .putString(KEY_INTRL_KEY, data.intrlKey)
            .putString(KEY_CMC_KEY, data.cmcKey)
            .putLong(KEY_LAST_SALE_INVC_NO, data.lastSaleInvcNo)
            .putLong(KEY_LAST_PCHS_INVC_NO, data.lastPchsInvcNo)
            .putLong(KEY_LAST_RCPT_NO, data.lastRcptNo)
            .putLong(KEY_LAST_ZREPORT_NO, data.lastZreportRptNo)
            .apply()
    }

    override fun isRegistered(): Boolean = prefs.contains(KEY_CMC_KEY)

    override fun readTin(): String? = prefs.getString(KEY_TIN, null)
    override fun readBhfId(): String? = prefs.getString(KEY_BHF_ID, null)
    override fun readSdcId(): String? = prefs.getString(KEY_SDC_ID, null)
    override fun readMrcNo(): String? = prefs.getString(KEY_MRC_NO, null)
    override fun readTaxprNm(): String? = prefs.getString(KEY_TAXPR_NM, null)
    override fun readBsnsActv(): String? = prefs.getString(KEY_BSNS_ACTV, null)
    override fun readBhfNm(): String? = prefs.getString(KEY_BHF_NM, null)
    override fun readMgrNm(): String? = prefs.getString(KEY_MGR_NM, null)
    override fun readMgrTelNo(): String? = prefs.getString(KEY_MGR_TEL_NO, null)
    override fun readMgrEmail(): String? = prefs.getString(KEY_MGR_EMAIL, null)
    override fun readLocDesc(): String? = prefs.getString(KEY_LOC_DESC, null)
    
    override fun readSignKey(): String? = prefs.getString(KEY_SIGN_KEY, null)
    override fun readIntrlKey(): String? = prefs.getString(KEY_INTRL_KEY, null)
    override fun readCmcKey(): String? = prefs.getString(KEY_CMC_KEY, null)
    override fun readLastSaleInvoiceNumber(): Long = prefs.getLong(KEY_LAST_SALE_INVC_NO, 0L)
    override fun readLastPurchaseInvoiceNumber(): Long = prefs.getLong(KEY_LAST_PCHS_INVC_NO, 0L)
    override fun readLastReceiptNumber(): Long = prefs.getLong(KEY_LAST_RCPT_NO, 0L)
    override fun readLastZReportNumber(): Long = prefs.getLong(KEY_LAST_ZREPORT_NO, 0L)

    private companion object {
        const val KEY_TIN = "tin"
        const val KEY_BHF_ID = "bhf_id"
        const val KEY_SDC_ID = "sdc_id"
        const val KEY_MRC_NO = "mrc_no"
        const val KEY_TAXPR_NM = "taxpr_nm"
        const val KEY_BSNS_ACTV = "bsns_actv"
        const val KEY_BHF_NM = "bhf_nm"
        const val KEY_MGR_NM = "mgr_nm"
        const val KEY_MGR_TEL_NO = "mgr_tel_no"
        const val KEY_MGR_EMAIL = "mgr_email"
        const val KEY_LOC_DESC = "loc_desc"
        const val KEY_SIGN_KEY = "sign_key"
        const val KEY_INTRL_KEY = "intrl_key"
        const val KEY_CMC_KEY = "cmc_key"
        const val KEY_LAST_SALE_INVC_NO = "last_sale_invc_no"
        const val KEY_LAST_PCHS_INVC_NO = "last_pchs_invc_no"
        const val KEY_LAST_RCPT_NO = "last_rcpt_no"
        const val KEY_LAST_ZREPORT_NO = "last_zreport_no"
    }
}
