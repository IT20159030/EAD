package com.example.mobile.repository

import com.example.mobile.dto.AddReview
import com.example.mobile.dto.UpdateReview
import com.example.mobile.services.api.vendor.VendorApiService
import com.example.mobile.utils.apiRequestFlow
import javax.inject.Inject

/*
* A repository class for vendors.
* Handles the API calls for vendors.
* */

class VendorRepository @Inject constructor (
    private val mainApiService: VendorApiService,
) {
    fun getVendorById(id: String) = apiRequestFlow {
        mainApiService.getVendorById(id)
    }

    fun addVendorRating(vendorReview: AddReview) = apiRequestFlow {
        mainApiService.addVendorRating(vendorReview)
    }

    fun updateVendorRating(vendorReview: UpdateReview) = apiRequestFlow {
        mainApiService.updateVendorRating(vendorReview)
    }

}