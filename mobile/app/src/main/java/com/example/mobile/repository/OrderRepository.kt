package com.example.mobile.repository

import android.content.Context
import com.example.mobile.dto.Order
import com.example.mobile.services.api.order.OrderApiService
import javax.inject.Inject

class OrderRepository @Inject constructor(
    private val orderApiService: OrderApiService,
) {
    suspend fun createOrder(order: Order) = orderApiService.createOrder(order)
}