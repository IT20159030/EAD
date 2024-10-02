package com.example.mobile.viewModels

import androidx.lifecycle.MutableLiveData
import com.example.mobile.dto.LoginRequest
import com.example.mobile.dto.LoginResponse
import com.example.mobile.dto.RegisterRequest
import com.example.mobile.dto.RegisterResponse
import com.example.mobile.dto.UserResponse
import com.example.mobile.repository.AuthRepository
import com.example.mobile.utils.ApiResponse
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class AuthViewModel @Inject constructor(
    private val authRepository: AuthRepository,
): BaseViewModel() {

    private val _loginResponse = MutableLiveData<ApiResponse<LoginResponse>>()
    val loginResponse = _loginResponse

    private val _registerResponse = MutableLiveData<ApiResponse<RegisterResponse>>()
    val registerResponse = _registerResponse

    fun login(auth: LoginRequest, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _loginResponse,
        coroutinesErrorHandler
    ) {
        authRepository.login(auth)
    }

    fun register(auth: RegisterRequest, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _registerResponse,
        coroutinesErrorHandler
    ) {
        authRepository.register(auth)
    }

}