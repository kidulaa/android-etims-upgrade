package com.kra.paypoint.domain.exception

import com.google.gson.JsonParseException
import retrofit2.HttpException
import java.io.IOException
import java.net.ConnectException
import java.net.SocketTimeoutException
import java.net.UnknownHostException
import javax.net.ssl.SSLException

/**
 * Turns a failure into something an operator can act on, instead of a raw exception message
 * ("Unable to resolve host etims-api…"). Never includes request data, keys or credentials.
 */
fun Throwable.toUserMessage(): String = when (this) {
    is UnknownHostException, is ConnectException ->
        "No connection to KRA eTIMS. Check the internet connection — work is saved and will sync later."
    is SocketTimeoutException ->
        "KRA eTIMS took too long to respond. It will be retried automatically."
    is SSLException ->
        "Secure connection to KRA eTIMS failed. Check the device date and time, then retry."
    is HttpException -> when (code()) {
        401, 403 -> "eTIMS rejected this device's credentials. Re-initialize the device under Device Setting."
        in 500..599 -> "KRA eTIMS is unavailable right now (HTTP ${code()}). It will be retried automatically."
        else -> "KRA eTIMS refused the request (HTTP ${code()})."
    }
    is JsonParseException, is IllegalStateException ->
        message?.takeIf { it.startsWith("eTIMS") } ?: "KRA eTIMS sent an unexpected response. Please retry."
    is IOException -> "Network error talking to KRA eTIMS. It will be retried automatically."
    is PayPointException, is IllegalArgumentException -> message ?: "Invalid input."
    else -> message?.takeIf { it.isNotBlank() } ?: "Something went wrong. Please retry."
}
