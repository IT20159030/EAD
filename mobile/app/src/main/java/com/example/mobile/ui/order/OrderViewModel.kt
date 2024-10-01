package com.example.mobile.ui.order

import com.example.mobile.dto.Order
import com.example.mobile.repository.OrderRepository
import com.example.mobile.viewModels.BaseViewModel
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class OrderViewModel@Inject constructor (
    private val orderRepository: OrderRepository
) : BaseViewModel() {
    suspend fun createOrder(order: Order) {
        orderRepository.createOrder(order)
    }
}