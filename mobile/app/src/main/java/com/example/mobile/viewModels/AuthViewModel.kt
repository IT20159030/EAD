package com.example.mobile.viewModels

import androidx.lifecycle.MutableLiveData
import com.example.mobile.services.api.dto.AuthRequest
import com.example.mobile.services.api.dto.LoginResponse
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

    fun login(auth: AuthRequest, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _loginResponse,
        coroutinesErrorHandler
    ) {
        authRepository.login(auth)
    }
}