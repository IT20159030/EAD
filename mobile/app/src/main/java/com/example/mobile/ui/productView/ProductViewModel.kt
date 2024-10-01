package com.example.mobile.ui.productView

import androidx.lifecycle.MutableLiveData
import com.example.mobile.dto.Product
import com.example.mobile.repository.ProductRepository
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.BaseViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class ProductViewModel @Inject constructor (
    private val productRepository: ProductRepository
) : BaseViewModel() {
    private val _products = MutableLiveData<ApiResponse<List<Product>>>()
    val products = _products

    fun getProducts(coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _products,
        coroutinesErrorHandler,
    ) {
        productRepository.getProducts()
    }
}
