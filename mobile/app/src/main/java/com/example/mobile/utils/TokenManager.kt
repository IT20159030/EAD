package com.example.mobile.utils

import android.content.Context
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import com.example.mobile.di.dataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map

class TokenManager (private val context: Context){
    companion object{
        private val TOKEN_KEY = stringPreferencesKey("user_jwt_token")
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

}
