package com.example.mobile.di

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.preferencesDataStore
import com.example.mobile.services.api.auth.AuthApiService
import com.example.mobile.services.api.notification.NotificationApiService
import com.example.mobile.services.api.order.OrderApiService
import com.example.mobile.services.api.product.ProductApiService
import com.example.mobile.services.api.profile.AddressApiService
import com.example.mobile.services.api.profile.ProfileApiService
import com.example.mobile.services.api.vendor.VendorApiService
import com.example.mobile.utils.AuthInterceptor
import com.example.mobile.utils.TokenManager
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.android.qualifiers.ApplicationContext
import dagger.hilt.components.SingletonComponent
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import javax.inject.Singleton

val Context.dataStore: DataStore<Preferences> by preferencesDataStore(name = "ecommerce_data_store")

@Module
@InstallIn(SingletonComponent::class)
class SingletonModule {
    @Singleton
    @Provides
    fun provideTokenManager(@ApplicationContext context: Context): TokenManager = TokenManager(context)

    @Singleton
    @Provides
    fun provideOkHttpClient(
        authInterceptor: AuthInterceptor,
    ): OkHttpClient {
        val loggingInterceptor = HttpLoggingInterceptor()
        loggingInterceptor.level = HttpLoggingInterceptor.Level.BODY

        return OkHttpClient.Builder()
            .addInterceptor(authInterceptor)
            .addInterceptor(loggingInterceptor)
            .build()
    }

    @Singleton
    @Provides
    fun provideAuthInterceptor(tokenManager: TokenManager): AuthInterceptor =
        AuthInterceptor(tokenManager)

    @Singleton
    @Provides
    fun provideRetrofitBuilder(): Retrofit.Builder =
        Retrofit.Builder()
            .baseUrl("http://192.168.1.6:5159/api/v1/")
            .addConverterFactory(GsonConverterFactory.create())

    @Singleton
    @Provides
    fun provideAuthAPIService(retrofit: Retrofit.Builder): AuthApiService =
        retrofit
            .build()
            .create(AuthApiService::class.java)

    @Singleton
    @Provides
    fun provideMainAPIService(okHttpClient: OkHttpClient, retrofit: Retrofit.Builder): ProfileApiService =
        retrofit
            .client(okHttpClient)
            .build()
            .create(ProfileApiService::class.java)

    @Singleton
    @Provides
    fun provideAddressAPIService(okHttpClient: OkHttpClient, retrofit: Retrofit.Builder): AddressApiService =
        retrofit
            .client(okHttpClient)
            .build()
            .create(AddressApiService::class.java)

    @Singleton
    @Provides
    fun provideProductAPIService(okHttpClient: OkHttpClient, retrofit: Retrofit.Builder): ProductApiService =
        retrofit
            .client(okHttpClient)
            .build()
            .create(ProductApiService::class.java)

    @Singleton
    @Provides
    fun provideOrderAPIService(okHttpClient: OkHttpClient, retrofit: Retrofit.Builder): OrderApiService =
        retrofit
            .client(okHttpClient)
            .build()
            .create(OrderApiService::class.java)

    @Singleton
    @Provides
    fun provideVendorAPIService(okHttpClient: OkHttpClient, retrofit: Retrofit.Builder): VendorApiService =
        retrofit
            .client(okHttpClient)
            .build()
            .create(VendorApiService::class.java)

    @Singleton
    @Provides
    fun provideNotificationAPIService(okHttpClient: OkHttpClient, retrofit: Retrofit.Builder): NotificationApiService =
        retrofit
            .client(okHttpClient)
            .build()
            .create(NotificationApiService::class.java)
}