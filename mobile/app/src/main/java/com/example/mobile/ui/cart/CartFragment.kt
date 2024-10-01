package com.example.mobile.ui.cart

import androidx.fragment.app.viewModels
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.mobile.data.model.CartItem

import com.example.mobile.databinding.FragmentCartBinding
import com.example.mobile.dto.Order
import com.example.mobile.ui.order.OrderViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class CartFragment : Fragment() {

    private var _binding: FragmentCartBinding? = null
    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    private var cartItems = mutableListOf<CartItem>()
    private lateinit var cartAdapter: CartAdapter

    private val cartViewModel: CartViewModel by viewModels()
    private val orderViewModel: OrderViewModel by viewModels()

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentCartBinding.inflate(inflater, container, false)
        val root: View = binding.root

        return root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        // get views
        val cartRecyclerView  = binding.cartRecyclerView
        cartRecyclerView.layoutManager = LinearLayoutManager(context)
        cartAdapter = CartAdapter(cartItems) { cartItem ->
            cartViewModel.removeCartItem(cartItem)
        }
        cartRecyclerView.adapter = cartAdapter

        val cartProceedButton = binding.cartProceedPayButton
        val cartErrorText = binding.cartErrorText

        // Load cart data from ViewModel
        cartViewModel.getCartItems().observe(viewLifecycleOwner) { cartItems ->
            cartAdapter.updateList(cartItems)

        }

        // button listeners
        cartProceedButton.setOnClickListener {
            showLoading(true)

            val order = createOrderObject()
            sendCreateOrderRequest(order)

            cartViewModel.clearCart()
            showLoading(false)
        }
    }

    private fun sendCreateOrderRequest(order: Order) {
        orderViewModel.createOrderRequest(order, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
            }
        })
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }

    private fun showLoading(status: Boolean) {
        val loadingIndicator = binding.cartLoadingIndicator
        loadingIndicator.visibility = if (status) View.VISIBLE else View.GONE
    }

    private fun createOrderObject(): Order {
        val orderItems = cartViewModel.getCartItemsAsOrderItems()
        val totalPrice: Double = orderItems.sumOf { it.price }
        val date = java.util.Date()

        val order = Order(
            orderId = "", // auto generated
            status = 0,
            orderDate = date.toString(),
            orderItems = orderItems,
            totalPrice = totalPrice,
            customerId = "" //TODO: get customer id from user
        )

        return order
    }

}