package com.example.mobile.dto

/*
This class is used to represent the NotificationDto object that is used to send the notification data to the mobile application.
Notfications are sent to the mobile application to inform the user of any new messages or updates.
*/

data class NotificationDto(
    val id: String,
    val recipientId: String,
    val message: String,
    val messageID: String,
    val role: String,
    val createdAt: String,
    val type: String,
    val isRead: Boolean
)