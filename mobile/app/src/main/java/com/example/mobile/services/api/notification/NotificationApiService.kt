package com.example.mobile.services.api.notification;

/*
NotificationApiService is an interface used to define the API endpoints for the Notification service.
The Notification service is used to send notifications to the mobile application to inform the user of any new messages or updates.
The Notification service contains the following endpoints:
- getNotificationsByRecipientId: Get all notifications for a specific recipient
- markAsRead: Mark a notification as read
*/

import com.example.mobile.dto.NotificationDto

import retrofit2.http.GET
import retrofit2.http.PUT
import retrofit2.http.Path

interface NotificationApiService {
        // Get all notifications for a specific recipient
        @GET("notification/recipient/{recipientId}")
        suspend fun getNotificationsByRecipientId(
                @Path("recipientId") recipientId: String
        ): List<NotificationDto>

        // Mark a notification as read by setting the isRead flag to true
        @PUT("notification/{id}/read")
        suspend fun markAsRead(
                @Path("id") id: String
        ): NotificationDto
}
