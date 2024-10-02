package com.example.mobile.repository


import com.example.mobile.services.api.auth.AuthApiService
import com.example.mobile.dto.LoginRequest
import com.example.mobile.utils.apiRequestFlow
import javax.inject.Inject

class AuthRepository @Inject constructor(
    private val authApiService: AuthApiService,
) {
    fun login(auth: LoginRequest) = apiRequestFlow {
        authApiService.login(auth)
    }

    fun register(auth: LoginRequest) = apiRequestFlow {
        authApiService.register(auth)
    }

    fun userInfo() = apiRequestFlow {
        authApiService.userInfo()
    }

}