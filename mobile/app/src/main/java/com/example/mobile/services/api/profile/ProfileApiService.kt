package com.example.mobile.services.api.profile

import com.example.mobile.services.api.dto.UserInfoResponse
import retrofit2.Response
import retrofit2.http.GET

interface ProfileApiService {
    @GET("customer-user/user")
    suspend fun getUserInfo(): Response<UserInfoResponse>
}