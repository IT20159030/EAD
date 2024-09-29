package com.example.mobile.ui.login


import android.content.Intent
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.navigation.NavController
import androidx.navigation.Navigation
import com.example.mobile.ProductViewActivity
import com.example.mobile.R
import com.example.mobile.databinding.FragmentLoginBinding
import com.example.mobile.dto.LoginRequest
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.AuthViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import com.example.mobile.viewModels.TokenViewModel

import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class LoginFragment : Fragment() {
    private val viewModel: AuthViewModel by viewModels()
    private val tokenViewModel: TokenViewModel by activityViewModels()

    private var _binding: FragmentLoginBinding? = null
    private val binding get() = _binding!!

    private lateinit var navController: NavController

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentLoginBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        navController = Navigation.findNavController(view)

        binding.registerTextView.setOnClickListener {
            // Handle navigation to registration page
           navController.navigate(R.id.action_loginFragment_to_registerFragment)
        }

        setupObservers()
        setupClickListeners()
    }

    private fun setupObservers() {
        tokenViewModel.token.observe(viewLifecycleOwner) { token ->
            if (token != null) {
                navController.navigate(R.id.action_loginFragment_to_main_nav_graph)
            }
        }

        viewModel.loginResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    showLoading(false)
                    tokenViewModel.saveToken(response.data.token)
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    showError(response.errorMessage)
                }
            }
        }
        //TODO: temp remove this
        view?.findViewById<Button>(R.id.test_product_view_button)?.setOnClickListener {
            val productViewActivityIntent = Intent(context, ProductViewActivity::class.java)
            startActivity(productViewActivityIntent)
        }
    }

    private fun setupClickListeners() {
        binding.loginButton.setOnClickListener {
            val email = binding.emailEditText.text.toString()
            val password = binding.passwordEditText.text.toString()

            if (validateInput(email, password)) {
                login(email, password)
            }
        }
    }

    private fun validateInput(email: String, password: String): Boolean {
        var isValid = true

        if (email.isEmpty()) {
            binding.emailInputLayout.error = "Error_email_required"
            isValid = false

        } else {
            binding.emailInputLayout.error = null
        }
        if (!android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
            binding.emailInputLayout.error = "Error_invalid_email"
            isValid = false
            binding.emailEditText.requestFocus()
        }

        if (password.isEmpty()) {
            binding.passwordInputLayout.error = "Error_password_required"
            isValid = false
        } else {
            binding.passwordInputLayout.error = null
        }

        return isValid
    }

    private fun login(email: String, password: String) {
        viewModel.login(
            LoginRequest(email, password),
            object : CoroutinesErrorHandler {
                override fun onError(message: String) {
                    showLoading(false)
                    showError(message)
                }
            }
        )
    }

    private fun showLoading(isLoading: Boolean) {
        binding.loadingProgressBar.visibility = if (isLoading) View.VISIBLE else View.GONE
        binding.loginButton.isEnabled = !isLoading
    }

    private fun showError(message: String) {
        binding.errorTextView.text = message
        binding.errorTextView.visibility = View.VISIBLE
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}