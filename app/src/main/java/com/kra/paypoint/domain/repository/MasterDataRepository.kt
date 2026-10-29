package com.kra.paypoint.domain.repository

import com.kra.paypoint.data.local.entity.CodeEntity
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemClsEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.domain.model.auth.User
import kotlinx.coroutines.flow.Flow

/** Outcome of a local save that is also pushed to eTIMS; [pushError] is null when KRA accepted it. */
data class MasterSaveOutcome(val pushError: String?) {
    val registeredWithEtims: Boolean get() = pushError == null
}

interface MasterDataRepository {
    fun getAllItems(): Flow<List<ItemEntity>>
    fun getAllItemsIncludingUnused(): Flow<List<ItemEntity>>
    fun searchItems(query: String): Flow<List<ItemEntity>>
    suspend fun getItemByCode(itemCode: String): ItemEntity?

    fun getAllCustomers(): Flow<List<CustomerEntity>>
    fun getAllCustomersIncludingUnused(): Flow<List<CustomerEntity>>
    fun searchCustomers(query: String): Flow<List<CustomerEntity>>
    suspend fun getCustomerByTin(tin: String): CustomerEntity?

    /** Reference codes for one `selectCodeList` class (see EtimsCodes.CodeClass). */
    fun observeCodes(classCode: String): Flow<List<CodeEntity>>
    fun searchItemClassifications(query: String): Flow<List<ItemClsEntity>>
    suspend fun getItemClassification(code: String): ItemClsEntity?

    /** Next free KRA item code: origin + item type + packaging unit + quantity unit + 7-digit sequence. */
    suspend fun generateItemCode(originNationCode: String, itemTypeCode: String, packagingUnitCode: String, quantityUnitCode: String): String

    /**
     * Saves a new item locally, books [beginningStock] as an opening stock-in movement, then
     * registers it with eTIMS (`saveItem`). A failed push leaves the item PENDING for the
     * background sync to retry; the item is usable for sales either way.
     */
    suspend fun registerItem(item: ItemEntity, beginningStock: Double, operator: User): Result<MasterSaveOutcome>

    /** Updates item master fields (never stock — that only moves through Inventory) and re-pushes to eTIMS. */
    suspend fun updateItem(item: ItemEntity, operator: User): Result<MasterSaveOutcome>

    /** Validates the KRA PIN against the customer type, saves locally, then registers with eTIMS (`saveBhfCustomer`). */
    suspend fun registerCustomer(customer: CustomerEntity, operator: User): Result<MasterSaveOutcome>

    /** Retries eTIMS registration for items/customers still PENDING. Returns how many were accepted. */
    suspend fun pushPendingMasterData(tin: String, bhfId: String): Int

    /** Pulls the branch item catalog from eTIMS (`selectItemList`) and persists it locally. */
    suspend fun syncItemsFromEtims(tin: String, bhfId: String): Result<Int>

    /** Pulls this branch's registered customers from eTIMS (`selectCustomerList`). */
    suspend fun syncCustomersFromEtims(tin: String, bhfId: String): Result<Int>

    /** Pulls reference code lists (tax types, unit codes, etc.) from eTIMS. */
    suspend fun syncReferenceCodesFromEtims(tin: String, bhfId: String): Result<Int>

    /** Pulls item classification standards from eTIMS. */
    suspend fun syncItemClassificationsFromEtims(tin: String, bhfId: String): Result<Int>
}
