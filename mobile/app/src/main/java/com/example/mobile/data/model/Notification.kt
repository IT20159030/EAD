package com.example.mobile.data.model

import java.util.UUID

data class Notification(
    val id: String,
    val recipientId: String,
    val message: String,
    val messageID: String,
    val role: String,
    val createdAt: String,
    val type: String,
    val isRead: Boolean
)