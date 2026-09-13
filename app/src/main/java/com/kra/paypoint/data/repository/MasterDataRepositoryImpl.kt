package com.kra.paypoint.data.repository

import com.kra.paypoint.data.local.dao.CustomerDao
import com.kra.paypoint.data.local.dao.ItemDao
import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import com.kra.paypoint.domain.repository.MasterDataRepository
import kotlinx.coroutines.flow.Flow
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class MasterDataRepositoryImpl @Inject constructor(
    private val itemDao: ItemDao,
    private val customerDao: CustomerDao
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
}
