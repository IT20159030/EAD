package com.example.mobile.repository

import com.example.mobile.services.api.product.ProductApiService
import com.example.mobile.utils.apiRequestFlow
import javax.inject.Inject

/*
* A repository class for products.
* Manages the API calls for products.
* */

class ProductRepository @Inject constructor (
    private val mainApiService: ProductApiService,
) {
    fun getProducts() = apiRequestFlow {
        mainApiService.getProducts()
    }

    fun searchProducts(query: String) = apiRequestFlow {
        mainApiService.searchProducts(query)
    }

    fun getProductsByVendor(vendorId: String) = apiRequestFlow {
        mainApiService.getProductsByVendor(vendorId)
    }

    fun getProductCategories() = apiRequestFlow {
        mainApiService.getProductCategories()
    }

    fun getProductsByCategory(categoryId: String) = apiRequestFlow {
        mainApiService.getProductsByCategory(categoryId)
    }
}