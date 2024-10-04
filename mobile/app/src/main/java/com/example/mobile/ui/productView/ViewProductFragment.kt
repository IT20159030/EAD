package com.example.mobile.ui.productView

import android.content.Context
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.EditText
import android.widget.ImageView
import android.widget.RatingBar
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.viewModels
import com.example.mobile.R
import com.example.mobile.databinding.FragmentViewProductBinding
import com.example.mobile.dto.AddReview
import com.example.mobile.dto.Vendor
import com.example.mobile.ui.cart.CartViewModel
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
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
    private lateinit var vendor: Vendor

    private lateinit var productViewNameView: TextView
    private lateinit var productImageView: ImageView
    private lateinit var productViewDescriptionText: TextView
    private lateinit var productCategoryView: TextView
    private lateinit var productPriceView: TextView
    private lateinit var productVendorView: TextView

    private lateinit var productVendorAverageRatingView: TextView
    private lateinit var productVendorReviewCountView: TextView
    private lateinit var productVendorAddReviewButton: Button
    private lateinit var productVendorUserReviewView: TextView
    private lateinit var productLoadingIndicator: View

    private lateinit var productAddToCartButton: Button
    private lateinit var productCartMinusButton: TextView
    private lateinit var productCartPlusButton: TextView
    private lateinit var productCartCountView: TextView

    private val cartViewModel: CartViewModel by viewModels()
    private val vendorViewModel: VendorViewModel by viewModels()

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

        productVendorAverageRatingView = binding.productViewAvgRating
        productVendorReviewCountView = binding.productViewReviewCount
        productVendorAddReviewButton = binding.productViewAddReviewButton
        productVendorUserReviewView = binding.productViewUserReview
        productLoadingIndicator = binding.productViewLoadingIndicator

        productAddToCartButton = binding.productViewAddToCartButton
        productCartMinusButton = binding.productViewCartMinus
        productCartPlusButton = binding.productViewCartPlus
        productCartCountView = binding.productViewCartCounter

        //default values
        productCartCountView.text = 1.toString()
        productVendorAverageRatingView.text = "0.0"
        productVendorReviewCountView.text = String.format(Locale.getDefault(),
            getString(R.string.review_count), 0)

        return root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        val productId = arguments?.getString("productId")
        val productName = arguments?.getString("productName")
        val productPrice = arguments?.getString("productPrice")
        val productDescription = arguments?.getString("productDescription")
        val productCategory = arguments?.getString("productCategory")
        val productImageUrl = arguments?.getString("productImageUrl")
        val productVendor = arguments?.getString("productVendor")
        val productVendorId = arguments?.getString("productVendorId")

        // get vendor details
        if (productVendorId != null) {
            getVendorDetails(productVendorId)
        }

        //set observer for vendor details
        vendorDetailsObserver()

        // button listeners
        setCartCountButtonListeners()
        setAddToCartButtonListener(productName, productId, productImageUrl)

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

        // set add review button listener
        productVendorAddReviewButton.setOnClickListener {
            if (this::vendor.isInitialized) {
                showRatingDialog(requireContext()) { rating, comment ->

                    if (rating == 0) {
                        Toast.makeText(context, "Please select a rating", Toast.LENGTH_SHORT).show()
                        return@showRatingDialog
                    }

                    val newReview = AddReview(vendor.vendorId, rating, comment ?: "")
                    addVendorReview(newReview)
                }
            }
        }
    }

    private fun vendorDetailsObserver() {
        vendorViewModel.vendor.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> setLoading(true)
                is ApiResponse.Success -> {
                    vendor = response.data
                    setLoading(false)

                    if (vendor.vendorRatingCount <= 0) return@observe

                    productVendorAverageRatingView.text = String.format(
                        Locale.getDefault(),
                        "%.1f", vendor.vendorRating
                    )
                    productVendorReviewCountView.text = String.format(
                        Locale.getDefault(),
                        getString(R.string.review_count), vendor.vendorRatingCount
                    )
                }

                is ApiResponse.Failure -> {
                    setLoading(false)
                    Toast.makeText(context, response.errorMessage, Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    private fun addVendorReview(review: AddReview) {
        vendorViewModel.addVendorRating(review, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
            }
        })
    }

    private fun showRatingDialog(context: Context, onConfirm: (rating: Int, review: String?) -> Unit) {
        // Inflate the custom layout for the dialog
        val dialogView = LayoutInflater.from(context).inflate(R.layout.dialog_add_rating, null)

        // Get the views from the inflated layout
        val ratingBar = dialogView.findViewById<RatingBar>(R.id.dialog_rating_bar)
        val reviewInput = dialogView.findViewById<EditText>(R.id.dialog_review_input)

        // Build the dialog
        val dialogBuilder = AlertDialog.Builder(context)
            .setView(dialogView)
            .setTitle("Add Rating")
            .setPositiveButton("Confirm") { dialog, _ ->
                val rating = ratingBar.rating.toInt()
                val review = reviewInput.text.toString().ifEmpty { null } // Allow empty comment

                // Call the confirm handler function
                onConfirm(rating, review)
                dialog.dismiss()
            }
            .setNegativeButton("Cancel") { dialog, _ ->
                dialog.dismiss()
            }

        // Show the dialog
        dialogBuilder.create().show()
    }

    private fun setLoading(isLoading: Boolean) {
        productLoadingIndicator.visibility = if (isLoading) View.VISIBLE else View.GONE
    }

    private fun getVendorDetails(vendorId: String) {
        vendorViewModel.getVendorById(vendorId, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
            }
        })
    }

    private fun setAddToCartButtonListener(
        productName: String?,
        productId: String?,
        productImageUrl: String?
    ) {
        productAddToCartButton.setOnClickListener {
            val quantity = productCartCountView.text.toString().toInt()
            val price = cleanPriceText(productPriceView.text.toString())
            val totalPrice = price * quantity

            // Add product to cart in database
            if (productName != null && productId != null) {
                val rowId = cartViewModel.addToCart(
                    productId,
                    productName,
                    quantity,
                    totalPrice,
                    productImageUrl ?: placeholderImage
                )

                // show toast with success message
                Toast.makeText(
                    context,
                    if (rowId > 0) getString(R.string.product_added_to_cart) else getString(R.string.product_add_failed),
                    Toast.LENGTH_SHORT
                ).show()
            }
        }
    }

    private fun setCartCountButtonListeners() {
        productCartMinusButton.setOnClickListener {
            if ((productCartCountView.text as String).toInt() > 1) {
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
    }

    private fun cleanPriceText(priceText: String): Double {
        // Remove symbols like $ and commas, and convert to Double
        return priceText.replace(Regex("[^0-9.]"), "").toDouble()
    }
}