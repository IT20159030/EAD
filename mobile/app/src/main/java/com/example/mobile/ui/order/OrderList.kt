package com.example.mobile.ui.order

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.RecyclerView
import com.example.mobile.R
import com.example.mobile.databinding.FragmentOrderListBinding
import com.example.mobile.dto.OrderResponse
import dagger.hilt.android.AndroidEntryPoint

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

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
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
        val orderAdapter = OrderCardAdapter(orderResponses, requireContext()) { _id ->
            // TODO: Handle cancel order action
        }
        orderListRecyclerView.adapter = orderAdapter
        
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }


}