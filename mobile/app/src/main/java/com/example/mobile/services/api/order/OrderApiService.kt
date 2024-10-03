package com.example.mobile.services.api.order

import com.example.mobile.dto.Order
import com.example.mobile.dto.OrderCancelRequest
import com.example.mobile.dto.OrderResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

/*
* A service interface for orders APIs
* */

interface OrderApiService {
    @POST("Order")
    suspend fun createOrder(@Body order: Order): Response<Order>

    @GET("Order/GetByCustomerId")
    suspend fun getOrders(): Response<List<OrderResponse>>

    @POST("Order/CancelRequest/{id}")
    suspend fun cancelOrder(@Path("id") id: String,
                            @Body orderCancelRequest: OrderCancelRequest): Response<OrderResponse>
}