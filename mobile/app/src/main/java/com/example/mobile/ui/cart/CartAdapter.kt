package com.example.mobile.ui.cart

import android.content.Context
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
import java.util.Locale

/*
* An adapter class for cart items.
* Displays cart items in a RecyclerView.
* */

class CartAdapter(
    private var cartItems: MutableList<CartItem>,
    private var context: Context,
    private val onRemoveClick: (CartItem) -> Unit
) : RecyclerView.Adapter<CartAdapter.CartViewHolder>() {

    class CartViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        private val productImageView: ImageView = view.findViewById(R.id.cart_item_product_image)
        private val productNameTextView: TextView = view.findViewById(R.id.cart_item_product_name)
        private val productQuantityTextView: TextView = view.findViewById(R.id.cart_item_product_quantity)
        private val productPriceTextView: TextView = view.findViewById(R.id.cart_item_product_price)
        private val removeButton: Button = view.findViewById(R.id.cart_item_remove_button)

        fun bind(cartItem: CartItem, context: Context, onRemoveClick: (CartItem) -> Unit) {
            // Set product details
            Picasso.get().load(cartItem.imageUrl)
                .into(productImageView)
            productNameTextView.text = cartItem.productName
            productQuantityTextView.text = String.format(Locale.getDefault(),
                "Quantity: %2d", cartItem.quantity)
            productPriceTextView.text = String.format(Locale.getDefault(),
                "Price: %s%.2f", context.getString(R.string.currency), cartItem.totalPrice)

            // Remove button click listener
            removeButton.setOnClickListener {
                onRemoveClick(cartItem)
            }
        }
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CartViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.cart_item, parent, false)
        return CartViewHolder(view)
    }

    override fun onBindViewHolder(holder: CartViewHolder, position: Int) {
        holder.bind(cartItems[position], context, onRemoveClick)
    }

    override fun getItemCount(): Int = cartItems.size

    fun updateList(newCartItems: List<CartItem>) {
        cartItems.clear()
        cartItems.addAll(newCartItems)
        notifyDataSetChanged()
    }
}