package com.example.mobile.ui.auth.register

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.navigation.NavController
import androidx.navigation.Navigation
import com.example.mobile.R
import com.example.mobile.databinding.FragmentRegisterBinding
import com.example.mobile.dto.RegisterRequest
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.AuthViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import com.example.mobile.viewModels.TokenViewModel
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class RegisterFragment : Fragment() {

    private lateinit var binding: FragmentRegisterBinding
    private val viewModel: AuthViewModel by viewModels()
    private lateinit var navController: NavController

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        binding = FragmentRegisterBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        navController = Navigation.findNavController(view)

        binding.registerButton.setOnClickListener {
            if (validateInputs()) {
                performRegistration()
            }
        }

        binding.backToLoginButton.setOnClickListener {
            navController.navigate(R.id.action_registerFragment_to_loginFragment)
        }

        setupObservers()
    }

    private fun setupObservers() {
        viewModel.registerResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    showLoading(false)
                    showSuccessMessage("Registration successful. Please wait for admin approval.")
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    showError(response.errorMessage)
                }
            }
        }
    }

    private fun validateInputs(): Boolean {
        var isValid = true

        // Validate First Name
        if (binding.firstNameEditText.text.toString().trim().isEmpty()) {
            binding.firstNameLayout.error = "First name is required"
            isValid = false
        } else {
            binding.firstNameLayout.error = null
        }

        // Validate Last Name
        if (binding.lastNameEditText.text.toString().trim().isEmpty()) {
            binding.lastNameLayout.error = "Last name is required"
            isValid = false
        } else {
            binding.lastNameLayout.error = null
        }

        // Validate email
        val email = binding.emailEditText.text.toString().trim()
        if (email.isEmpty()) {
            binding.emailLayout.error = "Email is required"
            isValid = false
        } else if (!isValidEmail(email)) {
            binding.emailLayout.error = "Invalid email format"
            isValid = false
        } else {
            binding.emailLayout.error = null
        }

        // Validate NIC
        val nic = binding.nicEditText.text.toString().trim()
        if (nic.isEmpty()) {
            binding.nicEditLayout.error = "NIC is required"
            isValid = false
        } else if (!isValidNIC(nic)) {
            binding.nicEditLayout.error = "Invalid NIC format"
            isValid = false
        } else {
            binding.nicEditLayout.error = null
        }

        // Validate password
        val password = binding.passwordEditText.text.toString()
        if (password.isEmpty()) {
            binding.passwordLayout.error = "Password is required"
            isValid = false
        } else if (password.length < 8) {
            binding.passwordLayout.error = "Password must be at least 8 characters long"
            isValid = false
        } else {
            binding.passwordLayout.error = null
        }

        // Validate confirm password
        val confirmPassword = binding.confirmPasswordEditText.text.toString()
        if (confirmPassword.isEmpty()) {
            binding.confirmPasswordLayout.error = "Confirm password is required"
            isValid = false
        } else if (password != confirmPassword) {
            binding.confirmPasswordLayout.error = "Passwords do not match"
            isValid = false
        } else {
            binding.confirmPasswordLayout.error = null
        }

        return isValid
    }

    private fun isValidEmail(email: String): Boolean {
        return android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()
    }

    private fun isValidNIC(nic: String): Boolean {
        val twelveDigitPattern = "^\\d{12}$".toRegex()
        val nineDigitWithVPattern = "^\\d{9}[vV]$".toRegex()
        return twelveDigitPattern.matches(nic) || nineDigitWithVPattern.matches(nic)
    }

    private fun performRegistration() {
        val firstName = binding.firstNameEditText.text.toString()
        val lastName = binding.lastNameEditText.text.toString()
        val email = binding.emailEditText.text.toString()
        val nic = binding.nicEditText.text.toString()
        val password = binding.passwordEditText.text.toString()

        println("First Name: $firstName")
        println("Last Name: $lastName")
        println("Email: $email")
        println("NIC: $nic")
        println("Password: $password")

        viewModel.register(  RegisterRequest(
            firstName,
            lastName,
            email,
            nic,
            password
        ), object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                showError(message)
            }
        })
    }

    private fun showLoading(isLoading: Boolean) {

        binding.loadingProgressBar.visibility = if (isLoading) View.VISIBLE else View.GONE
        binding.registerButton.isEnabled = !isLoading

    }

    private fun showError(message: String) {
        binding.errorTextView.visibility = View.VISIBLE
        binding.errorTextView.text = message
    }

    private fun showSuccessMessage(message: String) {
        AlertDialog.Builder(requireContext())
            .setTitle("Success")
            .setMessage(message)
            .setPositiveButton("OK") { dialog, _ ->
                dialog.dismiss()
            }
            .setCancelable(false) // Ensure the user cannot dismiss the dialog by clicking outside
            .setOnDismissListener {
                navController.navigate(R.id.action_registerFragment_to_loginFragment_no_back)
            }
            .show()
    }

}