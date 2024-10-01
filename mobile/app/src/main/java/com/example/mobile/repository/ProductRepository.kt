package com.example.mobile.repository

import com.example.mobile.services.api.product.ProductApiService
import com.example.mobile.utils.apiRequestFlow
import javax.inject.Inject

class ProductRepository @Inject constructor (
    private val mainApiService: ProductApiService,
) {
    fun getProducts() = apiRequestFlow {
        mainApiService.getProducts()
    }
}