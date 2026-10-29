package com.kra.paypoint.domain.usecase

import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.domain.model.EtimsCodes

/** Returns a user-facing reason a record would be rejected, or null when it's valid. */
object MasterDataValidator {

    const val CUSTOMER_INDIVIDUAL = "INDIVIDUAL"
    const val CUSTOMER_CORPORATE = "CORPORATE"

    fun validateItem(item: ItemEntity, beginningStock: Double = 0.0): String? = when {
        item.itemName.isBlank() -> "Item name is required."
        item.itemClassificationCode.isBlank() -> "Item classification code is required."
        item.itemTypeCode.isBlank() -> "Item type is required."
        item.packagingUnitCode.isBlank() -> "Packaging unit is required."
        item.quantityUnitCode.isBlank() -> "Quantity unit is required."
        item.originNationCode.length != 2 -> "Origin must be a 2-letter country code."
        item.taxTypeCode !in TaxCalculationEngine.BANDS -> "Select a valid tax type."
        item.defaultUnitPrice < 0.0 -> "Sale price cannot be negative."
        beginningStock < 0.0 -> "Beginning stock cannot be negative."
        else -> null
    }

    /**
     * Per the KRA eTIMS guide, non-individual buyers must be registered as Corporate — and a
     * KRA PIN encodes that: "A…" for individuals, "P…" for non-individuals.
     */
    fun validateCustomer(customer: CustomerEntity): String? {
        val pin = customer.customerTin.trim().uppercase()
        return when {
            customer.customerName.isBlank() -> "Customer name is required."
            !EtimsCodes.isValidKraPin(pin) -> "Enter a valid KRA PIN (e.g. A012345678Z)."
            customer.customerType == CUSTOMER_INDIVIDUAL && !pin.startsWith("A") ->
                "Individual customers have PINs starting with 'A'. Use Corporate for non-individuals."
            customer.customerType == CUSTOMER_CORPORATE && !pin.startsWith("P") ->
                "Corporate (non-individual) customers have PINs starting with 'P'."
            !customer.email.isNullOrBlank() && !EMAIL.matches(customer.email.trim()) -> "Enter a valid email address."
            !customer.telNo.isNullOrBlank() && !PHONE.matches(customer.telNo.trim()) -> "Enter a valid phone number."
            else -> null
        }
    }

    fun customerTypeForPin(pin: String): String =
        if (pin.trim().uppercase().startsWith("P")) CUSTOMER_CORPORATE else CUSTOMER_INDIVIDUAL

    private val EMAIL = Regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")
    private val PHONE = Regex("^\\+?[0-9 ]{9,15}$")
}
