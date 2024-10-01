package com.example.mobile.dto

/*
* DTO for user response
* */

data class UserResponse (
    val isSuccess: Boolean,
    val message: String,
    val data: User
)

data class User (
    val userId: String,
    val name: String,
    val email: String
)