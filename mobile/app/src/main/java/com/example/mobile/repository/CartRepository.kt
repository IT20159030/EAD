package com.example.mobile.repository

import android.content.Context
import com.example.mobile.data.model.CartItem
import com.example.mobile.utils.CartDatabaseHelper

class CartRepository(context: Context) {
    private val dbHelper = CartDatabaseHelper(context)

    fun addProductToCart(productName: String, quantity: Int, totalPrice: Double): Long {
        return dbHelper.addToCart(productName, quantity, totalPrice)
    }

    fun getCartItems(): List<CartItem> {
        return dbHelper.getAllCartItems()
    }

    fun removeCartItem(cartItem: CartItem) {
        dbHelper.deleteCartItem(cartItem.id)
    }
}