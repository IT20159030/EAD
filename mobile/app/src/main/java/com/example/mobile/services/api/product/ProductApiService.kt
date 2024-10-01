package com.example.mobile.services.api.product

import com.example.mobile.dto.Product
import retrofit2.Response
import retrofit2.http.GET

interface ProductApiService {
    @GET("Product/active")
    suspend fun getProducts(): Response<List<Product>>
}