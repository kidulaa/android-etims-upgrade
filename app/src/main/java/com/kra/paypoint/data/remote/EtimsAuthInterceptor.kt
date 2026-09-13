package com.kra.paypoint.data.remote

import com.kra.paypoint.data.local.device.DeviceConfigStore
import okhttp3.Interceptor
import okhttp3.Response
import javax.inject.Inject
import javax.inject.Singleton

/**
 * Attaches the per-device cmcKey/tin/bhfId headers eTIMS expects on every call after device
 * initialization. Silently a no-op before registration (only `selectInitInfo` itself should
 * ever be called then) — this deliberately never blocks a request, since failing closed here
 * would turn a missing header into an unrelated-looking network error.
 */
@Singleton
class EtimsAuthInterceptor @Inject constructor(
    private val deviceConfigStore: DeviceConfigStore
) : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        val cmcKey = deviceConfigStore.readCmcKey()
        val tin = deviceConfigStore.readTin()
        val bhfId = deviceConfigStore.readBhfId()

        val request = chain.request().newBuilder().apply {
            if (cmcKey != null) addHeader("cmcKey", cmcKey)
            if (tin != null) addHeader("tin", tin)
            if (bhfId != null) addHeader("bhfId", bhfId)
        }.build()

        return chain.proceed(request)
    }
}
