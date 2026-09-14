package com.kra.paypoint.domain.exception

/**
 * Base class for all domain-specific exceptions in the PayPoint application.
 * Using a sealed class allows for exhaustive error handling in the UI layer.
 */
sealed class PayPointException(
    message: String,
    cause: Throwable? = null
) : Exception(message, cause)
