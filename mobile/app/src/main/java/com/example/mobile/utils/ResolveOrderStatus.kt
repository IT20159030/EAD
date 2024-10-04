package com.example.mobile.utils

/*
*
* A function to resolve the order status
* from the integer value to a string value
*
* */

fun resolveOrderStatus(status: Int): String {
    return when (status) {
        0 -> "Pending"
        1 -> "Partial"
        2 -> "Approved"
        3 -> "Rejected"
        4 -> "Completed"
        5 -> "Cancel Requested"
        6 -> "Cancelled"
        else -> "Unknown"
    }
}