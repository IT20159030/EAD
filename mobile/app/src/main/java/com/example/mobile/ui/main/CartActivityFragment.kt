package com.example.mobile.ui.main

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

import com.example.mobile.R
import com.squareup.picasso.Picasso

class CartActivityFragment : Fragment() {

    companion object {
        fun newInstance() = CartActivityFragment()
    }

    private val viewModel: MainViewModel by viewModels()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.fragment_cart, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        // get views
        val cartItemListView = view.findViewById<LinearLayout>(R.id.cart_item_list_view)
        val cartItemRemoveButton = view.findViewById<Button>(R.id.cart_item_remove_button)
        val cartProceedButton = view.findViewById<Button>(R.id.cart_proceed_pay_button)

        // add dummy data
        val dummyProducts = listOf(
            Triple("Product 1", 1, "$10.00"),
            Triple("Product 2", 2, "$15.00"),
            Triple("Product 3", 3, "$20.00"),
            Triple("Product 4", 4, "$25.00"),
            Triple("Product 5", 5, "$30.00")
        )
        for (product in dummyProducts) {
            // Inflate the card_item.xml layout and pass the cartItemListView as the parent
            val cardView = layoutInflater.inflate(R.layout.card_item, cartItemListView, false)

            // Get references to the views in the card_item.xml layout
            val productImageView = cardView.findViewById<ImageView>(R.id.cart_item_product_image)
            val productNameTextView = cardView.findViewById<TextView>(R.id.cart_item_product_name)
            val productQuantityTextView = cardView.findViewById<TextView>(R.id.cart_item_product_quantity)
            val productPriceTextView = cardView.findViewById<TextView>(R.id.cart_item_product_price)

            // Set the dummy data to the views
            Picasso.get()
                .load("https://images2.alphacoders.com/655/655076.jpg")
                .into(productImageView)
            productNameTextView.text = product.first
            productQuantityTextView.text = getString(R.string.cart_item_quantity, product.second)
            productPriceTextView.text = getString(R.string.cart_item_price, product.third)

            // Add the inflated card view to the LinearLayout
            cartItemListView.addView(cardView)
        }

        // button listeners
        cartItemRemoveButton.setOnClickListener {
            //TODO: remove logic here
        }

        cartProceedButton.setOnClickListener {
            //TODO: proceed logic here
        }
    }
}