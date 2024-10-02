package com.example.mobile.ui.cart

import androidx.fragment.app.viewModels
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import android.widget.Toast
import androidx.navigation.NavController
import androidx.navigation.Navigation
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.mobile.data.model.CartItem

import com.example.mobile.databinding.FragmentCartBinding
import com.example.mobile.dto.Order
import com.example.mobile.ui.order.OrderViewModel
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.AndroidEntryPoint
import java.util.Locale

@AndroidEntryPoint
class CartFragment : Fragment() {

    private var _binding: FragmentCartBinding? = null
    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!


    private lateinit var navController: NavController

    private var cartItems = mutableListOf<CartItem>()
    private lateinit var cartAdapter: CartAdapter
    private lateinit var cartErrorText: TextView

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
        navController = Navigation.findNavController(view)

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
        // TODO: query to get cartItems should have customer ID
        cartViewModel.getCartItems().observe(viewLifecycleOwner) { cartItems ->
            cartAdapter.updateList(cartItems)
        }

        // button listeners
        cartProceedButton.setOnClickListener {
            val order = createOrderObject()
            sendCreateOrderRequest(order)
        }

        orderResponseObserver()
    }

    private fun sendCreateOrderRequest(order: Order) {
        if (order.orderItems.isEmpty()) {
            Toast.makeText(context, "Cart is empty", Toast.LENGTH_SHORT).show()
            return
        }
        orderViewModel.createOrderRequest(order, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
            }
        })
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

//    private fun showError(message: String) {
//        cartErrorText.text = message
//    }

    private fun showLoading(status: Boolean) {
        val loadingIndicator = binding.cartLoadingIndicator
        loadingIndicator.visibility = if (status) View.VISIBLE else View.GONE
    }

    private fun createOrderObject(): Order {
        val orderItems = cartViewModel.getCartItemsAsOrderItems()
        val totalPrice: Double = orderItems.sumOf { it.price }
        val date: String = getDateTime()

        val order = Order(
            orderId = getUnixTimestamp().toString(),
            status = 0,
            orderDate = date,
            orderItems = orderItems,
            totalPrice = String.format(Locale.getDefault(), "%.2f", totalPrice).toDouble(),
        )

        return order
    }

    private fun getDateTime(): String {
        val date = java.util.Date()
        val formatter = java.text.SimpleDateFormat("yyyy-MM-dd", Locale.getDefault())
        return formatter.format(date)
    }

    private fun getUnixTimestamp(): Long {
        return System.currentTimeMillis() / 1000
    }

}