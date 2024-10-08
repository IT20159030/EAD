package com.example.mobile.dto

import com.google.gson.annotations.SerializedName

/*
* Data class for product.
* */

data class Product (
    @SerializedName("id") var productId: String,
    var name: String,
    @SerializedName("image") var imageUrl: String,
    var category: String,
    var categoryName: String,
    var description: String,
    var stock: Int,
    var price: Double,
    var isActive: Boolean,
    var vendorId: String,
    var vendorName: String
)

data class ProductCategory (
    @SerializedName("id") var categoryId: String,
    var name: String
)