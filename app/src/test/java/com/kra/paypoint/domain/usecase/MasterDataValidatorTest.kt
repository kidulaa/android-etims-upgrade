package com.kra.paypoint.domain.usecase

import com.kra.paypoint.data.local.entity.CustomerEntity
import com.kra.paypoint.data.local.entity.ItemEntity
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertNull
import org.junit.Test

class MasterDataValidatorTest {

    private val individual = CustomerEntity(
        customerTin = "A012345678Z",
        customerName = "Brian",
        customerType = MasterDataValidator.CUSTOMER_INDIVIDUAL,
        telNo = "0700000000",
        email = "brian@example.com"
    )

    private val item = ItemEntity(
        itemCode = "KE2NTU0000001",
        itemClassificationCode = "1218160100",
        itemTypeCode = "2",
        itemName = "Oils",
        defaultUnitPrice = 7000.0
    )

    @Test
    fun `valid individual customer passes`() {
        assertNull(MasterDataValidator.validateCustomer(individual))
    }

    @Test
    fun `malformed PIN is rejected`() {
        assertNotNull(MasterDataValidator.validateCustomer(individual.copy(customerTin = "A12345")))
    }

    @Test
    fun `non-individual PIN must be registered as corporate`() {
        val company = individual.copy(customerTin = "P051234567Z")
        assertNotNull(MasterDataValidator.validateCustomer(company))
        assertNull(MasterDataValidator.validateCustomer(company.copy(customerType = MasterDataValidator.CUSTOMER_CORPORATE)))
    }

    @Test
    fun `bad email is rejected`() {
        assertNotNull(MasterDataValidator.validateCustomer(individual.copy(email = "not-an-email")))
    }

    @Test
    fun `customer type is inferred from PIN prefix`() {
        assertEquals(MasterDataValidator.CUSTOMER_CORPORATE, MasterDataValidator.customerTypeForPin("p051234567z"))
        assertEquals(MasterDataValidator.CUSTOMER_INDIVIDUAL, MasterDataValidator.customerTypeForPin("A012345678Z"))
    }

    @Test
    fun `valid item passes and missing classification or bad band is rejected`() {
        assertNull(MasterDataValidator.validateItem(item, beginningStock = 100.0))
        assertNotNull(MasterDataValidator.validateItem(item.copy(itemClassificationCode = "")))
        assertNotNull(MasterDataValidator.validateItem(item.copy(taxTypeCode = "Z")))
        assertNotNull(MasterDataValidator.validateItem(item, beginningStock = -1.0))
    }
}
