package com.example.mobile.ui.cart

import androidx.fragment.app.viewModels
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.TextView
import androidx.lifecycle.ViewModelProvider
import androidx.recyclerview.widget.LinearLayoutManager

import com.example.mobile.R
import com.example.mobile.databinding.FragmentCartBinding
import com.example.mobile.viewModels.CartViewModel
import com.squareup.picasso.Picasso

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
        val textView: TextView = binding.cartTitleText
        textView.text = getString(R.string.cart)

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
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}