package com.kra.paypoint.di

import com.google.gson.Gson
import com.google.gson.GsonBuilder
import com.kra.paypoint.BuildConfig
import com.kra.paypoint.data.remote.EtimsAuthInterceptor
import com.kra.paypoint.data.remote.api.AuthService
import com.kra.paypoint.data.remote.api.MasterDataService
import com.kra.paypoint.data.remote.api.TransactionService
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object NetworkModule {

    @Provides
    @Singleton
    fun provideGson(): Gson {
        return GsonBuilder()
            .setLenient()
            .create()
    }

    @Provides
    @Singleton
    fun provideOkHttpClient(etimsAuthInterceptor: EtimsAuthInterceptor): OkHttpClient {
        val logging = HttpLoggingInterceptor().apply {
            // Never log request/response bodies in release: they carry TINs, tax data and
            // eTIMS signing material. Headers only, and only in debug builds. Even at HEADERS
            // level, the cmcKey/signing-key headers themselves must never reach a debug log
            // in a way that could leak into a bug report — see EtimsAuthInterceptor.
            level = if (BuildConfig.DEBUG) {
                HttpLoggingInterceptor.Level.HEADERS
            } else {
                HttpLoggingInterceptor.Level.NONE
            }
            redactHeader("cmcKey")
        }
        return OkHttpClient.Builder()
            .addInterceptor(etimsAuthInterceptor)
            .addInterceptor(logging)
            .connectTimeout(15, TimeUnit.SECONDS)
            .readTimeout(30, TimeUnit.SECONDS)
            .writeTimeout(30, TimeUnit.SECONDS)
            .retryOnConnectionFailure(true)
            .build()
    }

    @Provides
    @Singleton
    fun provideRetrofit(gson: Gson, okHttpClient: OkHttpClient): Retrofit {
        return Retrofit.Builder()
            .baseUrl(BuildConfig.BASE_URL)
            .client(okHttpClient)
            .addConverterFactory(GsonConverterFactory.create(gson))
            .build()
    }

    @Provides
    @Singleton
    fun provideAuthService(retrofit: Retrofit): AuthService {
        return retrofit.create(AuthService::class.java)
    }

    @Provides
    @Singleton
    fun provideMasterDataService(retrofit: Retrofit): MasterDataService {
        return retrofit.create(MasterDataService::class.java)
    }

    @Provides
    @Singleton
    fun provideTransactionService(retrofit: Retrofit): TransactionService {
        return retrofit.create(TransactionService::class.java)
    }
}
