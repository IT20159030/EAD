package com.example.mobile.services.api.order

import com.example.mobile.dto.Order
import com.example.mobile.dto.OrderResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

interface OrderApiService {
    @POST("Order")
    suspend fun createOrder(@Body order: Order): Response<Order>

    @GET("Order")
    suspend fun getOrders(): Response<List<OrderResponse>>

    @POST("Order/cancel/{id}")
    suspend fun cancelOrder(@Path("id") id: String): Response<OrderResponse>
}