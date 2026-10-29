package com.kra.paypoint.ui.theme

import android.os.Build
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Shapes
import androidx.compose.material3.Typography
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.dynamicDarkColorScheme
import androidx.compose.material3.dynamicLightColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.runtime.CompositionLocalProvider
import androidx.compose.runtime.Immutable
import androidx.compose.runtime.ReadOnlyComposable
import androidx.compose.runtime.staticCompositionLocalOf
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp

private val LightColorScheme = lightColorScheme(
    primary = Primary, onPrimary = OnPrimary,
    primaryContainer = PrimaryContainer, onPrimaryContainer = OnPrimaryContainer,
    secondary = Secondary, onSecondary = OnSecondary,
    secondaryContainer = SecondaryContainer, onSecondaryContainer = OnSecondaryContainer,
    tertiary = Tertiary, onTertiary = OnTertiary,
    tertiaryContainer = TertiaryContainer, onTertiaryContainer = OnTertiaryContainer,
    error = Error, errorContainer = ErrorContainer, onErrorContainer = OnErrorContainer,
    background = Surface, onBackground = OnSurface,
    surface = Surface, onSurface = OnSurface,
    surfaceVariant = SurfaceVariant, onSurfaceVariant = OnSurfaceVariant,
    outline = Outline, outlineVariant = OutlineVariant,
    surfaceContainerLowest = SurfaceContainerLowest, surfaceContainerLow = SurfaceContainerLow,
    surfaceContainer = SurfaceContainer, surfaceContainerHigh = SurfaceContainerHigh,
    surfaceContainerHighest = SurfaceContainerHighest,
    inverseSurface = InverseSurface, inverseOnSurface = InverseOnSurface, inversePrimary = InversePrimary
)

private val DarkColorScheme = darkColorScheme(
    primary = PrimaryDark, onPrimary = OnPrimaryDark,
    primaryContainer = PrimaryContainerDark, onPrimaryContainer = OnPrimaryContainerDark,
    secondary = SecondaryDark, onSecondary = OnSecondaryDark,
    secondaryContainer = SecondaryContainerDark, onSecondaryContainer = OnSecondaryContainerDark,
    tertiary = TertiaryDark, onTertiary = OnTertiaryDark,
    tertiaryContainer = TertiaryContainerDark, onTertiaryContainer = OnTertiaryContainerDark,
    error = ErrorDark, onError = OnErrorDark, errorContainer = ErrorContainerDark, onErrorContainer = OnErrorContainerDark,
    background = SurfaceDark, onBackground = OnSurfaceDark,
    surface = SurfaceDark, onSurface = OnSurfaceDark,
    surfaceVariant = SurfaceVariantDark, onSurfaceVariant = OnSurfaceVariantDark,
    outline = OutlineDark, outlineVariant = OutlineVariantDark,
    surfaceContainerLowest = SurfaceContainerLowestDark, surfaceContainerLow = SurfaceContainerLowDark,
    surfaceContainer = SurfaceContainerDark, surfaceContainerHigh = SurfaceContainerHighDark,
    surfaceContainerHighest = SurfaceContainerHighestDark,
    inverseSurface = InverseSurfaceDark, inverseOnSurface = InverseOnSurfaceDark, inversePrimary = InversePrimaryDark
)

/** Success / warning / info colours Material 3 doesn't define, tuned for each mode. */
@Immutable
data class StatusColors(
    val success: Color, val onSuccess: Color, val successContainer: Color, val onSuccessContainer: Color,
    val warning: Color, val warningContainer: Color, val onWarningContainer: Color,
    val info: Color, val infoContainer: Color, val onInfoContainer: Color
)

private val LightStatus = StatusColors(
    success = Color(0xFF1B6D35), onSuccess = Color.White, successContainer = Color(0xFFC8F2CF), onSuccessContainer = Color(0xFF00210B),
    warning = Color(0xFF8A5100), warningContainer = Color(0xFFFFDDB5), onWarningContainer = Color(0xFF2B1700),
    info = Color(0xFF2E5DA8), infoContainer = Color(0xFFD6E3FF), onInfoContainer = Color(0xFF001B3E)
)

private val DarkStatus = StatusColors(
    success = Color(0xFF8AD896), onSuccess = Color(0xFF003916), successContainer = Color(0xFF00531F), onSuccessContainer = Color(0xFFA5F5B0),
    warning = Color(0xFFFFB95C), warningContainer = Color(0xFF6A3C00), onWarningContainer = Color(0xFFFFDDB5),
    info = Color(0xFFA9C7FF), infoContainer = Color(0xFF00458F), onInfoContainer = Color(0xFFD6E3FF)
)

private val LocalStatusColors = staticCompositionLocalOf { LightStatus }

/** Access as `PayPointTheme.status.success` etc. */
object PayPointTheme {
    val status: StatusColors
        @Composable @ReadOnlyComposable get() = LocalStatusColors.current
}

private val AppTypography = Typography().run {
    copy(
        headlineSmall = headlineSmall.copy(fontWeight = FontWeight.SemiBold),
        titleLarge = titleLarge.copy(fontWeight = FontWeight.SemiBold),
        titleMedium = titleMedium.copy(fontWeight = FontWeight.SemiBold),
        labelLarge = labelLarge.copy(fontWeight = FontWeight.SemiBold)
    )
}

private val AppShapes = Shapes(
    extraSmall = RoundedCornerShape(6.dp),
    small = RoundedCornerShape(8.dp),
    medium = RoundedCornerShape(12.dp),
    large = RoundedCornerShape(16.dp),
    extraLarge = RoundedCornerShape(28.dp)
)

/**
 * Brand colours by default so receipts and screens stay recognisably KRA; [dynamicColor]
 * remains available for devices where the operator prefers wallpaper-based colours.
 */
@Composable
fun PayPointTheme(
    darkTheme: Boolean = isSystemInDarkTheme(),
    dynamicColor: Boolean = false,
    content: @Composable () -> Unit
) {
    val colorScheme = when {
        dynamicColor && Build.VERSION.SDK_INT >= Build.VERSION_CODES.S -> {
            val context = LocalContext.current
            if (darkTheme) dynamicDarkColorScheme(context) else dynamicLightColorScheme(context)
        }
        darkTheme -> DarkColorScheme
        else -> LightColorScheme
    }

    CompositionLocalProvider(LocalStatusColors provides if (darkTheme) DarkStatus else LightStatus) {
        MaterialTheme(
            colorScheme = colorScheme,
            typography = AppTypography,
            shapes = AppShapes,
            content = content
        )
    }
}
