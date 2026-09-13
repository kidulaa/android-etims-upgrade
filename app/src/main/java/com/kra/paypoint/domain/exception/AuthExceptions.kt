package com.kra.paypoint.domain.exception

class InvalidCredentialsException(message: String = "Invalid username or password.") : Exception(message)

class DeviceNotRegisteredException(
    message: String = "This device has not completed eTIMS registration yet."
) : Exception(message)
