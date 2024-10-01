package com.example.mobile.ui.order

import androidx.lifecycle.MutableLiveData
import com.example.mobile.dto.Order
import com.example.mobile.repository.OrderRepository
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.BaseViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class OrderViewModel@Inject constructor (
    private val orderRepository: OrderRepository
) : BaseViewModel() {

    private val _orderResponse = MutableLiveData<ApiResponse<Order>>()
    val orderResponse = _orderResponse

    fun createOrderRequest(order: Order, coroutinesErrorHandler: CoroutinesErrorHandler) = baseRequest(
        _orderResponse,
        coroutinesErrorHandler,
    ) {
        orderRepository.createOrder(order)
    }
}