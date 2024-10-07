package com.example.mobile.ui.home

import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.EditText
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.navigation.NavController
import androidx.navigation.Navigation
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.chip.Chip
import com.example.mobile.R
import com.example.mobile.dto.Product
import com.example.mobile.dto.ProductCategory
import com.example.mobile.dto.Vendor
import com.example.mobile.databinding.FragmentHomeBinding
import com.example.mobile.ui.productView.ProductAdapter
import com.example.mobile.ui.vendorView.VendorAdapter
import com.example.mobile.ui.productView.ProductViewModel
import com.example.mobile.ui.productView.VendorViewModel
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import com.example.mobile.viewModels.TokenViewModel
import dagger.hilt.android.AndroidEntryPoint

/*
* A Fragment class for the home page.
* Displays a list of products.
* Allows the user to search for products.
* */

@AndroidEntryPoint
class HomeFragment : Fragment() {

    private var _binding: FragmentHomeBinding? = null
    private lateinit var productAdapter: ProductAdapter
    private lateinit var vendorAdapter: VendorAdapter
    private val productList = mutableListOf<Product>()
    private val vendorList = mutableListOf<Vendor>()
    private lateinit var searchBar: EditText
    private lateinit var productsRecyclerView: RecyclerView
    private lateinit var vendorsRecyclerView: RecyclerView

    private lateinit var navController: NavController

    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    private val productViewModel: ProductViewModel by viewModels()
    private val vendorViewModel: VendorViewModel by viewModels()
    private val tokenViewModel: TokenViewModel by activityViewModels()

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentHomeBinding.inflate(inflater, container, false)
        val root: View = binding.root

        searchBar = binding.searchBar
        productsRecyclerView = binding.productsRecyclerView
        vendorsRecyclerView = binding.vendorsRecyclerView

        return root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        navController = Navigation.findNavController(view)

        // Initialize RecyclerViews and adapters
        productAdapter = ProductAdapter(productList, navController)
        productsRecyclerView.layoutManager = LinearLayoutManager(context)
        productsRecyclerView.adapter = productAdapter

        vendorAdapter = VendorAdapter(vendorList, navController)
        vendorsRecyclerView.layoutManager = LinearLayoutManager(context)
        vendorsRecyclerView.adapter = vendorAdapter

        // Initially, show products view
        productsRecyclerView.visibility = View.VISIBLE
        vendorsRecyclerView.visibility = View.GONE

        // Handle toggle between Products and Vendors
        binding.viewToggleGroup.setOnCheckedChangeListener { _, checkedId ->
            when (checkedId) {
                R.id.products_toggle -> {
                    productsRecyclerView.visibility = View.VISIBLE
                    vendorsRecyclerView.visibility = View.GONE
                    filterProducts(searchBar.text.toString())  // Load products
                }
                R.id.vendors_toggle -> {
                    productsRecyclerView.visibility = View.GONE
                    vendorsRecyclerView.visibility = View.VISIBLE
                    filterVendors(searchBar.text.toString())   // Load vendors
                }

            }
        }

        // Set up search functionality
        searchBar.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                if (binding.productsToggle.isChecked) {
                    filterProducts(s.toString())
                } else {
                    filterVendors(s.toString())
                }
            }

            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {}
            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {}
        })

        // Load initial product list
        loadProductList()
        loadVendorList()
        loadCategories()
    }

    private fun loadProductList() {

        tokenViewModel.token.observe(viewLifecycleOwner) { token ->
            if (token == null) {
                navController.navigate(R.id.action_global_loginFragment)
            } else {
                filterProducts("")
            }
        }

        productViewModel.products.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)

                is ApiResponse.Success -> {
                    showLoading(false)
                    productList.clear()
                    productList.addAll(response.data)
                    productAdapter.updateList(productList)
                }

                is ApiResponse.Failure -> {
                    showLoading(false)
                    Toast.makeText(context, response.errorMessage, Toast.LENGTH_SHORT).show()
                    showError(response.errorMessage)
                }
            }
        }
    }

    private fun loadVendorList() {
        vendorViewModel.vendors.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)

                is ApiResponse.Success -> {
                    showLoading(false)
                    vendorList.clear()
                    vendorList.addAll(response.data)
                    vendorAdapter.updateList(vendorList)
                }

                is ApiResponse.Failure -> {
                    showLoading(false)
                    Toast.makeText(context, response.errorMessage, Toast.LENGTH_SHORT).show()
                    showError(response.errorMessage)
                }
            }
        }
    }

    private fun loadCategories() {
        productViewModel.productCategories.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)

                is ApiResponse.Success -> {
                    showLoading(false)
                    setupCategoryChips(response.data)
                }

                is ApiResponse.Failure -> {
                    showLoading(false)
                    Toast.makeText(context, response.errorMessage, Toast.LENGTH_SHORT).show()
                }
            }
        }

        productViewModel.getProductCategories(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
            }
        })
    }

    private fun setupCategoryChips(categories: List<ProductCategory>) {
        val chipGroup = binding.categoryChipGroup
        chipGroup.removeAllViews()

        for (category in categories) {
            val chip = LayoutInflater.from(context).inflate(R.layout.item_chip, chipGroup, false) as Chip
            chip.text = category.name
            chip.isCheckable = true
            chip.setOnCheckedChangeListener { _, isChecked ->
                if (isChecked) {
                    filterProductsByCategory(category.categoryId)
                } else {
                    loadProductList() // Reload all products if no category is selected
                }
            }
            chipGroup.addView(chip)
        }
    }


    // Filter products based on search query
    private fun filterProducts(query: String) {
        showLoading(true)
        if (query.isEmpty()) {
            fetchAllProducts()
        } else {
            searchProducts(query)
        }
    }

    // Filter vendors based on search query
    private fun filterVendors(name: String) {
        showLoading(true)
        if (name.isEmpty()) {
            fetchAllVendors()
        } else {
            searchVendors(name)
        }
    }

    // Filter products based on category
    private fun filterProductsByCategory(categoryId: String) {
        productViewModel.getProductsByCategory(categoryId, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
            }
        })
    }


    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
        productList.clear()
        vendorList.clear()
    }

    private fun fetchAllProducts() {
        productViewModel.getProducts(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
                showError(message)
            }
        })
    }

    private fun fetchAllVendors() {
        vendorViewModel.getVendors(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
                showError(message)
            }
        })
    }

    private fun searchProducts(query: String) {
        productViewModel.searchProducts(query, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
                showError(message)
            }
        })
    }

    private fun searchVendors(name: String) {
        vendorViewModel.searchVendors(name, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
                showError(message)
            }
        })
    }

    private fun showLoading(loading: Boolean) {
        if (loading) {
            binding.homeLoadingIndicator.visibility = View.VISIBLE
        } else {
            binding.homeLoadingIndicator.visibility = View.GONE
        }
    }

    private fun showError(message: String) {
        binding.homeErrorText.visibility = View.VISIBLE
        binding.homeErrorText.text = message
    }

}