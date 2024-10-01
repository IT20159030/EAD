package com.example.mobile.ui.cart

import androidx.fragment.app.viewModels
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import android.widget.Toast
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.mobile.data.model.CartItem

import com.example.mobile.databinding.FragmentCartBinding
import com.example.mobile.dto.Order
import com.example.mobile.ui.order.OrderViewModel
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.AuthViewModel
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
    private lateinit var cartErrorText: TextView
    private var customerID: String = ""

    private val cartViewModel: CartViewModel by viewModels()
    private val orderViewModel: OrderViewModel by viewModels()
    private val authViewModel: AuthViewModel by viewModels()

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
        cartErrorText = binding.cartErrorText

        // Load cart data from ViewModel
        cartViewModel.getCartItems().observe(viewLifecycleOwner) { cartItems ->
            cartAdapter.updateList(cartItems)
        }

        // button listeners
        cartProceedButton.setOnClickListener {
            val order = createOrderObject()
            sendCreateOrderRequest(order)
        }

        orderResponseObserver()
        customerIdObserver()
        requestCustomerId()
    }

    private fun sendCreateOrderRequest(order: Order) {
        orderViewModel.createOrderRequest(order, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
            }
        })
    }

    private fun customerIdObserver() {
        authViewModel.userInfoResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    customerID = response.data.data.userId
                    showLoading(false)
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    Toast.makeText(context, response.toString(), Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    private fun orderResponseObserver() {
        orderViewModel.orderResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    Toast.makeText(context, "Order placed successfully", Toast.LENGTH_SHORT).show()
                    cartViewModel.clearCart()
                    showLoading(false)
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    Toast.makeText(context, response.errorMessage, Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }

    private fun showError(message: String) {
        cartErrorText.text = message
    }

    private fun showLoading(status: Boolean) {
        val loadingIndicator = binding.cartLoadingIndicator
        loadingIndicator.visibility = if (status) View.VISIBLE else View.GONE
    }

    private fun requestCustomerId() {
        authViewModel.userInfo(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
            }
        })
    }

    private fun createOrderObject(): Order {
        val orderItems = cartViewModel.getCartItemsAsOrderItems()
        val totalPrice: Double = orderItems.sumOf { it.price }
        val date: String = getDateTime()

//        if (customerID.isEmpty()) requestCustomerId()

        val order = Order(
            orderId = getUnixTimestamp().toString(),
            status = 0,
            orderDate = date,
            orderItems = orderItems,
            totalPrice = totalPrice,
            customerId = customerID.ifEmpty { "3fd78a08-d8ec-404f-bfc4-7a5dd1d9bbd5" }
        )

        return order
    }

    private fun getDateTime(): String {
        val date = java.util.Date()
        val formatter = java.text.SimpleDateFormat("yyyy-MM-dd", java.util.Locale.getDefault())
        return formatter.format(date)
    }

    private fun getUnixTimestamp(): Long {
        return System.currentTimeMillis() / 1000
    }

}