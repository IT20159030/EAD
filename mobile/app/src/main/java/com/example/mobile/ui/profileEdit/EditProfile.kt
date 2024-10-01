package com.example.mobile.ui.profileEdit

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.navigation.NavController
import androidx.navigation.Navigation
import com.example.mobile.R
import com.example.mobile.databinding.FragmentEditProfileBinding
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.CoroutinesErrorHandler
import com.example.mobile.viewModels.TokenViewModel
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class EditProfile : Fragment() {

    private var _binding: FragmentEditProfileBinding? = null
    private val binding get() = _binding!!

    private val viewModel: EditProfileViewModel by viewModels()
    private val tokenViewModel: TokenViewModel by activityViewModels()

    private lateinit var navController: NavController

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentEditProfileBinding.inflate(inflater, container, false)
        (activity as AppCompatActivity).supportActionBar?.title = "Edit Profile"
        (activity as AppCompatActivity).actionBar?.setDisplayShowHomeEnabled(true)
        (activity as AppCompatActivity).supportActionBar?.setDisplayShowHomeEnabled(true)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        navController = Navigation.findNavController(view)

        setupClickListeners()

        viewModel.deactivationResponse.observe(viewLifecycleOwner) { response ->
            when (response) {
                is ApiResponse.Loading ->
                    showLoading(true)

                is ApiResponse.Success -> {
                    tokenViewModel.deleteToken()
                    navController.navigate(R.id.action_editProfile_to_navigation_profile3)
                    showLoading(false)
                }

                is ApiResponse.Failure -> {
                    showLoading(false)
                    showError(response.errorMessage.ifEmpty { "Something went wrong" })
                }
            }
        }
    }

    private fun setupClickListeners() {
        binding.saveButton.setOnClickListener {
            // Logic to save profile changes

            // After saving, navigate back to profile
            navController.navigate(R.id.action_editProfile_to_navigation_profile3, null)
        }

        binding.cancelButton.setOnClickListener {
            // Navigate back to profile without saving
            navController.navigate(R.id.action_editProfile_to_navigation_profile3)
        }

        binding.deactivate.setOnClickListener {
            deactivateAccount()
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }

    private fun deactivateAccount() {
        viewModel.deactivateAccount(object : CoroutinesErrorHandler {
            override fun onError(message: String) {
                showLoading(false)
                showError(message.ifEmpty { "Something went wrong" })
            }
        })
    }

    private fun showLoading(isLoading: Boolean) {
        binding.loadingSpinner.visibility = if (isLoading) View.VISIBLE else View.GONE
        binding.saveButton.isEnabled = !isLoading
        binding.cancelButton.isEnabled = !isLoading
        binding.deactivate.isEnabled = !isLoading
    }

    private fun showError(message: String) {
        binding.errorTV.text = message
        binding.errorTV.visibility = View.VISIBLE
    }


}