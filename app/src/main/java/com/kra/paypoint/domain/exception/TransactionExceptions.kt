package com.kra.paypoint.domain.exception

class EmptyTransactionException(
    message: String = "Cannot process a transaction with no items."
) : PayPointException(message)

class InvalidOriginalTransactionException(
    val invoiceNumber: Long,
    message: String = "Original invoice #$invoiceNumber not found or invalid for refund."
) : PayPointException(message)
