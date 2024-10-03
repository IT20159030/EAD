package com.example.mobile.ui.order

import android.app.AlertDialog
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.EditText
import android.widget.RadioGroup
import android.widget.TextView
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.mobile.R
import com.example.mobile.databinding.FragmentOrderListBinding
import com.example.mobile.dto.OrderCancelRequest
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
class OrderListFragment : Fragment() {
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
            showCancelOrderDialog { reason ->
                sendCancelOrderRequest(id, reason)
            }
        }
        orderListRecyclerView.layoutManager = LinearLayoutManager(context)
        orderListRecyclerView.adapter = orderAdapter

        setupObservers()
        loadOrders()
    }

    private fun sendCancelOrderRequest(orderId: String, reason: String) {
        val orderCancelRequest = OrderCancelRequest("", "", reason)
        orderViewModel.cancelOrderRequest(orderId, orderCancelRequest, object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                showError(message)
            }
        })
    }

    private fun showCancelOrderDialog(cancelHandle: (String) -> Unit) {
        // Inflate the custom layout
        val dialogView = LayoutInflater.from(context).inflate(R.layout.cancel_order_dialog, null)
        val radioGroup = dialogView.findViewById<RadioGroup>(R.id.reasonRadioGroup)
        val otherReasonEditText = dialogView.findViewById<EditText>(R.id.otherReasonEditText)

        // Build the dialog
        val dialog = AlertDialog.Builder(context)
            .setView(dialogView)
            .setTitle("Cancel Order")
            .setPositiveButton("Confirm") { dialogInterface, _ ->
                // Get the selected radio button ID
                val selectedRadioButtonId = radioGroup.checkedRadioButtonId
                val reason = when (selectedRadioButtonId) {
                    R.id.wrongOrderRadioButton -> getString(R.string.wrong_order)
                    R.id.noNeedRadioButton -> getString(R.string.no_longer_need_item)
                    R.id.otherReasonRadioButton -> otherReasonEditText.text.toString().trim()
                    else -> ""
                }

                // Execute the cancel handle with the selected reason
                cancelHandle(reason)
                dialogInterface.dismiss()
            }
            .setNegativeButton("Cancel") { dialogInterface, _ ->
                dialogInterface.dismiss()
            }
            .create()

        // Show the EditText when "Other" is selected
        radioGroup.setOnCheckedChangeListener { _, checkedId ->
            if (checkedId == R.id.otherReasonRadioButton) {
                otherReasonEditText.visibility = View.VISIBLE
            } else {
                otherReasonEditText.visibility = View.GONE
            }
        }

        // Show the dialog
        dialog.show()
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

        orderViewModel.orderCancelResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    showLoading(false)
                    loadOrders()
                    Toast.makeText(context, "Order canceled successfully", Toast.LENGTH_SHORT).show()
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    showError(response.errorMessage)
                    Toast.makeText(context, "Order canceled failed", Toast.LENGTH_SHORT).show()
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