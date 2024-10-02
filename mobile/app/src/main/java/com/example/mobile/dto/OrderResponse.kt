package com.example.mobile.dto

data class Order (
    var orderId: String,
    var status: Int,
    var orderDate: String,
    var orderItems: List<OrderItem>,
    var totalPrice: Double
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