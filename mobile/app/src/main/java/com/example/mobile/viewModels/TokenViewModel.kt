package com.example.mobile.viewModels

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.mobile.utils.TokenManager
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject

@HiltViewModel
class TokenViewModel @Inject constructor(
    private val tokenManager: TokenManager
) : ViewModel() {
    val token = MutableLiveData<String?>()
    val userId: LiveData<String?> get() = tokenManager.userId

    init {
        viewModelScope.launch(Dispatchers.IO) {
            tokenManager.getToken().collect { fetchedToken ->
                withContext(Dispatchers.Main) {
                    token.value = fetchedToken
                }
            }
        }
    }

    fun saveToken(token: String) {
        viewModelScope.launch(Dispatchers.IO) {
            tokenManager.saveToken(token)
        }
    }

    fun deleteToken() {
        viewModelScope.launch(Dispatchers.IO) {
            tokenManager.deleteToken()
        }
    }
}
