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
import com.example.mobile.dto.Review
import com.example.mobile.dto.UpdateReview
import com.example.mobile.dto.UserInfo
import com.example.mobile.dto.Vendor
import com.example.mobile.ui.cart.CartViewModel
import com.example.mobile.ui.profile.ProfileViewModel
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
    private var productStock: Int = 0

    private lateinit var productViewNameView: TextView
    private lateinit var productImageView: ImageView
    private lateinit var productViewDescriptionText: TextView
    private lateinit var productCategoryView: TextView
    private lateinit var productPriceView: TextView
    private lateinit var productVendorView: TextView

    private lateinit var productVendorAverageRatingView: TextView
    private lateinit var productVendorReviewCountView: TextView
    private lateinit var productUserReviewLayout: View
    private lateinit var productVendorAddReviewButton: Button
    private lateinit var productUserRatingView: TextView
    private lateinit var productVendorUserReviewView: TextView
    private lateinit var productUserEditReviewButton: Button
    private lateinit var productLoadingIndicator: View

    private lateinit var productAddToCartButton: Button
    private lateinit var productCartMinusButton: TextView
    private lateinit var productCartPlusButton: TextView
    private lateinit var productCartCountView: TextView
    private lateinit var productStockView: TextView

    private lateinit var currentUserInfo: UserInfo

    private val cartViewModel: CartViewModel by viewModels()
    private val vendorViewModel: VendorViewModel by viewModels()
    private val profileViewModel: ProfileViewModel by viewModels()

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
        productUserReviewLayout = binding.productViewUserReviewLayout
        productVendorAddReviewButton = binding.productViewAddReviewButton
        productVendorUserReviewView = binding.productViewUserReview
        productLoadingIndicator = binding.productViewLoadingIndicator
        productUserRatingView = binding.productViewUserRating
        productUserEditReviewButton = binding.productViewEditReviewButton

        productAddToCartButton = binding.productViewAddToCartButton
        productCartMinusButton = binding.productViewCartMinus
        productCartPlusButton = binding.productViewCartPlus
        productCartCountView = binding.productViewCartCounter
        productStockView = binding.productViewStockText

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
        val productPrice = arguments?.getFloat("productPrice") ?: 0.0f
        val productDescription = arguments?.getString("productDescription")
        productStock = arguments?.getInt("productStock") ?: 0
        val productCategory = arguments?.getString("productCategory")
        val productImageUrl = arguments?.getString("productImageUrl")
        val productVendor = arguments?.getString("productVendor")
        val productVendorId = arguments?.getString("productVendorId")

        // get vendor details
        if (productVendorId != null) {
            getVendorDetails(productVendorId)
        }

        //set observers
        vendorDetailsObserver()
        currentUserInfoObserver()

        // button listeners
        setCartCountButtonListeners()
        setAddToCartButtonListener(productName, productId, productImageUrl, productPrice)
        addReviewButtonListener()
        userReviewEditButtonListener()

        val vendorName: String = if (productVendor == null || productVendor == "" )
        { "Unknown Vendor" } else { productVendor }

        // set views
        productViewNameView.text = productName ?: "Unknown Product"
        productViewDescriptionText.text = productDescription ?: "No Description"
        productCategoryView.text = productCategory ?: "Unknown"
        productPriceView.text = String.format(Locale.getDefault(), "%s%.2f",
            getString(R.string.currency), productPrice)
        productVendorView.text = String.format(Locale.getDefault(),
            getString(R.string.by_s), vendorName)
        Picasso.get()
            .load(productImageUrl ?: placeholderImage)
            .into(productImageView)
        productStockView.text = String.format(Locale.getDefault(),
            getString(R.string.choose_quantity_d_left), productStock)

        getCurrentUserInformation()
    }

    private fun userReviewEditButtonListener() {
        productUserEditReviewButton.setOnClickListener {
            showEditReviewDialog(
                requireContext(),
                vendor.reviews.first { it.reviewerId == currentUserInfo.id },
                { newReview ->
                    updateVendorReview(newReview)
                },
                { reviewId ->
                    deleteVendorReview(reviewId)
                })
        }
    }

    private fun setCurrentUserReviewDisplay() {
        if (this::vendor.isInitialized && vendor.reviews.isNotEmpty()
            && this::currentUserInfo.isInitialized) {
            if (vendor.reviews.any { it.reviewerId == currentUserInfo.id }) {
                val comment = vendor.reviews.first { it.reviewerId == currentUserInfo.id }.comment

                if (comment != "") {
                    productVendorUserReviewView.text = comment
                } else {
                    productVendorUserReviewView.text = getString(R.string.no_comment_given)
                }
                productUserRatingView.text =
                    String.format(Locale.getDefault(), getString(R.string.you_d),
                        vendor.reviews.first { it.reviewerId == currentUserInfo.id }.rating)
                productUserReviewLayout.visibility = View.VISIBLE
                productVendorAddReviewButton.visibility = View.GONE
            } else {
                productUserReviewLayout.visibility = View.GONE
            }
        }
    }

    private fun getCurrentUserInformation() {
        profileViewModel.getUserInfo(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
            }
        })
    }

    private fun currentUserInfoObserver() {
        profileViewModel.userInfoResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Success -> {
                    currentUserInfo = response.data.data
                    setCurrentUserReviewDisplay()
                }
                else -> {}
            }
        }
    }

    private fun addReviewButtonListener() {
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

                    if (vendor.vendorRatingCount <= 0) {
                        productVendorAverageRatingView.text = "0.0"
                        productVendorReviewCountView.text = String.format(
                            Locale.getDefault(),
                            getString(R.string.review_count), 0
                        )
                        productUserReviewLayout.visibility = View.GONE
                    } else {
                        productVendorAverageRatingView.text = String.format(
                            Locale.getDefault(),
                            "%.1f", vendor.vendorRating
                        )
                        productVendorReviewCountView.text = String.format(
                            Locale.getDefault(),
                            getString(R.string.review_count), vendor.vendorRatingCount
                        )
                        setCurrentUserReviewDisplay()
                    }
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

    private fun updateVendorReview(review: UpdateReview) {
        vendorViewModel.updateVendorRating(review, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
            }
        })
    }

    private fun deleteVendorReview(reviewId: String) {
        vendorViewModel.deleteVendorRating(vendor.vendorId, reviewId, object : CoroutinesErrorHandler {
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

    private fun showEditReviewDialog(
        context: Context,
        review: Review,
        onConfirm: (newReview: UpdateReview) -> Unit,
        onDelete: (reviewId: String) -> Unit
    ) {
        // Inflate the custom layout for the dialog
        val dialogView = LayoutInflater.from(context).inflate(R.layout.dialog_add_rating, null)

        // Get the views from the inflated layout
        val ratingBar = dialogView.findViewById<RatingBar>(R.id.dialog_rating_bar)
        val reviewInput = dialogView.findViewById<EditText>(R.id.dialog_review_input)

        // Set current values for editing
        ratingBar.rating = review.rating.toFloat()
        reviewInput.setText(review.comment)

        // Build the dialog
        val dialogBuilder = AlertDialog.Builder(context)
            .setView(dialogView)
            .setTitle("Edit Review")
            .setPositiveButton("Confirm") { dialog, _ ->
                val newRating = ratingBar.rating.toInt()
                val newReview = reviewInput.text.toString().ifEmpty { null } // Allow empty comment

                //create update review object
                val updateReview = UpdateReview(
                    review.reviewId,
                    vendor.vendorId,
                    review.reviewerId,
                    review.reviewerName,
                    newRating,
                    newReview ?: ""
                )
                // Call the confirm handler function
                onConfirm(updateReview)
                dialog.dismiss()
            }
            .setNegativeButton("Cancel") { dialog, _ ->
                dialog.dismiss()
            }
            .setNeutralButton("Delete") { dialog, _ ->
                // Call the delete handler function
                onDelete(review.reviewId)
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
        productImageUrl: String?,
        productPrice: Float
    ) {
        productAddToCartButton.setOnClickListener {
            val quantity = productCartCountView.text.toString().toInt()
            val totalPrice = productPrice * quantity

            if (productId != null && productName != null) {
                // Add product to cart in database
                val rowId = cartViewModel.addToCart(
                    productId,
                    productName,
                    quantity,
                    totalPrice.toDouble(),
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

            if (count < productStock) {
                count++
                productCartCountView.text = count.toString()
            } else {
                Toast.makeText(context, "Maximum stock reached", Toast.LENGTH_SHORT).show()
            }
        }
    }

}