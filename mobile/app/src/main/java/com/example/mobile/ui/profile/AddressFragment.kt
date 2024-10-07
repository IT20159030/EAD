package com.example.mobile.ui.profile

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
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
import com.google.android.material.textfield.TextInputLayout
import dagger.hilt.android.AndroidEntryPoint

/*
* A fragment that displays the user's address.
* And allows the user to update the address.
 */
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
        if (address == null || address.line1.isEmpty() || address.city.isEmpty() || address.postalCode.isEmpty() ) {
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
        val tilLine1 = dialogView.findViewById<TextInputLayout>(R.id.tilLine1)
        val tilLine2 = dialogView.findViewById<TextInputLayout>(R.id.tilLine2)
        val tilCity = dialogView.findViewById<TextInputLayout>(R.id.tilCity)
        val tilPostalCode = dialogView.findViewById<TextInputLayout>(R.id.tilPostalCode)

        val etLine1 = tilLine1.editText!!
        val etLine2 = tilLine2.editText!!
        val etCity = tilCity.editText!!
        val etPostalCode = tilPostalCode.editText!!

        addressData?.let {
            etLine1.setText(it.line1)
            etLine2.setText(it.line2)
            etCity.setText(it.city)
            etPostalCode.setText(it.postalCode)
        }

        val dialog = AlertDialog.Builder(requireContext())
            .setTitle(if (addressData == null) "Add Address" else "Update Address")
            .setView(dialogView)
            .setPositiveButton("Save", null)
            .setNegativeButton("Cancel", null)
            .create()

        dialog.setOnShowListener {
            val positiveButton = dialog.getButton(AlertDialog.BUTTON_POSITIVE)
            positiveButton.setOnClickListener {
                val newAddress = Address(
                    etLine1.text.toString(),
                    etLine2.text.toString(),
                    etCity.text.toString(),
                    etPostalCode.text.toString()
                )
                if (validateInput(tilLine1, tilLine2, tilCity, tilPostalCode)) {
                    viewModel.updateAddress(newAddress, object : CoroutinesErrorHandler {
                        override fun onError(message: String) {
                            showError(message)
                        }
                    })
                    dialog.dismiss()
                }
            }
        }

        dialog.show()
    }

    private fun validateInput(
        tilLine1: TextInputLayout,
        tilLine2: TextInputLayout,
        tilCity: TextInputLayout,
        tilPostalCode: TextInputLayout
    ): Boolean {
        var isValid = true

        if (tilLine1.editText?.text.toString().isEmpty()) {
            tilLine1.error = "Address Line 1 is required"
            isValid = false
        } else {
            tilLine1.error = null
        }

        if (tilCity.editText?.text.toString().isEmpty()) {
            tilCity.error = "City is required"
            isValid = false
        } else {
            tilCity.error = null
        }

        if (tilPostalCode.editText?.text.toString().isEmpty()) {
            tilPostalCode.error = "Postal Code is required"
            isValid = false
        } else {
            tilPostalCode.error = null
        }

        return isValid
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