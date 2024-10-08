package com.example.mobile.ui.productView

import androidx.lifecycle.MutableLiveData
import com.example.mobile.dto.Product
import com.example.mobile.dto.ProductCategory
import com.example.mobile.repository.ProductRepository
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.BaseViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

/*
* A ViewModel class for handling product-related operations.
* Handles the retrieval of products.
* */

@HiltViewModel
class ProductViewModel @Inject constructor (
    private val productRepository: ProductRepository
) : BaseViewModel() {
    private val _products = MutableLiveData<ApiResponse<List<Product>>>()
    val products = _products

    private val _productCategories = MutableLiveData<ApiResponse<List<ProductCategory>>>()
    val productCategories = _productCategories

    fun getProducts(coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _products,
        coroutinesErrorHandler,
    ) {
        productRepository.getProducts()
    }

    fun searchProducts(query: String, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _products,
        coroutinesErrorHandler,
    ) {
        productRepository.searchProducts(query)
    }

    fun getProductsByVendor(vendorId: String, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _products,
        coroutinesErrorHandler,
    ) {
        productRepository.getProductsByVendor(vendorId)
    }

    fun getProductCategories(coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _productCategories,
        coroutinesErrorHandler,
    ) {
        productRepository.getProductCategories()
    }

    fun getProductsByCategory(categoryId: String, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _products,
        coroutinesErrorHandler,
    ) {
        productRepository.getProductsByCategory(categoryId)
    }

}
