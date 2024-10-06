package com.example.mobile.services.api.notification;

/*
NotificationApiService is an interface used to define the API endpoints for the Notification service.
The Notification service is used to send notifications to the mobile application to inform the user of any new messages or updates.
*/

import com.example.mobile.dto.NotificationDto

import retrofit2.http.GET
import retrofit2.http.PUT
import retrofit2.http.Path

interface NotificationApiService {
        @GET("notification/recipient/{recipientId}")
        suspend fun getNotificationsByRecipientId(
                @Path("recipientId") recipientId: String
        ): List<NotificationDto>

        @PUT("notification/{id}/read")
        suspend fun markAsRead(
                @Path("id") id: String
        ): NotificationDto
}
