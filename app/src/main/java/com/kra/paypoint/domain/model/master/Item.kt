package com.kra.paypoint.domain.model.master

data class Item(
    val itemCode: String,
    val itemClassificationCode: String,
    val itemTypeCode: String,
    val itemName: String,
    val packagingUnitCode: String,
    val quantityUnitCode: String,
    val taxTypeCode: String,
    val defaultUnitPrice: Double,
    val isUsed: Boolean
)
