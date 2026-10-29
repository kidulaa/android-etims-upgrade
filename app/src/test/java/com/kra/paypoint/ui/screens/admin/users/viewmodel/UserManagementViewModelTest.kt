package com.kra.paypoint.ui.screens.admin.users.viewmodel

import com.kra.paypoint.domain.model.auth.User
import com.kra.paypoint.domain.repository.AuthRepository
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.flowOf
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertNull
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.mock
import org.mockito.Mockito.never
import org.mockito.kotlin.any
import org.mockito.kotlin.anyOrNull
import org.mockito.kotlin.eq
import org.mockito.kotlin.isNull
import org.mockito.kotlin.verify
import org.mockito.kotlin.whenever

@OptIn(ExperimentalCoroutinesApi::class)
class UserManagementViewModelTest {

    private val dispatcher = StandardTestDispatcher()
    private lateinit var authRepository: AuthRepository
    private val currentUser = MutableStateFlow<User?>(null)

    private val admin = User(
        id = "00000", name = "Dunford", authorityCode = User.ROLE_ADMIN,
        branchId = "00", tin = "P012345678X", deviceSerial = "MOBILE001"
    )

    @Before
    fun setup() {
        Dispatchers.setMain(dispatcher)
        authRepository = mock(AuthRepository::class.java)
        whenever(authRepository.currentUser).thenReturn(currentUser)
        whenever(authRepository.observeUsers()).thenReturn(flowOf(emptyList()))
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun `admin creates a cashier on this device's PIN, branch and serial`() = runTest {
        currentUser.value = admin
        whenever(authRepository.registerLocalUser(any(), any(), any(), any(), any(), any(), any(), anyOrNull()))
            .thenReturn(Result.success(Unit))
        val vm = UserManagementViewModel(authRepository)

        vm.startNew()
        vm.updateForm { it.copy(userCode = "00001", userName = "Grace", password = "secret1", telNo = "0712345678") }
        vm.save()
        dispatcher.scheduler.advanceUntilIdle()

        verify(authRepository).registerLocalUser(
            eq("00001"), eq("secret1"), eq("Grace"), eq(User.ROLE_USER), eq("00"), eq("P012345678X"), eq("MOBILE001"), eq("0712345678")
        )
        assertNull(vm.uiState.value.form)
        assertNotNull(vm.uiState.value.message)
    }

    @Test
    fun `editing with a blank password keeps the existing password`() = runTest {
        currentUser.value = admin
        whenever(authRepository.updateLocalUser(any(), any(), anyOrNull(), any(), anyOrNull())).thenReturn(Result.success(Unit))
        val vm = UserManagementViewModel(authRepository)
        val cashier = User(id = "00001", name = "Grace", authorityCode = User.ROLE_USER, branchId = "00", tin = "P012345678X")

        vm.startEdit(cashier)
        vm.updateForm { it.copy(isAdmin = true) }
        vm.save()
        dispatcher.scheduler.advanceUntilIdle()

        verify(authRepository).updateLocalUser(eq("00001"), eq("Grace"), anyOrNull(), eq(User.ROLE_ADMIN), isNull())
    }

    @Test
    fun `a cashier cannot manage users`() = runTest {
        currentUser.value = admin.copy(authorityCode = User.ROLE_USER)
        val vm = UserManagementViewModel(authRepository)

        vm.startNew()
        vm.updateForm { it.copy(userCode = "00002", userName = "Mallory", password = "secret1") }
        vm.save()
        dispatcher.scheduler.advanceUntilIdle()

        verify(authRepository, never()).registerLocalUser(any(), any(), any(), any(), any(), any(), any(), anyOrNull())
        assertEquals("Only an admin can manage users.", vm.uiState.value.errorMessage)
    }
}
