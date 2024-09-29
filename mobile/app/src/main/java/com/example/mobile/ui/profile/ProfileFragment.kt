package com.example.mobile.ui.profile

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.navigation.NavController
import androidx.navigation.Navigation
import com.example.mobile.R
import com.example.mobile.databinding.FragmentProfileBinding
import com.example.mobile.dto.UserInfo
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import com.example.mobile.viewModels.TokenViewModel
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class ProfileFragment : Fragment() {

    private var _binding: FragmentProfileBinding? = null
    private val binding get() = _binding!!

    private val viewModel: ProfileViewModel by viewModels()
    private val tokenViewModel: TokenViewModel by activityViewModels()

    private lateinit var navController: NavController

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentProfileBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        navController = Navigation.findNavController(view)

        setupObservers()
        setupClickListeners()
    }

    private fun setupObservers() {
        tokenViewModel.token.observe(viewLifecycleOwner) { token ->
            if (token == null) {
                navController.navigate(R.id.action_global_loginFragment)
            } else {
                fetchUserInfo()
            }
        }

        viewModel.userInfoResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> showLoading(true)
                is ApiResponse.Success -> {
                    showLoading(false)
                    updateUserInfo(response.data.data)
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    showError(response.errorMessage)
                }
            }
        }
    }

    private fun setupClickListeners() {
        with(binding) {
            logoutButton.setOnClickListener {
                tokenViewModel.deleteToken()
            }

            cartButton.setOnClickListener {
                // TODO: Navigate to cart
            }

            ordersButton.setOnClickListener {
                // TODO: Navigate to orders
            }

            wishlistButton.setOnClickListener {
                // TODO: Navigate to wishlist
            }

            addressesButton.setOnClickListener {
                // TODO: Navigate to addresses
            }

            editProfileButton.setOnClickListener {
                // TODO: Navigate to edit profile
            }
        }
    }

    private fun fetchUserInfo() {
        viewModel.getUserInfo(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                showError(message)
            }
        })
    }

    private fun updateUserInfo(userData: UserInfo) {
        with(binding) {
            profileName.text = userData.name
            profileEmail.text = userData.email
        }
    }

    private fun showLoading(isLoading: Boolean) {
        binding.loadingSpinner.visibility = if (isLoading) View.VISIBLE else View.GONE
        binding.actionsCard.isEnabled = !isLoading
        binding.editProfileButton.isEnabled = !isLoading
        binding.logoutButton.isEnabled = !isLoading
    }

    private fun showError(message: String) {
        binding.errorTV.text = message
        binding.errorTV.visibility = View.VISIBLE
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}

