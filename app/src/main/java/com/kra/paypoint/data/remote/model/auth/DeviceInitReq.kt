package com.kra.paypoint.data.remote.model.auth

import com.google.gson.annotations.SerializedName

/** eTIMS device initialization request ("selectInitInfo" in the legacy till). */
data class DeviceInitReq(
    @SerializedName("tin") val tin: String,
    @SerializedName("bhfId") val bhfId: String,
    @SerializedName("dvcId") val dvcId: String
)
