# ============================================================================
# eTIMS PayPoint Android - Production ProGuard & R8 Configuration
# ============================================================================

# --- Android Jetpack Room ---
-keepclassmembers class * extends androidx.room.RoomDatabase {
    public abstract <methods>;
}
-keep class androidx.room.** { *; }
-dontwarn androidx.room.**
-keep class com.kra.paypoint.data.local.entity.** { *; }
-keep class com.kra.paypoint.data.local.dao.** { *; }
-keep class com.kra.paypoint.data.local.database.** { *; }

# --- Retrofit & OkHttp ---
-dontwarn retrofit2.**
-keep class retrofit2.** { *; }
-keepattributes Signature
-keepattributes Exceptions
-keepattributes *Annotation*

# --- Gson / eTIMS API Serialization Models ---
# Crucial: Preserve all JSON payload models verbatim for KRA API compliance
-keepattributes EnclosingMethod
-keepclassmembers enum * { *; }
-keepclassmembers class * {
    @com.google.gson.annotations.SerializedName <fields>;
}
-keep class com.kra.paypoint.data.remote.model.** { *; }
-keep class com.kra.paypoint.domain.model.** { *; }

# --- Dagger / Hilt ---
-keep class dagger.hilt.** { *; }
-keep class com.kra.paypoint.di.** { *; }
-dontwarn dagger.hilt.**

# --- Android Jetpack WorkManager ---
-keep class androidx.work.** { *; }
-dontwarn androidx.work.**
-keep class com.kra.paypoint.worker.** { *; }

# --- Android Jetpack Compose ---
-keep class androidx.compose.** { *; }
-dontwarn androidx.compose.**

# --- Bluetooth Printer Hardware ---
-keep class com.kra.paypoint.hardware.** { *; }
