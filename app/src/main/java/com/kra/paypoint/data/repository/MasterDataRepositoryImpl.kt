package com.kra.paypoint.data.repository

import com.kra.paypoint.domain.exception.toUserMessage
import com.kra.paypoint.data.local.dao.CodeDao
import com.kra.paypoint.data.local.dao.CustomerDao
import com.kra.paypoint.data.local.dao.ItemClsDao
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.entity.CodeEntity
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemClsEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.data.remote.api.MasterDataService
import com.kra.paypoint.data.remote.model.master.*
import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.InventoryRepository
import com.kra.paypoint.domain.repository.MasterDataRepository
import com.kra.paypoint.domain.repository.MasterSaveOutcome
import com.kra.paypoint.domain.usecase.ItemCodeGenerator
import com.kra.paypoint.domain.usecase.MasterDataValidator
import kotlinx.coroutines.flow.Flow
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class MasterDataRepositoryImpl @Inject constructor(
    private val itemDao: ItemDao,
    private val customerDao: CustomerDao,
    private val codeDao: CodeDao,
    private val itemClsDao: ItemClsDao,
    private val masterDataService: MasterDataService,
    private val inventoryRepository: InventoryRepository
) : MasterDataRepository {

    override fun getAllItems(): Flow<List<ItemEntity>> {
        return itemDao.getAllItems()
    }

    override fun getAllItemsIncludingUnused(): Flow<List<ItemEntity>> {
        return itemDao.getAllItemsIncludingUnused()
    }

    override fun searchItems(query: String): Flow<List<ItemEntity>> {
        return itemDao.searchItems(query)
    }

    override suspend fun getItemByCode(itemCode: String): ItemEntity? {
        return itemDao.getItemByCode(itemCode)
    }

    override fun getAllCustomers(): Flow<List<CustomerEntity>> {
        return customerDao.getAllCustomers()
    }

    override fun getAllCustomersIncludingUnused(): Flow<List<CustomerEntity>> {
        return customerDao.getAllCustomersIncludingUnused()
    }

    override fun searchCustomers(query: String): Flow<List<CustomerEntity>> {
        return customerDao.searchCustomers(query)
    }

    override suspend fun getCustomerByTin(tin: String): CustomerEntity? {
        return customerDao.getCustomerByTin(tin)
    }

    override fun observeCodes(classCode: String): Flow<List<CodeEntity>> {
        return codeDao.getCodesByClass(classCode)
    }

    override fun searchItemClassifications(query: String): Flow<List<ItemClsEntity>> {
        return itemClsDao.search(query)
    }

    override suspend fun getItemClassification(code: String): ItemClsEntity? {
        return itemClsDao.getByCode(code)
    }

    override suspend fun generateItemCode(
        originNationCode: String,
        itemTypeCode: String,
        packagingUnitCode: String,
        quantityUnitCode: String
    ): String {
        val prefix = ItemCodeGenerator.prefix(originNationCode, itemTypeCode, packagingUnitCode, quantityUnitCode)
        return ItemCodeGenerator.next(prefix, itemDao.getItemCodesWithPrefix(prefix))
    }

    override suspend fun registerItem(item: ItemEntity, beginningStock: Double, operator: User): Result<MasterSaveOutcome> = runCatching {
        MasterDataValidator.validateItem(item, beginningStock)?.let { throw IllegalArgumentException(it) }
        require(item.itemCode.isNotBlank()) { "Item code is required." }
        if (itemDao.getItemByCode(item.itemCode) != null) {
            throw IllegalArgumentException("Item code ${item.itemCode} already exists.")
        }

        // Stock starts at zero and the opening balance goes through Inventory, so it shows up
        // as an auditable movement rather than an unexplained starting number.
        val toSave = item.copy(stockQuantity = 0.0, syncStatus = STATUS_PENDING, updatedAt = System.currentTimeMillis())
        itemDao.insertItem(toSave)
        if (beginningStock > 0.0) {
            inventoryRepository.recordStockIn(
                itemCode = toSave.itemCode,
                quantity = beginningStock,
                reasonCode = "01",
                reasonDescription = "Beginning Stock",
                operatorId = operator.id
            ).getOrThrow()
        }
        MasterSaveOutcome(pushItem(toSave, operator.tin, operator.branchId, operator))
    }

    override suspend fun updateItem(item: ItemEntity, operator: User): Result<MasterSaveOutcome> = runCatching {
        MasterDataValidator.validateItem(item)?.let { throw IllegalArgumentException(it) }
        val existing = itemDao.getItemByCode(item.itemCode)
            ?: throw IllegalArgumentException("Item ${item.itemCode} not found.")
        val toSave = item.copy(
            stockQuantity = existing.stockQuantity,
            syncStatus = STATUS_PENDING,
            updatedAt = System.currentTimeMillis()
        )
        itemDao.updateItem(toSave)
        MasterSaveOutcome(pushItem(toSave, operator.tin, operator.branchId, operator))
    }

    override suspend fun registerCustomer(customer: CustomerEntity, operator: User): Result<MasterSaveOutcome> = runCatching {
        val normalized = customer.copy(
            customerTin = customer.customerTin.trim().uppercase(),
            customerName = customer.customerName.trim(),
            telNo = customer.telNo?.trim()?.takeIf { it.isNotEmpty() },
            email = customer.email?.trim()?.takeIf { it.isNotEmpty() },
            branchId = operator.branchId,
            syncStatus = STATUS_PENDING,
            updatedAt = System.currentTimeMillis()
        )
        MasterDataValidator.validateCustomer(normalized)?.let { throw IllegalArgumentException(it) }
        val toSave = normalized.copy(customerNo = normalized.customerNo.ifBlank { normalized.customerTin })
        customerDao.insertCustomer(toSave)
        MasterSaveOutcome(pushCustomer(toSave, operator.tin, operator.branchId, operator))
    }

    override suspend fun pushPendingMasterData(tin: String, bhfId: String): Int {
        var accepted = 0
        for (item in itemDao.getUnsyncedItems()) {
            if (pushItem(item, tin, bhfId, operator = null) == null) accepted++
        }
        for (customer in customerDao.getUnsyncedCustomers()) {
            if (pushCustomer(customer, tin, bhfId, operator = null) == null) accepted++
        }
        return accepted
    }

    /** Returns null when eTIMS accepted the item, else why it didn't (the item stays PENDING). */
    private suspend fun pushItem(item: ItemEntity, tin: String, bhfId: String, operator: User?): String? {
        val error = try {
            val response = masterDataService.saveItem(
                ItemSaveReq(
                    tin = tin,
                    bhfId = bhfId,
                    itemCd = item.itemCode,
                    itemClsCd = item.itemClassificationCode,
                    itemTyCd = item.itemTypeCode,
                    itemNm = item.itemName,
                    itemStdNm = null,
                    orgnNatCd = item.originNationCode,
                    pkgUnitCd = item.packagingUnitCode,
                    qtyUnitCd = item.quantityUnitCode,
                    taxTyCd = item.taxTypeCode,
                    btchNo = null,
                    bcd = item.barcode,
                    dftPrc = item.defaultUnitPrice,
                    grpPrcL1 = item.defaultUnitPrice,
                    grpPrcL2 = item.defaultUnitPrice,
                    grpPrcL3 = item.defaultUnitPrice,
                    grpPrcL4 = item.defaultUnitPrice,
                    grpPrcL5 = null,
                    addInfo = item.description,
                    sftyQty = null,
                    useYn = if (item.isUsed) "Y" else "N",
                    regBhfId = bhfId,
                    rraModYn = "N",
                    isrcAplcbYn = "N",
                    batchNum = null,
                    regrId = operator?.id ?: SYSTEM_USER,
                    regrNm = operator?.name ?: SYSTEM_USER,
                    modrId = operator?.id ?: SYSTEM_USER,
                    modrNm = operator?.name ?: SYSTEM_USER
                )
            )
            if (response.isSuccess) null else "eTIMS rejected item: ${response.resultCd} ${response.resultMsg.orEmpty()}".trim()
        } catch (e: Exception) {
            e.toUserMessage()
        }
        itemDao.updateItemSyncStatus(item.itemCode, if (error == null) STATUS_SYNCED else STATUS_PENDING)
        return error
    }

    private suspend fun pushCustomer(customer: CustomerEntity, tin: String, bhfId: String, operator: User?): String? {
        val error = try {
            val response = masterDataService.saveBhfCustomer(
                BhfCustSaveReq(
                    tin = tin,
                    bhfId = bhfId,
                    custNo = customer.customerNo.ifBlank { customer.customerTin },
                    custTin = customer.customerTin,
                    custNid = customer.nationalId,
                    custNm = customer.customerName,
                    custBhfId = null,
                    adrs = customer.address,
                    telNo = customer.telNo,
                    email = customer.email,
                    faxNo = null,
                    useYn = if (customer.isUsed) "Y" else "N",
                    remark = customer.customerType,
                    regrId = operator?.id ?: SYSTEM_USER,
                    regrNm = operator?.name ?: SYSTEM_USER,
                    modrId = operator?.id ?: SYSTEM_USER,
                    modrNm = operator?.name ?: SYSTEM_USER
                )
            )
            if (response.isSuccess) null else "eTIMS rejected customer: ${response.resultCd} ${response.resultMsg.orEmpty()}".trim()
        } catch (e: Exception) {
            e.toUserMessage()
        }
        customerDao.updateCustomerSyncStatus(customer.customerTin, if (error == null) STATUS_SYNCED else STATUS_PENDING)
        return error
    }

    override suspend fun syncItemsFromEtims(tin: String, bhfId: String): Result<Int> = runCatching {
        val response = masterDataService.selectItemList(ItemListReq(tin = tin, bhfId = bhfId))
        if (!response.isSuccess) {
            throw IllegalStateException(
                "eTIMS rejected item list request: ${response.resultCd} ${response.resultMsg.orEmpty()}"
            )
        }
        val remoteItems = response.data?.itemList.orEmpty()
        // KRA's catalog carries no stock levels, and a REPLACE of the whole row would zero
        // every local stock count and clobber local edits not yet pushed — merge instead.
        val merged = remoteItems.mapNotNull { remote ->
            val local = itemDao.getItemByCode(remote.itemCd)
            when {
                local == null -> remote.toEntity()
                local.syncStatus != STATUS_SYNCED -> null
                else -> remote.toEntity().copy(
                    stockQuantity = local.stockQuantity,
                    packageQuantity = local.packageQuantity,
                    description = local.description
                )
            }
        }
        itemDao.insertItems(merged)
        remoteItems.size
    }

    override suspend fun syncCustomersFromEtims(tin: String, bhfId: String): Result<Int> = runCatching {
        val response = masterDataService.selectCustomerList(CustomerListReq(tin = tin, bhfId = bhfId))
        if (!response.isSuccess) {
            throw IllegalStateException(
                "eTIMS rejected customer list request: ${response.resultCd} ${response.resultMsg.orEmpty()}"
            )
        }
        val remoteCustomers = response.data?.custList.orEmpty()
        val merged = remoteCustomers.mapNotNull { remote ->
            val local = customerDao.getCustomerByTin(remote.custTin)
            if (local != null && local.syncStatus != STATUS_SYNCED) null
            else remote.toEntity(bhfId, local?.customerType)
        }
        customerDao.insertCustomers(merged)
        remoteCustomers.size
    }

    override suspend fun syncReferenceCodesFromEtims(tin: String, bhfId: String): Result<Int> = runCatching {
        val response = masterDataService.selectCodeList(SelectCodeListReq(tin = tin, bhfId = bhfId, lastReqDt = "20230101000000"))
        if (!response.isSuccess) {
            throw IllegalStateException("eTIMS rejected code list request: ${response.resultCd}")
        }

        val allCodes = mutableListOf<CodeEntity>()
        response.data?.clsList?.forEach { category ->
            category.dtlList.forEachIndexed { index, code ->
                allCodes.add(
                    CodeEntity(
                        classCode = category.cdCls,
                        detailCode = code.cd,
                        detailName = code.cdNm,
                        isUsed = code.useYn == "Y",
                        sortOrder = index
                    )
                )
            }
        }

        if (allCodes.isNotEmpty()) {
            codeDao.insertCodes(allCodes)
        }
        allCodes.size
    }

    override suspend fun syncItemClassificationsFromEtims(tin: String, bhfId: String): Result<Int> = runCatching {
        val response = masterDataService.selectItemClsList(ItemClsListReq(tin = tin, bhfId = bhfId, lastReqDt = "20230101000000"))
        if (!response.isSuccess) {
            throw IllegalStateException("eTIMS rejected item classification request: ${response.resultCd}")
        }

        val entities = response.data?.itemClsList?.map {
            ItemClsEntity(
                code = it.itemClsCd,
                name = it.itemClsNm,
                level = it.itemClsLvl ?: 1,
                taxTypeCode = it.taxTyCd,
                isUsed = it.useYn == "Y"
            )
        }.orEmpty()

        if (entities.isNotEmpty()) {
            itemClsDao.insertAll(entities)
        }
        entities.size
    }

    private fun RemoteItem.toEntity(): ItemEntity = ItemEntity(
        itemCode = itemCd,
        itemClassificationCode = itemClsCd,
        itemTypeCode = itemTyCd,
        itemName = itemNm,
        barcode = bcd,
        packagingUnitCode = pkgUnitCd ?: "NT",
        quantityUnitCode = qtyUnitCd ?: "U",
        taxTypeCode = taxTyCd ?: "B",
        defaultUnitPrice = dftPrc ?: 0.0,
        originNationCode = orgnNatCd ?: "KE",
        isUsed = useYn?.equals("Y", ignoreCase = true) ?: true,
        syncStatus = STATUS_SYNCED
    )

    private fun RemoteCustomer.toEntity(bhfId: String, localType: String?): CustomerEntity = CustomerEntity(
        customerTin = custTin,
        branchId = custBhfId ?: bhfId,
        customerNo = custNo.orEmpty(),
        customerName = custNm,
        customerType = localType ?: MasterDataValidator.customerTypeForPin(custTin),
        address = adrs,
        telNo = telNo,
        email = email,
        isUsed = useYn?.equals("Y", ignoreCase = true) ?: true,
        syncStatus = STATUS_SYNCED
    )

    private companion object {
        const val STATUS_PENDING = "PENDING"
        const val STATUS_SYNCED = "SYNCED"
        const val SYSTEM_USER = "SYSTEM"
    }
}
