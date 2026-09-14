package com.kra.paypoint.domain.exception

class DeviceNotRegisteredException(
    message: String = "This device has not completed eTIMS registration yet."
) : PayPointException(message)

class DeviceRegistrationException(
    message: String,
    cause: Throwable? = null
) : PayPointException(message, cause)
