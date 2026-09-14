package com.kra.paypoint.domain.exception

class InvalidCredentialsException(
    message: String = "Invalid username or password."
) : PayPointException(message)

class InactiveUserException(
    val username: String,
    message: String = "User account '$username' is deactivated."
) : PayPointException(message)

class UserAlreadyExistsException(
    val username: String,
    message: String = "User with username '$username' already exists."
) : PayPointException(message)
