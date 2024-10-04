package com.example.mobile.repository

import com.example.mobile.dto.Order
import com.example.mobile.dto.OrderCancelRequest
import com.example.mobile.services.api.order.OrderApiService
import com.example.mobile.utils.apiRequestFlow
import javax.inject.Inject

/*
* A repository class for orders.
* Manages the API calls for orders.
* */

class OrderRepository @Inject constructor(
    private val orderApiService: OrderApiService,
) {
    fun createOrder(order: Order) = apiRequestFlow {
        orderApiService.createOrder(order)
    }
    fun getOrders() = apiRequestFlow {
        orderApiService.getOrders()
    }
    fun cancelOrder(id: String, orderCancelRequest: OrderCancelRequest) = apiRequestFlow {
        orderApiService.cancelOrder(id, orderCancelRequest)
    }
}