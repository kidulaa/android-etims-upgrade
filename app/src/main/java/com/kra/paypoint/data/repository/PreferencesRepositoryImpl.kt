package com.kra.paypoint.data.repository

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.booleanPreferencesKey
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
}
