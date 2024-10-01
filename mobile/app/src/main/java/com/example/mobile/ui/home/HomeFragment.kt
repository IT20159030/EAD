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
import com.example.mobile.R
import com.example.mobile.dto.Product
import com.example.mobile.databinding.FragmentHomeBinding
import com.example.mobile.ui.productView.ProductAdapter
import com.example.mobile.ui.productView.ProductViewModel
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import com.example.mobile.viewModels.TokenViewModel
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class HomeFragment : Fragment() {

    private var _binding: FragmentHomeBinding? = null
    private lateinit var productAdapter: ProductAdapter
    private val productList = mutableListOf<Product>()
    private lateinit var searchBar: EditText
    private lateinit var productsRecyclerView: RecyclerView

    private lateinit var navController: NavController

    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    private val productViewModel: ProductViewModel by viewModels()
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

        return root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        navController = Navigation.findNavController(view)

        // Initialize RecyclerView and Adapter
        productAdapter = ProductAdapter(productList, navController)
        productsRecyclerView.layoutManager = LinearLayoutManager(context)
        productsRecyclerView.adapter = productAdapter

        // Set up search functionality
        searchBar.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                filterProducts(s.toString())
            }

            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {}

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {}
        })

        // Load sample products
        loadProductList()
    }

    private fun loadProductList() {

        tokenViewModel.token.observe(viewLifecycleOwner) { token ->
            if (token == null) {
                navController.navigate(R.id.action_global_loginFragment)
            } else {
                fetchProductInfo()
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

    // Filter products based on search query
    private fun filterProducts(query: String) {
        val filteredList = productList.filter {
            it.name.contains(query, ignoreCase = true)
        }
        productAdapter.updateList(filteredList)
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
        productList.clear()
    }

    private fun fetchProductInfo() {
        productViewModel.getProducts(object : CoroutinesErrorHandler {
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