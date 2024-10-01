package com.example.mobile.dto

import com.google.gson.annotations.SerializedName

data class Order (
    @SerializedName("id")
    var orderId: String,
    var status: Int,
    var orderDate: String,
    var orderItems: List<OrderItem>,
    var totalPrice: Double,
    var customerId: String
)

data class OrderItem (
    var id: String,
    var orderItemId: String,
    var productId: String,
    var productName: String,
    var quantity: Int,
    var price: Double,
    var status: Int,
)