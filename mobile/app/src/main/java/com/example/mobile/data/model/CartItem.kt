package com.example.mobile.data.model

/*
* Model class for cart item.
* */

data class CartItem(
    val id: Int,
    val productId: String,
    val productName: String,
    val quantity: Int,
    val totalPrice: Double,
    val imageUrl: String
)