package com.example.mobile.dto

/*
* Data classes for orders.
* */

data class Order (
    var orderId: String,
    var status: Int,
    var orderDate: String,
    var deliveryAddress: String,
    var orderItems: List<OrderItem>,
    var totalPrice: Double
)

data class OrderResponse (
    var id: String,
    var orderId: String,
    var status: Int,
    var orderDate: String,
    var deliveryAddress: String,
    var orderItems: List<OrderItem>,
    var totalPrice: Double,
    val customerId: String,
    val customerName: String
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