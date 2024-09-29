package com.example.mobile

import com.example.mobile.repository.AuthRepository
import com.example.mobile.repository.ProfileRepository
import com.example.mobile.services.api.auth.AuthApiService
import com.example.mobile.services.api.profile.ProfileApiService
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.android.components.ViewModelComponent

@Module
@InstallIn(ViewModelComponent::class)
class HiltModule {

    @Provides
    fun provideAuthRepository(authApiService: AuthApiService) = AuthRepository(authApiService)

    @Provides
    fun provideMainRepository(profileApiService: ProfileApiService) = ProfileRepository(profileApiService)
}