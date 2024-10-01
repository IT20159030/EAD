package com.example.mobile.dto

import com.google.gson.annotations.SerializedName

data class Product (
    @SerializedName("ProductId")
    var id: String,
    var name: String,
    var image: String,
    var category: String,
    var description: String,
    var price: Double,
    var isActive: Boolean,
    var vendorId: String
)