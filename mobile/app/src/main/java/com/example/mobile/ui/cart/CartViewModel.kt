package com.example.mobile.ui.cart

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import com.example.mobile.data.model.CartItem
import com.example.mobile.dto.OrderItem
import com.example.mobile.repository.CartRepository

class CartViewModel(application: Application) : AndroidViewModel(application) {
    private val cartRepository = CartRepository(application)
    private val _cartItems = MutableLiveData<List<CartItem>>()

    fun addToCart(productId: String, productName: String, quantity: Int, totalPrice: Double, imageUrl: String) {
        cartRepository.addProductToCart(productId, productName, quantity, totalPrice, imageUrl)
    }

    fun removeCartItem(cartItem: CartItem) {
        cartRepository.removeCartItem(cartItem)
        _cartItems.value = cartRepository.getCartItems()  // Refresh cart items
    }

    fun getCartItems(): LiveData<List<CartItem>> {
        _cartItems.value = cartRepository.getCartItems()
        return _cartItems
    }

    fun clearCart() {
        cartRepository.clearCart()
        _cartItems.value = emptyList()
    }

    fun getCartItemsAsOrderItems(): List<OrderItem> {
        val cartItems = _cartItems.value ?: emptyList()

        return cartItems.mapIndexed { index, cartItem ->
            OrderItem(
                orderItemId = index.toString(),
                productId = cartItem.productId,
                productName = cartItem.productName,
                quantity = cartItem.quantity,
                price = cartItem.totalPrice,
                status = 0
            )
        }
    }

}