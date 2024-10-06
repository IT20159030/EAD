package com.example.mobile.ui.profile

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.core.view.isVisible
import androidx.fragment.app.viewModels
import androidx.navigation.NavController
import androidx.navigation.Navigation
import com.example.mobile.R
import com.example.mobile.databinding.FragmentAddressBinding
import com.example.mobile.dto.Address
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class AddressFragment: Fragment() {

    private var _binding: FragmentAddressBinding? = null
    private val binding get() = _binding!!

    private val viewModel: AddressViewModel by viewModels()
    private lateinit var navController: NavController
    private var addressData: Address? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentAddressBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        navController = Navigation.findNavController(view)
        setupObservers()
        setupListeners()
        viewModel.getAddresses(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showError(message)
            }
        })
    }

    private fun setupObservers() {
        viewModel.addressResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    showLoading(false)
                    addressData = response.data.data
                    updateAddressUI(addressData)
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    showError(response.errorMessage)
                }
            }
        }
    }

    private fun setupListeners() {
        binding.btnAddUpdateAddress.setOnClickListener {
            showAddressDialog()
        }
    }

    private fun updateAddressUI(address: Address?) {
        if (address == null) {
            binding.tvAddressStatus.text = "No address set"
            binding.btnAddUpdateAddress.text = "Add Address"
        } else {
            binding.tvAddressStatus.text = buildAddressString(address)
            binding.btnAddUpdateAddress.text = "Update Address"
        }
    }

    private fun buildAddressString(address: Address): String {
        return "${address.line1}\n${address.line2}\n${address.city}, ${address.postalCode}"
    }

    private fun showAddressDialog() {
        val dialogView = layoutInflater.inflate(R.layout.dialog_address, null)
        val etLine1 = dialogView.findViewById<EditText>(R.id.etLine1)
        val etLine2 = dialogView.findViewById<EditText>(R.id.etLine2)
        val etCity = dialogView.findViewById<EditText>(R.id.etCity)
        val etPostalCode = dialogView.findViewById<EditText>(R.id.etPostalCode)

        addressData?.let {
            etLine1.setText(it.line1)
            etLine2.setText(it.line2)
            etCity.setText(it.city)
            etPostalCode.setText(it.postalCode)
        }

        AlertDialog.Builder(requireContext())
            .setTitle(if (addressData == null) "Add Address" else "Update Address")
            .setView(dialogView)
            .setPositiveButton("Save") { _, _ ->
                val newAddress = Address(
                    etLine1.text.toString(),
                    etLine2.text.toString(),
                    etCity.text.toString(),
                    etPostalCode.text.toString()
                )
                viewModel.updateAddress(newAddress, object : CoroutinesErrorHandler {
                    override fun onError(message: String) {
                        showError(message)
                    }
                })
            }
            .setNegativeButton("Cancel", null)
            .show()
    }

    private fun showLoading(isLoading: Boolean) {
        binding.progressBar.isVisible = isLoading
    }

    private fun showError(message: String) {
        Toast.makeText(requireContext(), message, Toast.LENGTH_SHORT).show()
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}