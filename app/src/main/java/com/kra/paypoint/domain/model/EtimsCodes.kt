package com.kra.paypoint.domain.model

/**
 * KRA eTIMS (OSCU/VSCU) code tables the app depends on directly. Code lists synced via
 * `selectCodeList` are authoritative and preferred by the UI; the fallbacks here only exist so
 * an admin can still register items/customers before the first successful sync.
 */
object EtimsCodes {

    data class Code(val code: String, val name: String)

    /** `selectCodeList` class codes (cdCls). */
    object CodeClass {
        const val TAX_TYPE = "04"
        const val COUNTRY = "05"
        const val PAYMENT_TYPE = "07"
        const val QUANTITY_UNIT = "10"
        const val PACKAGING_UNIT = "17"
        const val ITEM_TYPE = "24"
    }

    /** KRA tax bands — note A is *Exempt* and B is the 16% standard rate. */
    object TaxType {
        const val EXEMPT = "A"
        const val STANDARD = "B"
        const val ZERO_RATED = "C"
        const val NON_VAT = "D"
        const val REDUCED = "E"

        val all = listOf(
            Code(EXEMPT, "A-Exempt"),
            Code(STANDARD, "B-16.00%"),
            Code(ZERO_RATED, "C-0%"),
            Code(NON_VAT, "D-Non-VAT"),
            Code(REDUCED, "E-8%")
        )

        fun label(code: String): String = all.firstOrNull { it.code == code }?.name ?: code
    }

    /** `pmtTyCd` values. */
    object PaymentType {
        const val CASH = "01"
        const val CREDIT = "02"
        const val CASH_CREDIT = "03"
        const val BANK_CHECK = "04"
        const val CARD = "05"
        const val MOBILE_MONEY = "06"
        const val OTHER = "07"

        /** The modes offered at the till: the guide's cash / mobile wallet, plus card and pay-later credit. */
        val tillModes = listOf(
            Code(CASH, "Cash"),
            Code(MOBILE_MONEY, "Mobile"),
            Code(CARD, "Card"),
            Code(CREDIT, "Credit")
        )

        fun label(code: String): String = when (code) {
            CASH -> "Cash"
            CREDIT -> "Credit"
            CASH_CREDIT -> "Cash/Credit"
            BANK_CHECK -> "Bank Cheque"
            CARD -> "Card"
            MOBILE_MONEY -> "Mobile Wallet"
            else -> "Other"
        }
    }

    /** `salesTyCd` values. */
    object SalesType {
        const val NORMAL = "N"
    }

    /** `rcptTyCd` values. */
    object ReceiptType {
        const val SALE = "S"
        const val CREDIT_NOTE = "R"
    }

    object ItemType {
        val fallback = listOf(
            Code("1", "Raw Material"),
            Code("2", "Finished Product"),
            Code("3", "Service")
        )
    }

    val fallbackPackagingUnits = listOf(
        Code("NT", "Net"),
        Code("BG", "Bag"),
        Code("BX", "Box"),
        Code("CT", "Carton"),
        Code("CA", "Can")
    )

    val fallbackQuantityUnits = listOf(
        Code("U", "Pieces/Item"),
        Code("KG", "Kilogram")
    )

    val fallbackCountries = listOf(
        Code("KE", "Kenya"),
        Code("UG", "Uganda"),
        Code("TZ", "Tanzania"),
        Code("RW", "Rwanda"),
        Code("CN", "China"),
        Code("IN", "India")
    )

    /**
     * KRA PIN: one letter, nine digits, one letter. Individuals' PINs start with "A",
     * non-individuals' (companies, partnerships, ...) with "P".
     */
    private val KRA_PIN = Regex("^[AP]\\d{9}[A-Z]$")

    fun isValidKraPin(pin: String): Boolean = KRA_PIN.matches(pin.trim().uppercase())
}
