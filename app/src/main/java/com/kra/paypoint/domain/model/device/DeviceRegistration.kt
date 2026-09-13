package com.kra.paypoint.domain.model.device

/**
 * What device init returned — deliberately excludes the raw signing keys (signKey/intrlKey/
 * cmcKey). Those stay inside [com.kra.paypoint.data.repository.DeviceRepositoryImpl] /
 * [com.kra.paypoint.data.local.device.DeviceConfigStore] and are never handed to a ViewModel.
 */
data class DeviceRegistration(
    val tin: String,
    val branchId: String,
    val sdcId: String,
    val mrcNo: String,
    val lastSaleInvoiceNumber: Long,
    val lastPurchaseInvoiceNumber: Long,
    val lastReceiptNumber: Long,
    val lastZReportNumber: Long
)
