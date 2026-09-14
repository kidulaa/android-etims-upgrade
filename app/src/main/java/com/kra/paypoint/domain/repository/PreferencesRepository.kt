package com.kra.paypoint.domain.repository

import kotlinx.coroutines.flow.StateFlow

interface PreferencesRepository {
    val hasSeenOnboarding: StateFlow<Boolean?>
    suspend fun setHasSeenOnboarding(hasSeen: Boolean)
}
