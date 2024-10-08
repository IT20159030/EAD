package com.example.mobile.ui.vendorView

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ProgressBar
import android.widget.RatingBar
import android.widget.TextView
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.navigation.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.mobile.R
import com.example.mobile.databinding.FragmentViewVendorBinding
import com.example.mobile.dto.Vendor
import com.example.mobile.ui.productView.ProductAdapter
import com.example.mobile.ui.productView.ProductViewModel
import com.example.mobile.ui.productView.VendorViewModel
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class ViewVendorFragment : Fragment() {
    private var _binding: FragmentViewVendorBinding? = null
    private val binding get() = _binding!!

    private lateinit var vendorName: TextView
    private lateinit var vendorPhone: TextView
    private lateinit var vendorEmail: TextView
    private lateinit var vendorAddress: TextView
    private lateinit var vendorCity: TextView
    private lateinit var vendorRating: RatingBar
    private lateinit var vendorRatingCount: TextView
    private lateinit var vendorRecyclerView: RecyclerView
    private lateinit var vendorLoadingIndicator: ProgressBar
    private lateinit var vendorErrorText: TextView
    private lateinit var vendorNoProductsText: TextView

    private val vendorViewModel: VendorViewModel by viewModels()
    private val productViewModel: ProductViewModel by viewModels()

    private lateinit var productAdapter: ProductAdapter

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentViewVendorBinding.inflate(inflater, container, false)
        val root: View = binding.root

        // Bind views
        vendorName = binding.vendorName
        vendorPhone = binding.vendorPhone
        vendorEmail = binding.vendorEmail
        vendorAddress = binding.vendorAddress
        vendorCity = binding.vendorCity
        vendorRating = binding.vendorRating
        vendorRatingCount = binding.vendorRatingCount
        vendorRecyclerView = binding.vendorProductsList
        vendorLoadingIndicator = binding.vendorLoadingIndicator
        vendorErrorText = binding.vendorErrorText
        vendorNoProductsText = binding.vendorProductsEmptyText

        // Setup RecyclerView
        vendorRecyclerView.layoutManager = LinearLayoutManager(context)

        return root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        productAdapter = ProductAdapter(listOf(), view.findNavController())
        vendorRecyclerView.adapter = productAdapter

        // Get vendor details from arguments
        val vendorId = arguments?.getString("vendorId")
        val vendorName = arguments?.getString("vendorName")
        val vendorPhone = arguments?.getString("vendorPhone")
        val vendorEmail = arguments?.getString("vendorEmail")
        val vendorAddress = arguments?.getString("vendorAddress")
        val vendorCity = arguments?.getString("vendorCity")
        val vendorRating = arguments?.getFloat("vendorRating")
        val vendorRatingCount = arguments?.getInt("vendorRatingCount")

        // Populate views with vendor details
        this.vendorName.text = vendorName
        this.vendorPhone.text = vendorPhone
        this.vendorEmail.text = vendorEmail
        this.vendorAddress.text = vendorAddress
        this.vendorCity.text = vendorCity
        this.vendorRating.rating = vendorRating ?: 0f
        this.vendorRatingCount.text = String.format("(%d)", vendorRatingCount ?: 0)

        // Fetch vendor details from ViewModel
        vendorViewModel.getVendorById(vendorId!!, object : CoroutinesErrorHandler{
            override fun onError(message: String) {
                handleError(message)
            }
        })
        productViewModel.getProductsByVendor(vendorId, object : CoroutinesErrorHandler{
            override fun onError(message: String) {
                handleError(message)
            }
        })

        // Observe vendor data
        vendorViewModel.vendor.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Success -> {
                    val vendor = response.data
                    this.vendorName.text = vendor.vendorName
                    this.vendorPhone.text = vendor.vendorPhone
                    this.vendorEmail.text = vendor.vendorEmail
                    this.vendorAddress.text = vendor.vendorAddress
                    this.vendorCity.text = vendor.vendorCity
                    this.vendorRating.rating = vendor.vendorRating.toFloat()
                    this.vendorRatingCount.text = String.format("(%d)", vendor.vendorRatingCount)
                    vendorLoadingIndicator.visibility = View.GONE
                }
                is ApiResponse.Failure -> {
                    vendorErrorText.visibility = View.VISIBLE
                    vendorErrorText.text = response.errorMessage
                    vendorLoadingIndicator.visibility = View.GONE
                }

                ApiResponse.Loading -> {
                    vendorLoadingIndicator.visibility = View.VISIBLE
                }
            }
        }

        // Observe product data
        productViewModel.products.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Success -> {
                    val products = response.data
                    if (products.isEmpty()) {
                        vendorNoProductsText.visibility = View.VISIBLE
                    } else {
                        productAdapter.updateList(products)
                    }
                    vendorLoadingIndicator.visibility = View.GONE
                }
                is ApiResponse.Failure -> {
                    vendorErrorText.visibility = View.VISIBLE
                    vendorErrorText.text = response.errorMessage
                    vendorLoadingIndicator.visibility = View.GONE
                }

                ApiResponse.Loading -> {
                    vendorLoadingIndicator.visibility = View.VISIBLE
                }
            }
        }
    }

    private fun handleError(message: String) {
        Toast.makeText(requireContext(), message, Toast.LENGTH_SHORT).show()
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
