package com.example.mobile.data.model

/*
This class is used to represent the Notification object that is used to send the notification data to the mobile application.
Notfications are sent to the mobile application to inform the user of any new messages or updates.
*/

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