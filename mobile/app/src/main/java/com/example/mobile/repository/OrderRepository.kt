package com.example.mobile.repository

import com.example.mobile.dto.Order
import com.example.mobile.services.api.order.OrderApiService
import com.example.mobile.utils.apiRequestFlow
import javax.inject.Inject

class OrderRepository @Inject constructor(
    private val orderApiService: OrderApiService,
) {
    fun createOrder(order: Order) = apiRequestFlow {
        orderApiService.createOrder(order)
    }
    fun getOrders() = apiRequestFlow {
        orderApiService.getOrders()
    }
    fun cancelOrder(id: String) = apiRequestFlow {
        orderApiService.cancelOrder(id)
    }
}