package com.kra.paypoint.domain.exception

import okhttp3.ResponseBody.Companion.toResponseBody
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Test
import retrofit2.HttpException
import retrofit2.Response
import java.net.SocketTimeoutException
import java.net.UnknownHostException

class UserMessagesTest {

    private fun http(code: Int) = HttpException(Response.error<Any>(code, "".toResponseBody(null)))

    @Test
    fun `offline and timeout map to actionable messages without raw host names`() {
        val offline = UnknownHostException("Unable to resolve host \"etims-api.kra.go.ke\"").toUserMessage()
        assertTrue(offline.startsWith("No connection"))
        assertFalse(offline.contains("etims-api"))
        assertTrue(SocketTimeoutException().toUserMessage().contains("too long"))
    }

    @Test
    fun `rejected device credentials point to re-initialisation`() {
        assertTrue(http(401).toUserMessage().contains("Re-initialize"))
        assertTrue(http(503).toUserMessage().contains("unavailable"))
    }

    @Test
    fun `validation messages pass through unchanged`() {
        assertEquals("Select the supplier.", IllegalArgumentException("Select the supplier.").toUserMessage())
        assertEquals("eTIMS rejected item list request: 910", IllegalStateException("eTIMS rejected item list request: 910").toUserMessage())
    }
}
