package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.device.DeviceConfigStore
import com.kra.paypoint.data.remote.api.AuthService
import com.kra.paypoint.data.remote.model.auth.DeviceInitData
import com.kra.paypoint.data.remote.model.auth.DeviceInitReq
import com.kra.paypoint.data.remote.model.auth.DeviceInitRes
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.kotlin.any
import org.mockito.kotlin.mock
import org.mockito.kotlin.whenever

/**
 * DeviceConfigStore wraps EncryptedSharedPreferences, which needs a real Android Keystore —
 * unavailable in a plain JVM unit test — so it's swapped here for an in-memory fake backed by
 * a plain HashMap "SharedPreferences", exercising DeviceRepositoryImpl's real logic without
 * needing Robolectric/instrumentation for this one dependency.
 */
class DeviceRepositoryImplTest {

    private lateinit var authService: AuthService
    private lateinit var deviceConfigStore: DeviceConfigStore
    private lateinit var repository: DeviceRepositoryImpl

    private val successData = DeviceInitData(
        tin = "P012345678X",
        bhfId = "00",
        sdcId = "SDC001",
        mrcNo = "MRC001",
        signKey = "MFRGGZDFMZTWQ2LK",
        intrlKey = "NBSWY3DPFQQHO33SNRSA",
        cmcKey = "OB2GK43UEB2GK43U",
        lastSaleInvcNo = 100L,
        lastPchsInvcNo = 10L,
        lastRcptNo = 100L,
        lastZreportRptNo = 5L
    )

    @Before
    fun setup() {
        authService = mock()
        deviceConfigStore = FakeDeviceConfigStore()
        repository = DeviceRepositoryImpl(authService, deviceConfigStore)
    }

    @Test
    fun `an unregistered device is not registered and cannot sign`() = runTest {
        assertFalse(repository.isRegistered())
        val result = repository.signReceipt("some-receipt-data")
        assertTrue(result.isFailure)
    }

    @Test
    fun `a successful device init persists keys and counters, and enables signing`() = runTest {
        whenever(authService.initializeDevice(any())).thenReturn(
            DeviceInitRes(resultCd = "000", resultMsg = "OK", resultDt = null, data = successData)
        )

        val result = repository.registerDevice(tin = "P012345678X", branchId = "00", deviceSerial = "SN123456")

        assertTrue(result.isSuccess)
        assertTrue(repository.isRegistered())
        assertEquals(100L, result.getOrThrow().lastSaleInvoiceNumber)

        val signed = repository.signReceipt("P012345678X|00|101|20260913|500.0")
        assertTrue(signed.isSuccess)
    }

    @Test
    fun `a business rejection from eTIMS does not register the device`() = runTest {
        whenever(authService.initializeDevice(any())).thenReturn(
            DeviceInitRes(resultCd = "999", resultMsg = "Unknown TIN", resultDt = null, data = null)
        )

        val result = repository.registerDevice(tin = "P000000000X", branchId = "00", deviceSerial = "SN123456")

        assertTrue(result.isFailure)
        assertFalse(repository.isRegistered())
    }
}

/** Minimal in-memory stand-in for the EncryptedSharedPreferences-backed store. */
private class FakeDeviceConfigStore : DeviceConfigStore {
    private val values = mutableMapOf<String, Any?>()

    override fun save(data: DeviceInitData) {
        values["tin"] = data.tin
        values["bhfId"] = data.bhfId
        values["sdcId"] = data.sdcId
        values["mrcNo"] = data.mrcNo
        values["signKey"] = data.signKey
        values["intrlKey"] = data.intrlKey
        values["cmcKey"] = data.cmcKey
        values["lastSaleInvcNo"] = data.lastSaleInvcNo
        values["lastPchsInvcNo"] = data.lastPchsInvcNo
        values["lastRcptNo"] = data.lastRcptNo
        values["lastZreportRptNo"] = data.lastZreportRptNo
    }

    override fun isRegistered(): Boolean = values.containsKey("cmcKey")
    override fun readTin(): String? = values["tin"] as String?
    override fun readBhfId(): String? = values["bhfId"] as String?
    override fun readSdcId(): String? = values["sdcId"] as String?
    override fun readMrcNo(): String? = values["mrcNo"] as String?
    override fun readTaxprNm(): String? = values["taxprNm"] as String?
    override fun readBsnsActv(): String? = values["bsnsActv"] as String?
    override fun readBhfNm(): String? = values["bhfNm"] as String?
    override fun readMgrNm(): String? = values["mgrNm"] as String?
    override fun readMgrTelNo(): String? = values["mgrTelNo"] as String?
    override fun readMgrEmail(): String? = values["mgrEmail"] as String?
    override fun readLocDesc(): String? = values["locDesc"] as String?
    override fun readSignKey(): String? = values["signKey"] as String?
    override fun readIntrlKey(): String? = values["intrlKey"] as String?
    override fun readCmcKey(): String? = values["cmcKey"] as String?
    override fun readLastSaleInvoiceNumber(): Long = values["lastSaleInvcNo"] as? Long ?: 0L
    override fun readLastPurchaseInvoiceNumber(): Long = values["lastPchsInvcNo"] as? Long ?: 0L
    override fun readLastReceiptNumber(): Long = values["lastRcptNo"] as? Long ?: 0L
    override fun readLastZReportNumber(): Long = values["lastZreportRptNo"] as? Long ?: 0L
}
