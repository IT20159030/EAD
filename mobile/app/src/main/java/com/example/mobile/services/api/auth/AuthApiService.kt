package com.example.mobile.services.api.auth

import com.example.mobile.services.api.dto.AuthRequest
import com.example.mobile.services.api.dto.LoginResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.Header
import retrofit2.http.POST

interface AuthApiService {
    @POST("customer-auth/login")
    suspend fun login(
        @Body auth: AuthRequest,
    ): Response<LoginResponse>

    @POST("customer-auth/register")
    suspend fun register(
        @Body auth: AuthRequest,
    ): Response<LoginResponse>
}