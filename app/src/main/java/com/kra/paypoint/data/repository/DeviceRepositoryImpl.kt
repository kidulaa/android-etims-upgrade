package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.device.DeviceConfigStore
import com.kra.paypoint.data.remote.api.AuthService
import com.kra.paypoint.data.remote.model.auth.DeviceInitReq
import com.kra.paypoint.domain.exception.DeviceNotRegisteredException
import com.kra.paypoint.domain.model.device.DeviceRegistration
import com.kra.paypoint.domain.repository.DeviceRepository
import com.kra.paypoint.domain.repository.SignedReceipt
import com.kra.paypoint.security.FiscalSignature
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.withContext
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class DeviceRepositoryImpl @Inject constructor(
    private val authService: AuthService,
    private val deviceConfigStore: DeviceConfigStore
) : DeviceRepository {

    private val _registration = MutableStateFlow(loadFromStore())
    override val registration: StateFlow<DeviceRegistration?> = _registration.asStateFlow()

    override suspend fun isRegistered(): Boolean = deviceConfigStore.isRegistered()

    override suspend fun registerDevice(tin: String, branchId: String): Result<DeviceRegistration> =
        withContext(Dispatchers.IO) {
            runCatching {
                val response = authService.initializeDevice(DeviceInitReq(tin = tin, bhfId = branchId))
                if (!response.isSuccess || response.data == null) {
                    throw IllegalStateException(
                        "Device registration rejected by eTIMS: ${response.resultCd} ${response.resultMsg.orEmpty()}"
                    )
                }
                deviceConfigStore.save(response.data)
                val registered = loadFromStore()
                    ?: error("Device config store returned nothing immediately after a successful save")
                _registration.value = registered
                registered
            }
        }

    override suspend fun signReceipt(receiptData: String): Result<SignedReceipt> = withContext(Dispatchers.IO) {
        runCatching {
            val signKey = deviceConfigStore.readSignKey() ?: throw DeviceNotRegisteredException()
            val intrlKey = deviceConfigStore.readIntrlKey() ?: throw DeviceNotRegisteredException()
            SignedReceipt(
                receiptSignature = FiscalSignature.receiptSignature(receiptData, signKey),
                internalData = FiscalSignature.internalData(receiptData, intrlKey)
            )
        }
    }

    private fun loadFromStore(): DeviceRegistration? {
        if (!deviceConfigStore.isRegistered()) return null
        val tin = deviceConfigStore.readTin() ?: return null
        val bhfId = deviceConfigStore.readBhfId() ?: return null
        return DeviceRegistration(
            tin = tin,
            branchId = bhfId,
            sdcId = deviceConfigStore.readSdcId().orEmpty(),
            mrcNo = deviceConfigStore.readMrcNo().orEmpty(),
            taxprNm = deviceConfigStore.readTaxprNm(),
            bsnsActv = deviceConfigStore.readBsnsActv(),
            bhfNm = deviceConfigStore.readBhfNm(),
            mgrNm = deviceConfigStore.readMgrNm(),
            mgrTelNo = deviceConfigStore.readMgrTelNo(),
            mgrEmail = deviceConfigStore.readMgrEmail(),
            locDesc = deviceConfigStore.readLocDesc(),
            lastSaleInvoiceNumber = deviceConfigStore.readLastSaleInvoiceNumber(),
            lastPurchaseInvoiceNumber = deviceConfigStore.readLastPurchaseInvoiceNumber(),
            lastReceiptNumber = deviceConfigStore.readLastReceiptNumber(),
            lastZReportNumber = deviceConfigStore.readLastZReportNumber()
        )
    }
}
