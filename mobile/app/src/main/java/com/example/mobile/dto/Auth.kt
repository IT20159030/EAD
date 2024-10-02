package com.example.mobile.dto
import com.google.gson.annotations.SerializedName

data class LoginRequest(
    val email: String,
    val password: String
)

data class LoginResponse(
    @SerializedName("accessToken")
    val token: String
)
data class RegisterRequest(
    val firstName: String,
    val lastName: String,
    val email: String,
    val nic: String,
    val password: String
)

data class RegisterResponse(
    @SerializedName("isSuccess")
    val isSuccess: Boolean,
    @SerializedName("message")
    val message: String
)