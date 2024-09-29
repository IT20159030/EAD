package com.example.mobile.services.api.dto


data class UserInfoResponse (
    var isSuccess: Boolean,
    var message: String,
    var data: UserInfo,
)