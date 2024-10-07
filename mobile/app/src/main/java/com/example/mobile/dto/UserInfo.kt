package com.example.mobile.dto

import com.google.gson.annotations.SerializedName
/*
* DTO for profile page calls and user update calls
 */

data class UserInfoResponse (
    var isSuccess: Boolean,
    var message: String,
    var data: UserInfo,
)

data class UserInfo(
    @SerializedName("userId")
    val id: String,
    val email: String,
    val name: String,
    val nic: String,
)

data class UserDeactivationResponse(
    var isSuccess: Boolean,
    var message: String,
)

data class UserUpdateRequest(
    val firstName: String,
    val lastName: String,
    val nic: String,
)

data class UserUpdateResponse(
    var isSuccess: Boolean,
    var message: String,
)