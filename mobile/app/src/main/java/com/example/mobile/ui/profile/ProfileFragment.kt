package com.example.mobile.ui.profile

import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.navigation.NavController
import androidx.navigation.Navigation
import com.example.mobile.R
import com.example.mobile.databinding.FragmentProfileBinding
import com.example.mobile.dto.UserInfo
import com.example.mobile.ui.cart.CartViewModel
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import com.example.mobile.viewModels.TokenViewModel
import dagger.hilt.android.AndroidEntryPoint

/*
* A fragment that displays the user's profile information.
* And allows user to
* 1. Logout
* 2. Navigate to View orders
* 3. Navigate to View address
* 4. Navigate to Edit profile
 */

@AndroidEntryPoint
class ProfileFragment : Fragment() {

    private var _binding: FragmentProfileBinding? = null
    private val binding get() = _binding!!

    private val viewModel: ProfileViewModel by viewModels()
    private val tokenViewModel: TokenViewModel by activityViewModels()
    private val cartViewModel: CartViewModel by viewModels()

    private lateinit var navController: NavController

    // current user
    private lateinit var currentUserInfo: UserInfo

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
        // TODO: might need to move this to a base fragment
        tokenViewModel.token.observe(viewLifecycleOwner) { token ->
            if (token == null) {
                navController.navigate(R.id.action_global_loginFragment)
            } else {
                fetchUserInfo()
            }
        }

        viewModel.userInfoResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading -> {
                    showLoading(true)
                    binding.editProfileButton.isEnabled = false
                }
                is ApiResponse.Success -> {
                    showLoading(false)
                    currentUserInfo = response.data.data
                    binding.editProfileButton.isEnabled = true
                    updateUserInfo(response.data.data)
                }
                is ApiResponse.Failure -> {
                    showLoading(false)
                    binding.editProfileButton.isEnabled = false
                    showError(response.errorMessage)
                }
            }
        }
    }

    private fun setupClickListeners() {
        with(binding) {
            logoutButton.setOnClickListener {
                cartViewModel.clearCart()
                tokenViewModel.deleteToken()
            }

            ordersButton.setOnClickListener {
                navController.navigate(R.id.action_navigation_profile_to_orderList)
            }

            addressesButton.setOnClickListener {
                navController.navigate(R.id.action_navigation_profile_to_addressFragment)
            }

            editProfileButton.setOnClickListener {
                val fullName = currentUserInfo.name
                val nameParts = fullName.split(" ", limit = 2)
                val firstName = nameParts.getOrNull(0) ?: fullName  // Use full name as firstName if split fails
                val lastName = nameParts.getOrNull(1) ?: " "        // Use space as lastName if no last name
                val nic = currentUserInfo.nic.ifEmpty { " " }       // Use space if NIC is empty

                val bundle = Bundle().apply {
                    putString("firstName", firstName)
                    putString("lastName", lastName)
                    putString("nic", nic)
                }
                try {
                    navController.navigate(R.id.action_navigation_profile_to_editProfile, bundle)
                } catch (e: Exception) {
                    Log.e("ProfileFragment", "Navigation failed", e)
                    Toast.makeText(context, "Failed to open edit profile", Toast.LENGTH_SHORT).show()
                }
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
        binding.loading.visibility = if (isLoading) View.VISIBLE else View.GONE
        binding.actionsCard.isEnabled = !isLoading
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

