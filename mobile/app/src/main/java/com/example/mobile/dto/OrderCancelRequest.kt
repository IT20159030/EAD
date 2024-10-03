package com.example.mobile.dto

/*
* A data class representing a cancel order request.
* */

data class OrderCancelRequest (
    val orderId: String,
    val customerId: String,
    val reason: String
)