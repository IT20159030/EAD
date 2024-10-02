package com.example.mobile.repository

import com.example.mobile.dto.UserUpdateRequest
import com.example.mobile.services.api.profile.ProfileApiService
import com.example.mobile.utils.apiRequestFlow
import javax.inject.Inject

class ProfileRepository @Inject constructor(
    private val mainApiService: ProfileApiService,
) {
    fun getUserInfo() = apiRequestFlow {
        mainApiService.getUserInfo()
    }

    fun deactivateAccount() = apiRequestFlow {
        mainApiService.deactivateAccount()
    }

    fun updateAccount(updateRequest: UserUpdateRequest) = apiRequestFlow {
        mainApiService.updateAccount(updateRequest)
    }


}