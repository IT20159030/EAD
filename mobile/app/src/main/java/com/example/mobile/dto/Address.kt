package com.example.mobile.dto

/*
* AddressResponse is a data class that represents the response from the server
* */

data class AddressResponse(
    val isSuccess: Boolean,
    val message: String,
    val data: Address
)

data class Address(
    var line1: String,
    var line2: String,
    var city: String,
    var postalCode: String
)