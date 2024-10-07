package com.example.mobile.services.api.profile

import com.example.mobile.dto.UserDeactivationResponse
import com.example.mobile.dto.UserInfoResponse
import com.example.mobile.dto.UserUpdateRequest
import com.example.mobile.dto.UserUpdateResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.PUT
/*
* A service interface for profile APIs
* */

interface ProfileApiService {
    @GET("customer-auth/user")
    suspend fun getUserInfo(): Response<UserInfoResponse>

    @PUT("customer-auth/deactivate")
    suspend fun deactivateAccount(): Response<UserDeactivationResponse>

    @PUT("customer-auth/update")
    suspend fun updateAccount(
        @Body updateRequest: UserUpdateRequest
    ): Response<UserUpdateResponse>
}

