package com.example.mobile.dto

import com.google.gson.annotations.SerializedName

data class Product (
    @SerializedName("id") var productId: String,
    var name: String,
    @SerializedName("image") var imageUrl: String,
    var category: String,
    var description: String,
    var price: Double,
    var isActive: Boolean,
    var vendorId: String
)