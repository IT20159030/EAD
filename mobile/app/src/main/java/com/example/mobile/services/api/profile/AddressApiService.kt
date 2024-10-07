package com.example.mobile.services.api.profile

import com.example.mobile.dto.Address
import com.example.mobile.dto.AddressResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.PUT

/*
* A service interface for address APIs
* */

interface AddressApiService {
    @GET("customer-auth/address")
    suspend fun getAddresses(): Response<AddressResponse>

    @PUT("customer-auth/address")
    suspend fun updateAddress(
        @Body address: Address
    ): Response<AddressResponse>
}
