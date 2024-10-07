package com.example.mobile.ui.profileEdit

import androidx.lifecycle.MutableLiveData
import com.example.mobile.dto.UserDeactivationResponse
import com.example.mobile.dto.UserUpdateRequest
import com.example.mobile.dto.UserUpdateResponse
import com.example.mobile.repository.ProfileRepository
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.BaseViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

/*
* A view model for the edit profile fragment.
 */
@HiltViewModel
class EditProfileViewModel @Inject constructor(
    private val profileRepository: ProfileRepository
): BaseViewModel(){
    private val _deactivationResponse = MutableLiveData<ApiResponse<UserDeactivationResponse>>()
    val deactivationResponse = _deactivationResponse

    private val _updateProfileResponse = MutableLiveData<ApiResponse<UserUpdateResponse>>()
    val updateProfileResponse = _updateProfileResponse

    fun deactivateAccount(coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _deactivationResponse,
        coroutinesErrorHandler,
    ) {
        profileRepository.deactivateAccount()
    }

    fun updateAccount(updateRequest: UserUpdateRequest,coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _updateProfileResponse,
        coroutinesErrorHandler,
    ) {
        profileRepository.updateAccount(updateRequest)
    }
}