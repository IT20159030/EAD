package com.example.mobile.repository

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