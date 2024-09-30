package com.example.mobile.ui.cart

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.mobile.R
import com.example.mobile.data.model.CartItem
import com.squareup.picasso.Picasso

class CartAdapter(
    private val cartItems: List<CartItem>,
    private val onRemoveClick: (CartItem) -> Unit
) : RecyclerView.Adapter<CartAdapter.CartViewHolder>() {

    class CartViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val productImageView: ImageView = view.findViewById(R.id.cart_item_product_image)
        val productNameTextView: TextView = view.findViewById(R.id.cart_item_product_name)
        val productQuantityTextView: TextView = view.findViewById(R.id.cart_item_product_quantity)
        val productPriceTextView: TextView = view.findViewById(R.id.cart_item_product_price)
        val removeButton: Button = view.findViewById(R.id.cart_item_remove_button)

        fun bind(cartItem: CartItem, onRemoveClick: (CartItem) -> Unit) {
            // Set product details
            Picasso.get().load("https://images2.alphacoders.com/655/655076.jpg")
                .into(productImageView)
            productNameTextView.text = cartItem.productName
            productQuantityTextView.text = "Quantity: ${cartItem.quantity}"
            productPriceTextView.text = "Price: $${cartItem.totalPrice}"

            // Remove button click listener
            removeButton.setOnClickListener {
                onRemoveClick(cartItem)
            }
        }
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CartViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.card_item, parent, false)
        return CartViewHolder(view)
    }

    override fun onBindViewHolder(holder: CartViewHolder, position: Int) {
        holder.bind(cartItems[position], onRemoveClick)
    }

    override fun getItemCount(): Int = cartItems.size
}