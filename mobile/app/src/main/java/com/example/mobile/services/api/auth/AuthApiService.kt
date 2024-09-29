package com.example.mobile.services.api.auth

import com.example.mobile.dto.LoginRequest
import com.example.mobile.dto.LoginResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface AuthApiService {
    @POST("customer-auth/login")
    suspend fun login(
        @Body auth: LoginRequest,
    ): Response<LoginResponse>

    @POST("customer-auth/register")
    suspend fun register(
        @Body auth: LoginRequest,
    ): Response<LoginResponse>
}