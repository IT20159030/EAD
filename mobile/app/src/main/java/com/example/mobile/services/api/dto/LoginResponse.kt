package com.example.mobile.services.api.dto

import com.google.gson.annotations.SerializedName

data class LoginResponse(
    @SerializedName("accessToken")
    val token: String
)
