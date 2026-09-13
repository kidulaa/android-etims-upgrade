package com.kra.paypoint.domain.model.auth

data class User(
    val id: String,
    val name: String,
    val authorityCode: String,
    val branchId: String,
    val tin: String
)
