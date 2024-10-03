package com.example.mobile.ui.order

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.mobile.databinding.FragmentOrderListBinding
import com.example.mobile.dto.OrderResponse
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.AndroidEntryPoint

/*
*
* A fragment that displays a list of orders.
* Allows to cancel orders.
*
* */

@AndroidEntryPoint
class OrderList : Fragment() {
    private var _binding: FragmentOrderListBinding? = null
    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!
    private val orderResponses = mutableListOf<OrderResponse>()

    private lateinit var orderListRecyclerView: RecyclerView
    private lateinit var orderListLoadingIndicator: View
    private lateinit var orderListErrorText: TextView
    private lateinit var orderAdapter: OrderCardAdapter

    private val orderViewModel: OrderViewModel by viewModels()

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentOrderListBinding.inflate(inflater, container, false)
        val root: View = binding.root

        // get views
        orderListRecyclerView = binding.orderListRecyclerView
        orderListLoadingIndicator = binding.orderListLoadingIndicator
        orderListErrorText = binding.orderListErrorText

        return root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        // Initialize RecyclerView and Adapter
        orderAdapter = OrderCardAdapter(orderResponses, requireContext()) { id ->
            // TODO: Handle cancel order action
        }
        orderListRecyclerView.layoutManager = LinearLayoutManager(context)
        orderListRecyclerView.adapter = orderAdapter

        setupObservers()
        loadOrders()
    }

    private fun setupObservers() {
        orderViewModel.orderGetResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    showLoading(false)
                    orderResponses.clear()
                    orderResponses.addAll(response.data)
                    orderAdapter.updateList(response.data)
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    showError(response.errorMessage)
                }
            }
        }
    }

    private fun loadOrders() {
        orderViewModel.getOrdersRequest(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                showError(message)
            }
        })
    }

    private fun showLoading(loading: Boolean) {
        if (loading) {
            orderListLoadingIndicator.visibility = View.VISIBLE
        } else {
            orderListLoadingIndicator.visibility = View.GONE
        }
    }

    private fun showError(message: String) {
        orderListErrorText.text = message
        orderListErrorText.visibility = View.VISIBLE
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}