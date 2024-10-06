package com.example.mobile.ui.profile

import androidx.lifecycle.MutableLiveData
import com.example.mobile.dto.Address
import com.example.mobile.dto.AddressResponse
import com.example.mobile.repository.ProfileRepository
import com.example.mobile.dto.UserInfoResponse
import com.example.mobile.repository.AddressRepository
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.BaseViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class AddressViewModel @Inject constructor(
    private val addressRepository: AddressRepository
): BaseViewModel(){
    private val _addressResponse = MutableLiveData<ApiResponse<AddressResponse>>()
    val addressResponse = _addressResponse


    fun getAddresses(coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _addressResponse,
        coroutinesErrorHandler,
    ) {
        addressRepository.getAddresses()
    }

    fun updateAddress(address: Address, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _addressResponse,
        coroutinesErrorHandler,
    ) {
        addressRepository.updateAddress(address)
    }


}