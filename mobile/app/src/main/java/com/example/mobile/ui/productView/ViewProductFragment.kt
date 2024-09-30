package com.example.mobile.ui.productView

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView
import androidx.fragment.app.viewModels
import androidx.lifecycle.ViewModelProvider
import com.example.mobile.R
import com.example.mobile.databinding.FragmentViewProductBinding
import com.example.mobile.ui.main.MainViewModel
import com.example.mobile.viewModels.CartViewModel
import com.squareup.picasso.Picasso

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

    private lateinit var productAddToCartButton: Button
    private lateinit var productCartMinusButton: TextView
    private lateinit var productCartPlusButton: TextView
    private lateinit var productCartCountView: TextView

    private val cartViewModel: CartViewModel by viewModels()

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val productViewModel = ViewModelProvider(this)[ProductViewModel::class.java]
        _binding = FragmentViewProductBinding.inflate(inflater, container, false)
        val root: View = binding.root

        // product detail views
        productViewNameView = binding.productViewNameText
        productImageView = binding.productViewImage
        productViewDescriptionText = binding.productViewDescriptionText
        productCategoryView = binding.productViewCategory
        productPriceView = binding.productViewPrice

        productAddToCartButton = binding.productViewAddToCartButton
        productCartMinusButton = binding.productViewCartMinus
        productCartPlusButton = binding.productViewCartPlus
        productCartCountView = binding.productViewCartCounter

        return root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        productCartCountView.text = 1.toString()
        var productName = arguments?.getString("productName")
        var productPrice = arguments?.getString("productPrice")
        var productDescription = arguments?.getString("productDescription")
        var productCategory = arguments?.getString("productCategory")

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
            if (productName != null) {
                cartViewModel.addToCart(productName, quantity, totalPrice)
            }
        }

        // set views
        productViewNameView.text = productName ?: "Programmatically Set Product Name"
        productViewDescriptionText.text = productDescription ?: """
            lorem
            lorem
            lorem
            lorem
            lorem
            lorem
            lorem
            lorem
            lorem
            
            ipsum
            ipsum
            ipsum
            ipsum
            ipsum
            ipsum
            ipsum
            ipsum
            ipsum
            
            lorem
            lorem
            lorem
            lorem
            lorem
            lorem
            lorem
            lorem
            lorem
        """.trimIndent()
        productCategoryView.text = productCategory ?: "Programmatically Set Category"
        productPriceView.text = productPrice ?: "Programmatically Set Price"
        Picasso.get()
            .load(arguments?.getString("productImageUrl") ?: "https://images2.alphacoders.com/655/655076.jpg")
            .into(productImageView)
    }

    private fun cleanPriceText(priceText: String): Double {
        // Remove symbols like $ and commas, and convert to Double
        return priceText.replace(Regex("[^0-9.]"), "").toDouble()
    }
}