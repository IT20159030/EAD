package com.example.mobile.services.api.order

import com.example.mobile.data.model.Order
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface OrderApiService {
    @POST("order")
    suspend fun createOrder(@Body order: Order): Response<Order>

}