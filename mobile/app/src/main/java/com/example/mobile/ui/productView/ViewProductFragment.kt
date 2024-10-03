package com.example.mobile.ui.productView

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.fragment.app.viewModels
import com.example.mobile.R
import com.example.mobile.databinding.FragmentViewProductBinding
import com.example.mobile.ui.cart.CartViewModel
import com.squareup.picasso.Picasso
import dagger.hilt.android.AndroidEntryPoint
import java.util.Locale

/*
* A fragment that displays the details of a product.
* Shows the product name, description, category, price, and vendor.
* */

@AndroidEntryPoint
class ViewProductFragment : Fragment() {
    private var _binding: FragmentViewProductBinding? = null
    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    private lateinit var productViewNameView: TextView
    private lateinit var productImageView: ImageView
    private lateinit var productViewDescriptionText: TextView
    private lateinit var productCategoryView: TextView
    private lateinit var productPriceView: TextView
    private lateinit var productVendorView: TextView

    private lateinit var productAddToCartButton: Button
    private lateinit var productCartMinusButton: TextView
    private lateinit var productCartPlusButton: TextView
    private lateinit var productCartCountView: TextView

    private val cartViewModel: CartViewModel by viewModels()

    private val placeholderImage = "https://images2.alphacoders.com/655/655076.jpg"

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentViewProductBinding.inflate(inflater, container, false)
        val root: View = binding.root

        // product detail views
        productViewNameView = binding.productViewNameText
        productImageView = binding.productViewImage
        productViewDescriptionText = binding.productViewDescriptionText
        productCategoryView = binding.productViewCategory
        productPriceView = binding.productViewPrice
        productVendorView = binding.productViewVendorText

        productAddToCartButton = binding.productViewAddToCartButton
        productCartMinusButton = binding.productViewCartMinus
        productCartPlusButton = binding.productViewCartPlus
        productCartCountView = binding.productViewCartCounter

        return root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        productCartCountView.text = 1.toString()
        val productId = arguments?.getString("productId")
        val productName = arguments?.getString("productName")
        val productPrice = arguments?.getString("productPrice")
        val productDescription = arguments?.getString("productDescription")
        val productCategory = arguments?.getString("productCategory")
        val productImageUrl = arguments?.getString("productImageUrl")
        val productVendor = arguments?.getString("productVendor")

        // button listeners
        productCartMinusButton.setOnClickListener {
            if ( (productCartCountView.text as String).toInt() > 1) {
                var count = (productCartCountView.text as String).toInt()
                count--
                productCartCountView.text = count.toString()
            }
        }

        productCartPlusButton.setOnClickListener {
            var count = (productCartCountView.text as String).toInt()
            count++
            productCartCountView.text = count.toString()
        }

        productAddToCartButton.setOnClickListener {
            val quantity = productCartCountView.text.toString().toInt()
            val price = cleanPriceText(productPriceView.text.toString())
            val totalPrice = price * quantity

            // Add product to cart in database
            if (productName != null && productId != null) {
                val rowId = cartViewModel.addToCart(productId, productName, quantity, totalPrice, productImageUrl ?: placeholderImage)

                // show toast with success message
                Toast.makeText(
                    context,
                    if (rowId > 0) getString(R.string.product_added_to_cart) else getString(R.string.product_add_failed),
                    Toast.LENGTH_SHORT
                ).show()
            }
        }

        val vendorName: String = if (productVendor == null || productVendor == "" )
        { "Unknown Vendor" } else { productVendor }

        // set views
        productViewNameView.text = productName ?: "Unknown Product"
        productViewDescriptionText.text = productDescription ?: "No Description"
        productCategoryView.text = productCategory ?: "Unknown"
        productPriceView.text = productPrice ?: "Price Not Set"
        productVendorView.text = String.format(Locale.getDefault(),
            getString(R.string.by_s), vendorName)
        Picasso.get()
            .load(productImageUrl ?: placeholderImage)
            .into(productImageView)
    }

    private fun cleanPriceText(priceText: String): Double {
        // Remove symbols like $ and commas, and convert to Double
        return priceText.replace(Regex("[^0-9.]"), "").toDouble()
    }
}