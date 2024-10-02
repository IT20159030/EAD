package com.example.mobile.ui.productView

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

    fun updateList(newProducts: List<Product>) {
        products = newProducts
        notifyDataSetChanged()
    }

    class ProductViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        private val productImageView: ImageView = itemView.findViewById(R.id.product_item_image)
        private val productNameTextView: TextView = itemView.findViewById(R.id.product_item_name)
        private val productPriceTextView: TextView = itemView.findViewById(R.id.product_item_price)
        private val productCardView: View = itemView.findViewById(R.id.product_item_card)

        fun bind(product: Product, navController: NavController) {
            val priceString: String = String.format(Locale.getDefault(),
                "%s%.2f", navController.context.getString(R.string.currency), product.price)
            productNameTextView.text = product.name
            productPriceTextView.text = priceString
            Picasso.get().load(product.imageUrl).into(productImageView)
            productCardView.setOnClickListener {
                val bundle = Bundle().apply {
                    putString("productId", product.productId)
                    putString("productName", product.name)
                    putString("productPrice", priceString)
                    putString("productImageUrl", product.imageUrl)
                    putString("productDescription", product.description)
                    putString("productCategory", product.category)
                }
                // navigate to product view
                navController.navigate(R.id.action_navigation_home_to_viewProduct, bundle)
            }
        }
    }
}