package com.example.mobile.repository

/*
This class is used to represent the NotificationRepository object that is used to handle the notification data in the mobile application.
The NotificationRepository object contains the following attributes:
- notificationApiService: The service used to interact with the notification API
The NotificationRepository object contains the following methods:
- getNotificationsByRecipientId: Get all notifications for a specific recipient
- markAsRead: Mark a notification as read
*/

import com.example.mobile.dto.NotificationMapper
import com.example.mobile.data.model.Notification
import com.example.mobile.services.api.notification.NotificationApiService
import javax.inject.Inject

class NotificationRepository @Inject constructor(
    private val notificationApiService: NotificationApiService
) {
    suspend fun getNotificationsByRecipientId(recipientId: String): List<Notification> {
        val notificationDtos = notificationApiService.getNotificationsByRecipientId(recipientId)
        return notificationDtos.map { NotificationMapper.fromDto(it) }
    }

    suspend fun markAsRead(notificationId: String): Notification {
        val notificationDto = notificationApiService.markAsRead(notificationId)
        return NotificationMapper.fromDto(notificationDto)
    }
}