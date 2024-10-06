package com.example.mobile.dto

import com.google.gson.annotations.SerializedName

/*
* Data class for vendor request calls
* */

data class Vendor(
    @SerializedName("id") var vendorId: String,
    var vendorName: String = null.toString(),
    var vendorEmail: String = null.toString(),
    var vendorPhone: String = null.toString(),
    var vendorAddress: String = null.toString(),
    var vendorCity: String = null.toString(),
    var vendorRating: Double,
    var vendorRatingCount: Int,
    var reviews: List<Review>
)

data class Review(
    @SerializedName("id") var reviewId: String,
    var productId: String,
    var reviewerId: String,
    var reviewerName: String,
    @SerializedName("reviewRating") var rating: Int,
    @SerializedName("reviewText") var comment: String
)

data class AddReview(
    var productId: String,
    var vendorId: String,
    @SerializedName("reviewRating") var rating: Int,
    @SerializedName("reviewText") var comment: String
)

data class UpdateReview(
    @SerializedName("id") var reviewId: String,
    var vendorId: String,
    var productId: String,
    val reviewerId: String,
    val reviewerName: String,
    @SerializedName("reviewRating") var rating: Int,
    @SerializedName("reviewText") var comment: String
)