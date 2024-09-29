package com.example.mobile.ui.profile
//import androidx.lifecycle.LiveData
//import androidx.lifecycle.MutableLiveData
//import androidx.lifecycle.ViewModel
//
//class ProfileViewModel : ViewModel() {
//
//    private val _text = MutableLiveData<String>().apply {
//        value = "This is notifications Fragment"
//    }
//    val text: LiveData<String> = _text
//}

import androidx.lifecycle.MutableLiveData
import com.example.mobile.repository.ProfileRepository
import com.example.mobile.services.api.dto.UserInfoResponse
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.BaseViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class ProfileViewModel @Inject constructor(
    private val mainRepository: ProfileRepository
): BaseViewModel(){
    private val _userInfoResponse = MutableLiveData<ApiResponse<UserInfoResponse>>()
    val userInfoResponse = _userInfoResponse

    fun getUserInfo(coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _userInfoResponse,
        coroutinesErrorHandler,
    ) {
        mainRepository.getUserInfo()
    }
}