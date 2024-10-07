package com.example.mobile.ui.profileEdit

import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.navigation.NavController
import androidx.navigation.Navigation
import com.example.mobile.databinding.FragmentEditProfileBinding
import com.example.mobile.dto.UserUpdateRequest
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import com.example.mobile.viewModels.TokenViewModel
import dagger.hilt.android.AndroidEntryPoint

/*
* A fragment that allows the user to edit their profile information.
 */
@AndroidEntryPoint
class EditProfile : Fragment() {

    private var _binding: FragmentEditProfileBinding? = null
    private val binding get() = _binding!!

    private val viewModel: EditProfileViewModel by viewModels()
    private val tokenViewModel: TokenViewModel by activityViewModels()

    private lateinit var navController: NavController

    private var initialFirstName = ""
    private var initialLastName = ""
    private var initialNic = ""

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentEditProfileBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        navController = Navigation.findNavController(view)

        setupInitialData()
        setupClickListeners()
        setupTextChangeListeners()
        setupObservers()
    }

    private fun setupInitialData() {
        // Get arguments and provide default values if needed
        arguments?.let { args ->
            val firstName = args.getString("firstName", "")
            val lastName = args.getString("lastName", "")
            val nic = args.getString("nic", "")

            with(binding) {
                firstNameEditText.setText(firstName.trim())
                lastNameEditText.setText(lastName.trim())
                nicEditText.setText(nic.trim())

                // Store initial values for comparison
                initialFirstName = firstName.trim()
                initialLastName = lastName.trim()
                initialNic = nic.trim()
            }
        } ?: run {
            // Handle case where arguments are null
            Log.e("EditProfile", "No arguments provided")
            navController.navigateUp()
        }
    }


    private fun setupClickListeners() {
        binding.saveButton.setOnClickListener {
            if (validateInputs()) {
                updateProfile()
            }
        }

        binding.cancelButton.setOnClickListener {
            navController.navigateUp()
        }

        binding.deactivateButton.setOnClickListener {
            showDeactivationConfirmationDialog()
        }
    }

    private fun validateInputs(): Boolean {
        var isValid = true

        // Validate First Name
        val firstName = binding.firstNameEditText.text.toString().trim()
        when {
            firstName.isEmpty() -> {
                binding.firstNameLayout.error = "First name is required"
                isValid = false
            }
            firstName.contains(" ") -> {
                binding.firstNameLayout.error = "First name should not contain spaces"
                isValid = false
            }
            else -> binding.firstNameLayout.error = null
        }

        // Validate Last Name
        val lastName = binding.lastNameEditText.text.toString().trim()
        when {
            lastName.isEmpty() -> {
                binding.lastNameLayout.error = "Last name is required"
                isValid = false
            }
            lastName.contains(" ") -> {
                binding.lastNameLayout.error = "Last name should not contain spaces"
                isValid = false
            }
            else -> binding.lastNameLayout.error = null
        }

        // Validate NIC
        val nic = binding.nicEditText.text.toString().trim()
        when {
            nic.isEmpty() -> {
                binding.nicLayout.error = "NIC is required"
                isValid = false
            }
            !isValidNIC(nic) -> {
                binding.nicLayout.error = "Invalid NIC format"
                isValid = false
            }
            else -> binding.nicLayout.error = null
        }

        return isValid
    }

    private fun setupTextChangeListeners() {
        val textWatcher = object : TextWatcher {
            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {}
            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {}
            override fun afterTextChanged(s: Editable?) {
                updateSaveButtonState()
                binding.errorTV.visibility = View.GONE
                clearErrors()
            }
        }

        binding.firstNameEditText.addTextChangedListener(textWatcher)
        binding.lastNameEditText.addTextChangedListener(textWatcher)
        binding.nicEditText.addTextChangedListener(textWatcher)
    }

    private fun clearErrors() {
        binding.firstNameLayout.error = null
        binding.lastNameLayout.error = null
        binding.nicLayout.error = null
        binding.errorTV.visibility = View.GONE
    }
    private fun updateSaveButtonState() {
        val isDataChanged = binding.firstNameEditText.text.toString() != initialFirstName ||
                binding.lastNameEditText.text.toString() != initialLastName ||
                binding.nicEditText.text.toString() != initialNic

        binding.saveButton.isEnabled = isDataChanged
    }

    private fun setupObservers() {
        viewModel.updateProfileResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    showLoading(false)
                    showMessage("Profile updated successfully")
                    navController.navigateUp()
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    showError(response.errorMessage)
                }
            }
        }

        viewModel.deactivationResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    showLoading(false)
                    tokenViewModel.deleteToken()
                    // go back to previous screen
                    // removed the explicit navigation since it would affect the back button press behaviour
                    navController.navigateUp()
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    showError(response.errorMessage)
                }
            }
        }
    }

    private fun updateProfile() {
        val firstName = binding.firstNameEditText.text.toString()
        val lastName = binding.lastNameEditText.text.toString()
        val nic = binding.nicEditText.text.toString()

        viewModel.updateAccount(UserUpdateRequest(firstName, lastName, nic), object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                showError(message)
            }
        })
    }

    private fun showDeactivationConfirmationDialog() {
        AlertDialog.Builder(requireContext())
            .setTitle("Deactivate Account")
            .setMessage("Are you sure you want to deactivate your account? To reactivate it, you will need to contact a staff member.")
            .setPositiveButton("Deactivate") { _, _ ->
                deactivateAccount()
            }
            .setNegativeButton("Cancel", null)
            .show()
    }

    private fun deactivateAccount() {
        viewModel.deactivateAccount(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                showError(message)
            }
        })
    }

    private fun showLoading(isLoading: Boolean) {
        binding.loading.visibility = if (isLoading) View.VISIBLE else View.GONE
        binding.saveButton.isEnabled = !isLoading
        binding.cancelButton.isEnabled = !isLoading
        binding.deactivateButton.isEnabled = !isLoading
    }

    private fun showError(message: String) {
        binding.errorTV.text = message
        binding.errorTV.visibility = View.VISIBLE
    }

    private fun showMessage(message: String) {
        Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }

    private fun isValidNIC(nic: String): Boolean {
        val twelveDigitPattern = "^\\d{12}$".toRegex()
        val nineDigitWithVPattern = "^\\d{9}[vV]$".toRegex()
        return twelveDigitPattern.matches(nic) || nineDigitWithVPattern.matches(nic)
    }
}