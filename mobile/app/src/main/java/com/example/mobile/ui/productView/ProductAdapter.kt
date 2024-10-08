package com.example.mobile.ui.productView

import android.annotation.SuppressLint
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.navigation.NavController
import androidx.recyclerview.widget.RecyclerView
import com.example.mobile.R
import com.example.mobile.dto.Product
import com.squareup.picasso.Picasso
import java.util.Locale

/*
* A RecyclerView adapter for displaying a list of products.
* Displays the product name, price, and image.
* */

class ProductAdapter(private var products: List<Product>, private var navController: NavController) :
    RecyclerView.Adapter<ProductAdapter.ProductViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ProductViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.product_item, parent, false)
        return ProductViewHolder(view)
    }

    override fun onBindViewHolder(holder: ProductViewHolder, position: Int) {
        val product = products[position]
        holder.bind(product, navController)
    }

    override fun getItemCount(): Int {
        return products.size
    }

    @SuppressLint("NotifyDataSetChanged")
    fun updateList(newProducts: List<Product>) {
        products = newProducts
        notifyDataSetChanged()
    }

    class ProductViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        private val productImageView: ImageView = itemView.findViewById(R.id.product_item_image)
        private val productNameTextView: TextView = itemView.findViewById(R.id.product_item_name)
        private val productPriceTextView: TextView = itemView.findViewById(R.id.product_item_price)
        private val productCardView: View = itemView.findViewById(R.id.product_item_card)
        private val outOfStockChip: View = itemView.findViewById(R.id.out_of_stock_chip)

        fun bind(product: Product, navController: NavController) {
            val priceString: String = String.format(Locale.getDefault(),
                "%s%.2f", navController.context.getString(R.string.currency), product.price)
            productNameTextView.text = product.name
            productPriceTextView.text = priceString
            Picasso.get().load(product.imageUrl).into(productImageView)

            if (product.stock == 0) {
                outOfStockChip.visibility = View.VISIBLE
            } else {
                outOfStockChip.visibility = View.GONE
            }

            productCardView.setOnClickListener {
                val bundle = Bundle().apply {
                    putString("productId", product.productId)
                    putString("productName", product.name)
                    putFloat("productPrice", product.price.toFloat())
                    putString("productImageUrl", product.imageUrl)
                    putString("productDescription", product.description)
                    putInt("productStock", product.stock)
                    putString("productCategory", product.categoryName)
                    putString("productVendor", product.vendorName)
                    putString("productVendorId", product.vendorId)
                }
                // navigate to product view
                if (navController.currentDestination?.id == R.id.navigation_home) {
                    navController.navigate(R.id.action_navigation_home_to_viewProduct, bundle)
                } else if (navController.currentDestination?.id == R.id.viewVendor) {
                    navController.navigate(R.id.action_viewVendor_to_viewProduct, bundle)
                }
            }
        }
    }

}