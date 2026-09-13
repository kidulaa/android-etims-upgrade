package com.kra.paypoint.data.remote.api

import com.kra.paypoint.data.remote.model.auth.BhfUserSaveReq
import com.kra.paypoint.data.remote.model.auth.BhfUserSaveRes
import com.kra.paypoint.data.remote.model.auth.DeviceInitReq
import com.kra.paypoint.data.remote.model.auth.DeviceInitRes
import retrofit2.http.Body
import retrofit2.http.POST

interface AuthService {
    /**
     * Device initialization — issues this device's signing keys (signKey/intrlKey/cmcKey),
     * SDC identity, and the invoice/receipt/Z-report sequence counters to resume from. Must
     * succeed before any sale can be signed or submitted; see DeviceRepository.
     */
    @POST("selectInitInfo")
    suspend fun initializeDevice(@Body request: DeviceInitReq): DeviceInitRes

    @POST("saveBhfUser")
    suspend fun saveBhfUser(@Body request: BhfUserSaveReq): BhfUserSaveRes
}
