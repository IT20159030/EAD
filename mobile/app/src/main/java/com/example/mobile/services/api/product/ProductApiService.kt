package com.example.mobile.services.api.product

import com.example.mobile.dto.Product
import com.example.mobile.dto.ProductCategory
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Query
import retrofit2.http.Path

/*
* A service interface for products APIs
* */

interface ProductApiService {
    @GET("Product/active")
    suspend fun getProducts(): Response<List<Product>>

    @GET("Product/search")
    suspend fun searchProducts(@Query("query") query: String): Response<List<Product>>

    @GET("Product/vendor/{vendorId}/active")
    suspend fun getProductsByVendor(@Path("vendorId") vendorId: String): Response<List<Product>>

    @GET("ProductCategory")
    suspend fun getProductCategories(): Response<List<ProductCategory>>

    @GET("Product/category/{categoryId}/active")
    suspend fun getProductsByCategory(@Path("categoryId") categoryId: String): Response<List<Product>>

}