package com.example.mobile.ui.productView

import androidx.lifecycle.MutableLiveData
import com.example.mobile.dto.AddReview
import com.example.mobile.dto.UpdateReview
import com.example.mobile.dto.Vendor
import com.example.mobile.repository.VendorRepository
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.BaseViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

/*
* A ViewModel class for handling vendor-related operations.
* Handles the retrieval of vendors.
* */

@HiltViewModel
class VendorViewModel @Inject constructor (
    private val vendorRepository: VendorRepository
) : BaseViewModel() {
    private val _vendor = MutableLiveData<ApiResponse<Vendor>>()
    val vendor = _vendor

    fun getVendorById(id: String, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _vendor,
        coroutinesErrorHandler,
    ) {
        vendorRepository.getVendorById(id)
    }

    fun addVendorRating(vendorReview: AddReview,
                        coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _vendor,
        coroutinesErrorHandler,
    ) {
        vendorRepository.addVendorRating(vendorReview)
    }

    fun updateVendorRating(vendorReview: UpdateReview,
                           coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _vendor,
        coroutinesErrorHandler,
    ) {
        vendorRepository.updateVendorRating(vendorReview)
    }

    fun deleteVendorRating(vendorId: String,
                           reviewId: String,
                           coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _vendor,
        coroutinesErrorHandler,
    ) {
        vendorRepository.deleteVendorRating(vendorId, reviewId)
    }
}