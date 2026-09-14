package com.kra.paypoint.ui.screens.onboarding.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.kra.paypoint.domain.repository.PreferencesRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class OnboardingViewModel @Inject constructor(
    private val preferencesRepository: PreferencesRepository
) : ViewModel() {

    fun completeOnboarding() {
        viewModelScope.launch {
            preferencesRepository.setHasSeenOnboarding(true)
        }
    }
}
