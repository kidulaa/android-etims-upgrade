package com.kra.paypoint.data.repository

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
import com.kra.paypoint.domain.repository.MasterDataRepository
import kotlinx.coroutines.flow.Flow
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class MasterDataRepositoryImpl @Inject constructor(
    private val itemDao: ItemDao,
    private val customerDao: CustomerDao,
    private val codeDao: CodeDao,
    private val itemClsDao: ItemClsDao,
    private val masterDataService: MasterDataService
) : MasterDataRepository {

    override fun getAllItems(): Flow<List<ItemEntity>> {
        return itemDao.getAllItems()
    }

    override fun searchItems(query: String): Flow<List<ItemEntity>> {
        return itemDao.searchItems(query)
    }

    override suspend fun getItemByCode(itemCode: String): ItemEntity? {
        return itemDao.getItemByCode(itemCode)
    }

    override suspend fun saveItem(item: ItemEntity) {
        itemDao.insertItem(item)
    }

    override fun getAllCustomers(): Flow<List<CustomerEntity>> {
        return customerDao.getAllCustomers()
    }

    override fun searchCustomers(query: String): Flow<List<CustomerEntity>> {
        return customerDao.searchCustomers(query)
    }

    override suspend fun getCustomerByTin(tin: String): CustomerEntity? {
        return customerDao.getCustomerByTin(tin)
    }

    override suspend fun saveCustomer(customer: CustomerEntity) {
        customerDao.insertCustomer(customer)
    }

    override suspend fun syncItemsFromEtims(tin: String, bhfId: String): Result<Int> = runCatching {
        val response = masterDataService.selectItemList(ItemListReq(tin = tin, bhfId = bhfId))
        if (!response.isSuccess) {
            throw IllegalStateException(
                "eTIMS rejected item list request: ${response.resultCd} ${response.resultMsg.orEmpty()}"
            )
        }
        val remoteItems = response.data?.itemList.orEmpty()
        itemDao.insertItems(remoteItems.map { it.toEntity() })
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
        customerDao.insertCustomers(remoteCustomers.map { it.toEntity(bhfId) })
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
        taxTypeCode = taxTyCd ?: "A",
        defaultUnitPrice = dftPrc ?: 0.0,
        isUsed = useYn?.equals("Y", ignoreCase = true) ?: true
    )

    private fun RemoteCustomer.toEntity(bhfId: String): CustomerEntity = CustomerEntity(
        customerTin = custTin,
        branchId = custBhfId ?: bhfId,
        customerNo = custNo.orEmpty(),
        customerName = custNm,
        address = adrs,
        telNo = telNo,
        email = email,
        isUsed = useYn?.equals("Y", ignoreCase = true) ?: true,
        syncStatus = "SYNCED"
    )
}
