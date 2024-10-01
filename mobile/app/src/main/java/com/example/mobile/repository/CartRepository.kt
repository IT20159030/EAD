package com.example.mobile.repository

import android.content.Context
import androidx.lifecycle.LiveData
import com.example.mobile.data.model.CartItem
import com.example.mobile.data.model.Order
import com.example.mobile.utils.CartDatabaseHelper

class CartRepository(context: Context) {
    private val dbHelper = CartDatabaseHelper(context)

    fun addProductToCart(productName: String, quantity: Int, totalPrice: Double, imageUrl: String): Long {
        return dbHelper.addToCart(productName, quantity, totalPrice, imageUrl)
    }

    fun getCartItems(): List<CartItem> {
        return dbHelper.getAllCartItems()
    }

    fun removeCartItem(cartItem: CartItem) {
        dbHelper.deleteCartItem(cartItem.id)
    }

//    fun createOrder(order: Order): LiveData<Order> {
//
//    }

}