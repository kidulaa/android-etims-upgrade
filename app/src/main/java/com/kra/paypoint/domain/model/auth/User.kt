package com.kra.paypoint.domain.model.auth

data class User(
    val id: String,
    val name: String,
    val authorityCode: String,
    val branchId: String,
    val tin: String,
    val deviceSerial: String = "",
    val telNo: String? = null,
    val isActive: Boolean = true
) {
    val isAdmin: Boolean get() = authorityCode == ROLE_ADMIN

    companion object {
        /** Full access: Admin Menu (users, items, customers, settings) plus POS. */
        const val ROLE_ADMIN = "ROLE_ADMIN"
        /** POS only: sales, receipts, refunds, closeout. */
        const val ROLE_USER = "ROLE_USER"
    }
}
