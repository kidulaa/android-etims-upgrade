package com.kra.paypoint.hardware.printer

interface PrinterService {
    /** Paired Bluetooth devices, formatted as "Name [AA:BB:CC:DD:EE:FF]". */
    fun getPairedPrinters(): List<String>

    /** Connects to a printer by MAC address (or a "Name [MAC]" string from [getPairedPrinters]). */
    suspend fun connectToPrinter(deviceAddress: String): Boolean

    /** Prints the receipt journal for a transaction over the connected thermal printer. */
    suspend fun printReceipt(journalData: String, isReprint: Boolean)

    /** Renders an A4 invoice and hands it to the Android print framework (any registered print service). */
    suspend fun printInvoiceA4(invoiceData: String, isReprint: Boolean)

    fun isConnected(): Boolean

    suspend fun disconnect()
}
