package com.kra.paypoint.data.local.device

import com.kra.paypoint.data.remote.model.auth.DeviceInitData

/**
 * Encrypted-at-rest storage for this device's eTIMS signing keys and identity. These are the
 * actual security boundary for every fiscal receipt this till signs, so they never go through
 * plain SharedPreferences, Gson-in-a-string, or a Room column — only this store.
 */
interface DeviceConfigStore {
    fun save(data: DeviceInitData)
    fun isRegistered(): Boolean

    fun readTin(): String?
    fun readBhfId(): String?
    fun readSdcId(): String?
    fun readMrcNo(): String?
    fun readSignKey(): String?
    fun readIntrlKey(): String?
    fun readCmcKey(): String?
    fun readLastSaleInvoiceNumber(): Long
    fun readLastPurchaseInvoiceNumber(): Long
    fun readLastReceiptNumber(): Long
    fun readLastZReportNumber(): Long
}
