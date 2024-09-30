package com.example.mobile.dto

import com.google.gson.annotations.SerializedName


data class UserInfoResponse (
    var isSuccess: Boolean,
    var message: String,
    var data: UserInfo,
)

data class UserInfo(
    @SerializedName("userId")
    val id: String,
    val email: String,
    val name: String
)