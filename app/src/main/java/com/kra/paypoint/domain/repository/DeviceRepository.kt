package com.kra.paypoint.domain.repository

import com.kra.paypoint.domain.model.device.DeviceRegistration
import kotlinx.coroutines.flow.StateFlow

interface DeviceRepository {
    val registration: StateFlow<DeviceRegistration?>

    suspend fun isRegistered(): Boolean

    /** Calls eTIMS device initialization (`selectInitInfo`) and persists the returned keys/counters. */
    suspend fun registerDevice(tin: String, branchId: String, deviceSerial: String): Result<DeviceRegistration>

    /**
     * Signs a receipt data string using this device's provisioned keys.
     * Fails if [registerDevice] hasn't completed successfully yet.
     */
    suspend fun signReceipt(receiptData: String): Result<SignedReceipt>
}

data class SignedReceipt(
    val receiptSignature: String,
    val internalData: String
)
