package com.example.mobile.ui.home

import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.EditText
import android.widget.ImageView
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.NavController
import androidx.navigation.Navigation
import androidx.navigation.fragment.NavHostFragment.Companion.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.mobile.R
import com.example.mobile.databinding.FragmentHomeBinding
import com.squareup.picasso.Picasso

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

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        val homeViewModel =
            ViewModelProvider(this)[HomeViewModel::class.java]

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
        loadDummyProducts()
    }

    // Load some dummy products
    private fun loadDummyProducts() {
        productList.addAll(
            listOf(
                Product("Product 1", "$10.00", "https://images2.alphacoders.com/655/655076.jpg"),
                Product("Product 2", "$15.00", "https://static1.cbrimages.com/wordpress/wp-content/uploads/2023/08/gear-5-luffy-smiling-at-kaido-during-their-fight-in-one-piece.jpg"),
                Product("Product 3", "$20.00", "https://images2.alphacoders.com/655/655076.jpg"),
                Product("Product 4", "$25.00", "https://images2.alphacoders.com/655/655076.jpg"),
                Product("Product 5", "$30.00", "https://images2.alphacoders.com/655/655076.jpg")
            )
        )
        productAdapter.notifyDataSetChanged()
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
}

data class Product(val name: String, val price: String, val imageUrl: String)

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
            productNameTextView.text = product.name
            productPriceTextView.text = product.price
            Picasso.get().load(product.imageUrl).into(productImageView)
            productCardView.setOnClickListener {
                val bundle = Bundle().apply {
                    putString("productName", product.name)
                    putString("productPrice", product.price)
                    putString("productImageUrl", product.imageUrl)
                    putString("productDescription", "This is a sample product description.")
                    putString("productCategory", "Sample Category")
                }
                // navigate to product view
                navController.navigate(R.id.action_navigation_home_to_viewProduct, bundle)
            }
        }
    }
}