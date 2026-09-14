package com.kra.paypoint.data.repository

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.preferencesDataStore
import com.kra.paypoint.domain.repository.PreferencesRepository
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.flow.stateIn
import javax.inject.Inject
import javax.inject.Singleton

private val Context.dataStore: DataStore<Preferences> by preferencesDataStore(name = "app_preferences")

@Singleton
class PreferencesRepositoryImpl @Inject constructor(
    @ApplicationContext private val context: Context
) : PreferencesRepository {

    private val scope = CoroutineScope(SupervisorJob() + Dispatchers.IO)

    private val HAS_SEEN_ONBOARDING = booleanPreferencesKey("has_seen_onboarding")
    private val CUSTOM_EMAIL = stringPreferencesKey("custom_email")
    private val CUSTOM_PHONE = stringPreferencesKey("custom_phone")
    private val CUSTOM_ADDRESS = stringPreferencesKey("custom_address")
    private val NON_VAT_FLAG = booleanPreferencesKey("non_vat_flag")

    override val hasSeenOnboarding: StateFlow<Boolean?> = context.dataStore.data
        .map { preferences ->
            preferences[HAS_SEEN_ONBOARDING] ?: false
        }
        .stateIn(
            scope = scope,
            started = SharingStarted.Eagerly,
            initialValue = null
        )

    override suspend fun setHasSeenOnboarding(hasSeen: Boolean) {
        context.dataStore.edit { preferences ->
            preferences[HAS_SEEN_ONBOARDING] = hasSeen
        }
    }

    override val customEmail: StateFlow<String?> = context.dataStore.data
        .map { preferences -> preferences[CUSTOM_EMAIL] }
        .stateIn(scope = scope, started = SharingStarted.Eagerly, initialValue = null)

    override suspend fun setCustomEmail(email: String) {
        context.dataStore.edit { preferences -> preferences[CUSTOM_EMAIL] = email }
    }

    override val customPhone: StateFlow<String?> = context.dataStore.data
        .map { preferences -> preferences[CUSTOM_PHONE] }
        .stateIn(scope = scope, started = SharingStarted.Eagerly, initialValue = null)

    override suspend fun setCustomPhone(phone: String) {
        context.dataStore.edit { preferences -> preferences[CUSTOM_PHONE] = phone }
    }

    override val customAddress: StateFlow<String?> = context.dataStore.data
        .map { preferences -> preferences[CUSTOM_ADDRESS] }
        .stateIn(scope = scope, started = SharingStarted.Eagerly, initialValue = null)

    override suspend fun setCustomAddress(address: String) {
        context.dataStore.edit { preferences -> preferences[CUSTOM_ADDRESS] = address }
    }

    override val nonVatFlag: StateFlow<Boolean> = context.dataStore.data
        .map { preferences -> preferences[NON_VAT_FLAG] ?: false }
        .stateIn(scope = scope, started = SharingStarted.Eagerly, initialValue = false)

    override suspend fun setNonVatFlag(isNonVat: Boolean) {
        context.dataStore.edit { preferences -> preferences[NON_VAT_FLAG] = isNonVat }
    }
}
