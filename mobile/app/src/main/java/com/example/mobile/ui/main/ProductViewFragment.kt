package com.example.mobile.ui.main

//import androidx.fragment.app.viewModels
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView

import com.example.mobile.R
import com.squareup.picasso.Picasso

class ProductViewFragment : Fragment() {

    companion object {
        fun newInstance() = ProductViewFragment()
    }

//    private val viewModel: MainViewModel by viewModels()

//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.fragment_product_view, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        // product detail views
        val productViewNameView = view.findViewById<TextView>(R.id.product_view_name_text)
        val productImageView = view.findViewById<ImageView>(R.id.product_view_image)
        val productViewDescriptionText = view.findViewById<TextView>(R.id.product_view_description_text)
        val productCategoryView = view.findViewById<TextView>(R.id.product_view_category)
        val productPriceView = view.findViewById<TextView>(R.id.product_view_price)

        // cart button views
        val productAddToCartButton = view.findViewById<Button>(R.id.product_view_add_to_cart_button)
        val productCartMinusButton = view.findViewById<TextView>(R.id.product_view_cart_minus)
        val productCartPlusButton = view.findViewById<TextView>(R.id.product_view_cart_plus)
        val productCartCountView = view.findViewById<TextView>(R.id.product_view_cart_counter)

        productCartCountView.text = 0.toString()

        // button listeners
        productCartMinusButton.setOnClickListener {
            if ( (productCartCountView.text as String).toInt() > 0) {
                val count = (productCartCountView.text as String).toInt()
                 productCartCountView.text = (count - 1).toString()
            }
        }

        productCartPlusButton.setOnClickListener {
            val count = (productCartCountView.text as String).toInt()
            productCartCountView.text = (count + 1).toString()
        }

        // set views
        productViewNameView?.text = "Programmatically Set Product Name"
        productViewDescriptionText?.text = "Programmatically Set Product Description"
        productCategoryView?.text = "Set Category"
        productPriceView?.text = "$3000"

        try {
            Picasso.get()
                .load("https://images2.alphacoders.com/655/655076.jpg")
                .into(productImageView)
        } catch (e: Exception) {
            productViewDescriptionText?.text = e.message
        }
    }
}