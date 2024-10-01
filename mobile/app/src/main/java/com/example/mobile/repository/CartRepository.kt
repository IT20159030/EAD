package com.example.mobile.repository

import android.content.Context
import com.example.mobile.data.model.CartItem
import com.example.mobile.utils.CartDatabaseHelper

class CartRepository(context: Context) {
    private val dbHelper = CartDatabaseHelper(context)

    fun addProductToCart(productId: String, productName: String, quantity: Int, totalPrice: Double, imageUrl: String): Long {
        return dbHelper.addToCart(productId, productName, quantity, totalPrice, imageUrl)
    }

    fun getCartItems(): List<CartItem> {
        return dbHelper.getAllCartItems()
    }

    fun removeCartItem(cartItem: CartItem) {
        dbHelper.deleteCartItem(cartItem.id)
    }

    fun clearCart() {
        dbHelper.clearCart()
    }
}