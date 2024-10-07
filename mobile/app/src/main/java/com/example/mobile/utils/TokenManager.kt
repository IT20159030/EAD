package com.example.mobile.utils

import android.content.Context
import android.util.Base64
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import com.example.mobile.di.dataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.launch
import org.json.JSONObject

/*
* A class that manages the user's token. 
*  - It saves the token to the data store.
*  - It retrieves the token from the data store.
*  - It deletes the token from the data store.
* */

class TokenManager(private val context: Context) {
    companion object {
        private val TOKEN_KEY = stringPreferencesKey("user_jwt_token")
    }

    private val _userId = MutableLiveData<String?>()
    val userId: LiveData<String?> get() = _userId

    init {
        GlobalScope.launch {
            getToken().collect { token ->
                token?.let {
                    val userId = getUserIdFromToken(it)
                    _userId.postValue(userId)
                }
            }
        }
    }

    fun getToken(): Flow<String?> {
        return context.dataStore.data.map { pref ->
            pref[TOKEN_KEY]
        }
    }

    suspend fun saveToken(token: String) {
        context.dataStore.edit { pref ->
            pref[TOKEN_KEY] = token
        }
    }

    suspend fun deleteToken() {
        context.dataStore.edit { pref ->
            pref.remove(TOKEN_KEY)
        }
    }

    private fun getUserIdFromToken(token: String): String? {
        val parts = token.split(".")
        if (parts.size != 3) {
            return null
        }

        val payload = parts[1]
        val decodedBytes = Base64.decode(payload, Base64.URL_SAFE or Base64.NO_WRAP)
        val decodedString = String(decodedBytes)

        val json = JSONObject(decodedString)
        val userId = json.optString("sub")

        return userId
    }
}
