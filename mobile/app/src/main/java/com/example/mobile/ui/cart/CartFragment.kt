package com.example.mobile.ui.cart

import androidx.fragment.app.viewModels
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.EditText
import android.widget.ListView
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.navigation.NavController
import androidx.navigation.Navigation
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.mobile.R
import com.example.mobile.data.model.CartItem
import com.example.mobile.databinding.FragmentCartBinding
import com.example.mobile.dto.Order
import com.example.mobile.ui.order.OrderViewModel
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.AndroidEntryPoint
import java.util.Locale

/*
* Fragment class for cart.
* Displays cart items in a RecyclerView.
* Displays the total price of the cart items.
* Allows the user to place an order.
* */

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
    private lateinit var cartSubTotal: TextView

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
        cartAdapter = CartAdapter(cartItems, requireContext()) { cartItem ->
            cartViewModel.removeCartItem(cartItem)
        }
        cartRecyclerView.adapter = cartAdapter

        val cartProceedButton = binding.cartProceedPayButton
        cartErrorText = binding.cartErrorText
        cartSubTotal = binding.cartSubTotal

        // Load cart data from ViewModel
        cartViewModel.getCartItems().observe(viewLifecycleOwner) { cartItems ->
            cartAdapter.updateList(cartItems)
            setCartSubTotal(cartItems)
        }

        // button listeners
        cartProceedButton.setOnClickListener {
            val order = createOrderObject()
            showOrderSummaryDialog(order)
        }

        orderResponseObserver()
    }

    private fun showOrderSummaryDialog(order: Order) {
        val dialogView = LayoutInflater.from(context).inflate(R.layout.order_summary_dialog, null)

        // Get references to the views in the dialog
        val listView = dialogView.findViewById<ListView>(R.id.order_items_list)
        val subtotalTextView = dialogView.findViewById<TextView>(R.id.order_subtotal)
        val deliveryAddressEditText = dialogView.findViewById<EditText>(R.id.delivery_address)

        // Set the ListView adapter to display the order items
        val adapter = ArrayAdapter(
            requireContext(),
            android.R.layout.simple_list_item_1,
            order.orderItems.map {
                "${it.productName} x ${it.quantity} - ${getString(R.string.currency)}${
                    String.format(Locale.getDefault(), "%.2f", it.price)}"
            })
        listView.adapter = adapter

        // Calculate and display the subtotal
        val subtotal = order.totalPrice
        subtotalTextView.text = String.format(
            Locale.getDefault(),
            getString(R.string.sub_total_2f), getString(R.string.currency), subtotal
        )

        // Build and show the dialog
        val dialog = AlertDialog.Builder(requireContext())
            .setView(dialogView)
            .setCancelable(false)
            .setTitle("Order Summary")
            .setPositiveButton("Confirm") { _, _ ->
                // Get the delivery address
                val deliveryAddress = deliveryAddressEditText.text.toString()

                // Add the delivery address to the order object, if needed
                if (deliveryAddress.isNotEmpty()) order.deliveryAddress = deliveryAddress

                // Send the create order request
                sendCreateOrderRequest(order)
            }
            .setNegativeButton("Cancel", null)
            .create()

        dialog.show()
    }

    private fun setCartSubTotal(cartItems: List<CartItem>) {
        val totalPrice: Double = if (cartItems.isEmpty()) { 0.0 }
        else {
            // Calculate the total price of the cart items
            cartItems.sumOf { it.totalPrice }
        }
        val currency = getString(R.string.currency)

        cartSubTotal.text = String.format(Locale.getDefault(), getString(R.string.sub_total_2f), currency, totalPrice)
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
            deliveryAddress = "",
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