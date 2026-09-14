package com.kra.paypoint.domain.repository

import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import kotlinx.coroutines.flow.Flow

interface MasterDataRepository {
    fun getAllItems(): Flow<List<ItemEntity>>
    fun searchItems(query: String): Flow<List<ItemEntity>>
    suspend fun getItemByCode(itemCode: String): ItemEntity?
    suspend fun saveItem(item: ItemEntity)

    fun getAllCustomers(): Flow<List<CustomerEntity>>
    fun searchCustomers(query: String): Flow<List<CustomerEntity>>
    suspend fun getCustomerByTin(tin: String): CustomerEntity?
    suspend fun saveCustomer(customer: CustomerEntity)

    /** Pulls the branch item catalog from eTIMS (`selectItemList`) and persists it locally. */
    suspend fun syncItemsFromEtims(tin: String, bhfId: String): Result<Int>

    /** Pulls this branch's registered customers from eTIMS (`selectCustomerList`). */
    suspend fun syncCustomersFromEtims(tin: String, bhfId: String): Result<Int>

    /** Pulls reference code lists (tax types, unit codes, etc.) from eTIMS. */
    suspend fun syncReferenceCodesFromEtims(tin: String, bhfId: String): Result<Int>

    /** Pulls item classification standards from eTIMS. */
    suspend fun syncItemClassificationsFromEtims(tin: String, bhfId: String): Result<Int>
}
