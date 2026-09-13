plugins {
    // AGP 9's built-in Kotlin support is disabled (see gradle.properties) because KSP
    // doesn't support it yet, so the classic Kotlin Android plugin is still applied.
    alias(libs.plugins.android.application) apply false
    alias(libs.plugins.kotlin.android) apply false
    alias(libs.plugins.kotlin.compose) apply false
    alias(libs.plugins.ksp) apply false
    alias(libs.plugins.hilt) apply false
}
