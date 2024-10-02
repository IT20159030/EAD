package com.example.mobile.viewModels

import androidx.lifecycle.MutableLiveData
import com.example.mobile.dto.LoginRequest
import com.example.mobile.dto.LoginResponse
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

    private val _registerResponse = MutableLiveData<ApiResponse<LoginResponse>>()
    val registerResponse = _registerResponse

    private val _userInfoResponse = MutableLiveData<ApiResponse<UserResponse>>()
    val userInfoResponse = _userInfoResponse

    fun login(auth: LoginRequest, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _loginResponse,
        coroutinesErrorHandler
    ) {
        authRepository.login(auth)
    }

    fun register(auth: LoginRequest, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _registerResponse,
        coroutinesErrorHandler
    ) {
        authRepository.register(auth)
    }

    fun userInfo(coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _userInfoResponse,
        coroutinesErrorHandler
    ) {
        authRepository.userInfo()
    }
}