package com.kra.paypoint.domain.repository

import kotlinx.coroutines.flow.StateFlow

interface PreferencesRepository {
    val hasSeenOnboarding: StateFlow<Boolean?>
    suspend fun setHasSeenOnboarding(hasSeen: Boolean)

    val customEmail: StateFlow<String?>
    suspend fun setCustomEmail(email: String)

    val customPhone: StateFlow<String?>
    suspend fun setCustomPhone(phone: String)

    val customAddress: StateFlow<String?>
    suspend fun setCustomAddress(address: String)

    val nonVatFlag: StateFlow<Boolean>
    suspend fun setNonVatFlag(isNonVat: Boolean)
}
