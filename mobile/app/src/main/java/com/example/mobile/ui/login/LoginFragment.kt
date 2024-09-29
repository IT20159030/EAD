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
import com.example.mobile.services.api.dto.AuthRequest
import com.example.mobile.utils.ApiResponse
import com.example.mobile.viewModels.AuthViewModel
import com.example.mobile.viewModels.CoroutinesErrorHandler
import com.example.mobile.viewModels.TokenViewModel

import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class LoginFragment : Fragment() {
    private val viewModel: AuthViewModel by viewModels()
    private val tokenViewModel: TokenViewModel by activityViewModels()

    private lateinit var navController: NavController

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_login, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        navController = Navigation.findNavController(view)
        val loginTV = view.findViewById<TextView>(R.id.loginTV)

        tokenViewModel.token.observe(viewLifecycleOwner) { token ->
            if (token != null)
                navController.navigate(R.id.action_loginFragment_to_main_nav_graph)

        }

        viewModel.loginResponse.observe(viewLifecycleOwner) {
            when(it) {
                is ApiResponse.Failure -> loginTV.text = it.errorMessage
                ApiResponse.Loading -> loginTV.text = "Loading"
                is ApiResponse.Success -> {
                    tokenViewModel.saveToken(it.data.token)
                }
            }
        }

        view.findViewById<Button>(R.id.login_temp_button).setOnClickListener {
            viewModel.login(
                AuthRequest("customer@example.com", "password"),
                object: CoroutinesErrorHandler {
                    override fun onError(message: String) {
                        loginTV.text = "Error! $message"
                    }
                }
            )
        }

        view.findViewById<Button>(R.id.test_product_view_button).setOnClickListener {
            val productViewActivityIntent = Intent(context, ProductViewActivity::class.java)
            startActivity(productViewActivityIntent)
        }
    }

//
//import androidx.lifecycle.Observer
//import androidx.lifecycle.ViewModelProvider
//import androidx.annotation.StringRes
//import androidx.fragment.app.Fragment
//import android.os.Bundle
//import android.text.Editable
//import android.text.TextWatcher
//import android.view.LayoutInflater
//import android.view.View
//import android.view.ViewGroup
//import android.view.inputmethod.EditorInfo
//import android.widget.Toast
//import androidx.fragment.app.activityViewModels
//import androidx.fragment.app.viewModels
//import androidx.navigation.NavController
//import com.example.mobile.databinding.FragmentLoginBinding
//
//import com.example.mobile.R
//import com.example.mobile.ui.login.LoggedInUserView
//import com.example.mobile.ui.login.LoginViewModel
//import com.example.mobile.ui.login.LoginViewModelFactory
//import com.example.mobile.viewModels.AuthViewModel
//import com.example.mobile.viewModels.TokenViewModel
//import dagger.hilt.android.AndroidEntryPoint
//
//@AndroidEntryPoint
//class LoginFragment : Fragment() {
//
//    private val viewModel: AuthViewModel by viewModels()
//    private val tokenViewModel: TokenViewModel by activityViewModels()
//
//    private lateinit var navController: NavController
//
////    private lateinit var loginViewModel: LoginViewModel
//    private var _binding: FragmentLoginBinding? = null
//
//    // This property is only valid between onCreateView and
//    // onDestroyView.
//
//    private val binding get() = _binding!!
//
//    override fun onCreateView(
//        inflater: LayoutInflater,
//        container: ViewGroup?,
//        savedInstanceState: Bundle?
//    ): View? {
//
//        tokenViewModel.token.observe(viewLifecycleOwner) { token ->
//            if (token != null)
//                navController.navigate(R.id.action_loginFragment_to_main_nav_graph)
//        }
//
//        _binding = FragmentLoginBinding.inflate(inflater, container, false)
//        return binding.root
//    }
//
////    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
////        super.onViewCreated(view, savedInstanceState)
////        loginViewModel = ViewModelProvider(this, LoginViewModelFactory())
////            .get(LoginViewModel::class.java)
////
////        val usernameEditText = binding.email
////        val passwordEditText = binding.password
////        val loginButton = binding.login
////        val loadingProgressBar = binding.loading
////
////        loginViewModel.loginFormState.observe(viewLifecycleOwner,
////            Observer { loginFormState ->
////                if (loginFormState == null) {
////                    return@Observer
////                }
////                loginButton.isEnabled = loginFormState.isDataValid
////                loginFormState.usernameError?.let {
////                    usernameEditText.error = getString(it)
////                }
////                loginFormState.passwordError?.let {
////                    passwordEditText.error = getString(it)
////                }
////            })
////
////        loginViewModel.loginResult.observe(viewLifecycleOwner,
////            Observer { loginResult ->
////                loginResult ?: return@Observer
////                loadingProgressBar.visibility = View.GONE
////                loginResult.error?.let {
////                    showLoginFailed(it)
////                }
////                loginResult.success?.let {
////                    updateUiWithUser(it)
////                }
////            })
////
////        val afterTextChangedListener = object : TextWatcher {
////            override fun beforeTextChanged(s: CharSequence, start: Int, count: Int, after: Int) {
////                // ignore
////            }
////
////            override fun onTextChanged(s: CharSequence, start: Int, before: Int, count: Int) {
////                // ignore
////            }
////
////            override fun afterTextChanged(s: Editable) {
////                loginViewModel.loginDataChanged(
////                    usernameEditText.text.toString(),
////                    passwordEditText.text.toString()
////                )
////            }
////        }
////        usernameEditText.addTextChangedListener(afterTextChangedListener)
////        passwordEditText.addTextChangedListener(afterTextChangedListener)
////        passwordEditText.setOnEditorActionListener { _, actionId, _ ->
////            if (actionId == EditorInfo.IME_ACTION_DONE) {
////                loginViewModel.login(
////                    usernameEditText.text.toString(),
////                    passwordEditText.text.toString()
////                )
////            }
////            false
////        }
////
////        loginButton.setOnClickListener {
////            loadingProgressBar.visibility = View.VISIBLE
////            loginViewModel.login(
////                usernameEditText.text.toString(),
////                passwordEditText.text.toString()
////            )
////        }
////    }
////
////    private fun updateUiWithUser(model: LoggedInUserView) {
////        val welcome = getString(R.string.welcome) + model.displayName
////        // TODO : initiate successful logged in experience
////        val appContext = context?.applicationContext ?: return
////        Toast.makeText(appContext, welcome, Toast.LENGTH_LONG).show()
////    }
////
////    private fun showLoginFailed(@StringRes errorString: Int) {
////        val appContext = context?.applicationContext ?: return
////        Toast.makeText(appContext, errorString, Toast.LENGTH_LONG).show()
////    }
////
////    override fun onDestroyView() {
////        super.onDestroyView()
////        _binding = null
////    }
}