package com.example.mobile.ui.cart

import androidx.fragment.app.viewModels
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.LinearLayoutManager

import com.example.mobile.databinding.FragmentCartBinding

class CartFragment : Fragment() {

    private var _binding: FragmentCartBinding? = null
    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    private val cartViewModel: CartViewModel by viewModels()

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
        cartRecyclerView.layoutManager = LinearLayoutManager(requireContext())
        val cartProceedButton = binding.cartProceedPayButton

        // Load cart data from ViewModel
        cartViewModel.getCartItems().observe(viewLifecycleOwner) { cartItems ->
            val adapter = CartAdapter(cartItems) { cartItem ->
                // Handle remove item click
                cartViewModel.removeCartItem(cartItem)
            }
            cartRecyclerView.adapter = adapter
        }

        // button listeners
        cartProceedButton.setOnClickListener {
            //TODO: proceed logic here
            showLoading(true)
            cartViewModel.clearCart()
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }

    private fun showLoading(status: Boolean) {
        val loadingIndicator = binding.cartLoadingIndicator
        loadingIndicator.visibility = if (status) View.VISIBLE else View.GONE
    }

    private fun createOrderObject() {

    }
}