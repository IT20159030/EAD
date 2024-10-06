package com.example.mobile.repository

import com.example.mobile.dto.Address
import com.example.mobile.dto.UserUpdateRequest
import com.example.mobile.services.api.profile.AddressApiService
import com.example.mobile.services.api.profile.ProfileApiService
import com.example.mobile.utils.apiRequestFlow
import javax.inject.Inject

/*
* A repository class for address.
* Handles the API calls for address.
 */


class AddressRepository @Inject constructor(
    private val mainApiService: AddressApiService,
) {

    fun getAddresses() = apiRequestFlow {
        mainApiService.getAddresses()
    }

    fun updateAddress(address: Address) = apiRequestFlow {
        mainApiService.updateAddress(address)
    }


}