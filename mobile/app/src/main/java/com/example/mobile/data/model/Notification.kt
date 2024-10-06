package com.example.mobile.data.model

/*
This class is used to represent the Notification object that is used to send the notification data to the mobile application.
Notfications are sent to the mobile application to inform the user of any new messages or updates.
The Notification object contains the following attributes:
- id: A unique identifier for the notification
- recipientId: The id of the user who will receive the notification
- message: The message that will be displayed in the notification
- messageID: The id of the message that the notification is related to
- role: The role of the user who will receive the notification
- createdAt: The date and time when the notification was created
- type: The type of the notification
- isRead: A flag to indicate whether the notification has been read or not
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