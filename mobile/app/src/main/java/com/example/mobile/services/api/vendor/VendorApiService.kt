package com.example.mobile.services.api.vendor

import com.example.mobile.dto.AddReview
import com.example.mobile.dto.UpdateReview
import com.example.mobile.dto.Vendor
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path

/*
* A service interface for vendors APIs
* */

interface VendorApiService {
    @GET("Vendor/{id}")
    suspend fun getVendorById(@Path("id") id: String): Response<Vendor>

    @POST("Vendor/rating")
    suspend fun addVendorRating(@Body vendorReview: AddReview): Response<Vendor>

    @PUT("Vendor/rating")
    suspend fun updateVendorRating(@Body vendorReview: UpdateReview): Response<Vendor>

}