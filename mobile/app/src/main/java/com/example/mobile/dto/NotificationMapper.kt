package com.example.mobile.dto

/*
This class is used to represent the NotificationDto object that is used to send the notification data to the mobile application.
Notfications are sent to the mobile application to inform the user of any new messages or updates.
*/

import com.example.mobile.data.model.Notification
import java.util.UUID

object NotificationMapper {

    // Convert a NotificationDto object to a Notification object
    fun fromDto(notificationDto: NotificationDto): Notification {
        return Notification(
            id =  notificationDto.id,
            recipientId = notificationDto.recipientId,
            message = notificationDto.message,
            messageID = notificationDto.messageID,
            role = notificationDto.role,
            createdAt = notificationDto.createdAt,
            type = notificationDto.type,
            isRead = notificationDto.isRead
        )
    }

    // Convert a Notification object to a NotificationDto object
    fun toDto(notification: Notification): NotificationDto {
        return NotificationDto(
            id = notification.id,
            recipientId = notification.recipientId,
            message = notification.message,
            messageID = notification.messageID,
            role = notification.role,
            createdAt = notification.createdAt,
            type = notification.type,
            isRead = notification.isRead
        )
    }
}