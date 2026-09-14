package com.kra.paypoint.domain.exception

class ItemNotFoundException(
    val itemCode: String,
    message: String = "Item with code '$itemCode' not found."
) : PayPointException(message)

class InsufficientStockException(
    val itemCode: String,
    val requested: Double,
    val available: Double,
    message: String = "Cannot deduct $requested units of '$itemCode'; only $available units in stock."
) : PayPointException(message)
