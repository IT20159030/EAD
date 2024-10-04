package com.example.mobile.services.api.product

import com.example.mobile.dto.Product
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Query

/*
* A service interface for products APIs
* */

interface ProductApiService {
    @GET("Product/active")
    suspend fun getProducts(): Response<List<Product>>

    @GET("Product/search")
    suspend fun searchProducts(@Query("query") query: String): Response<List<Product>>



}