package com.example.mobile.viewModels

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import com.example.mobile.data.model.CartItem
import com.example.mobile.data.model.Order
import com.example.mobile.repository.CartRepository

class CartViewModel(application: Application) : AndroidViewModel(application) {
    private val repository = CartRepository(application)
    private val _cartItems = MutableLiveData<List<CartItem>>()

    fun addToCart(productName: String, quantity: Int, totalPrice: Double, imageUrl: String) {
        repository.addProductToCart(productName, quantity, totalPrice, imageUrl)
    }

    fun removeCartItem(cartItem: CartItem) {
        repository.removeCartItem(cartItem)
        _cartItems.value = repository.getCartItems()  // Refresh cart items
    }

    fun getCartItems(): LiveData<List<CartItem>> {
        _cartItems.value = repository.getCartItems()
        return _cartItems
    }

//    fun createOrder(order: Order): LiveData<Order> {
//    }
}