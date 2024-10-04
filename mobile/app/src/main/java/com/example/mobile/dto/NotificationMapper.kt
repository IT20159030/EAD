package com.example.mobile.dto


import com.example.mobile.data.model.Notification
import java.util.UUID

object NotificationMapper {

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