package com.example.mobile.services.api.notification;

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
