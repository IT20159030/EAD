package com.example.mobile.services.api.profile

import com.example.mobile.dto.UserDeactivationResponse
import com.example.mobile.dto.UserInfoResponse
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.PUT


interface ProfileApiService {
    @GET("customer-auth/user")
    suspend fun getUserInfo(): Response<UserInfoResponse>

    @PUT("customer-auth/deactivate")
    suspend fun deactivateAccount(): Response<UserDeactivationResponse>
}

