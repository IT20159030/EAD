package com.example.mobile.viewModels

/*
This class is used to represent the NotificationViewModel object that is used to handle the notification data in the mobile application.
The NotificationViewModel object contains the following attributes:
- notificationRepository: The repository used to interact with the notification data
The NotificationViewModel object contains the following methods:
- getNotifications: Get all notifications for a specific recipient
- markNotificationAsRead: Mark a notification as read
*/

import androidx.lifecycle.ViewModel
import androidx.lifecycle.liveData
import com.example.mobile.repository.NotificationRepository
import kotlinx.coroutines.Dispatchers
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class NotificationViewModel @Inject constructor(
    private val notificationRepository: NotificationRepository
) : ViewModel() {

    // Get all notifications for a specific recipient
    fun getNotifications(recipientId: String) = liveData(Dispatchers.IO) {
        val notifications = notificationRepository.getNotificationsByRecipientId(recipientId)
        val filteredNotifications = notifications.filter { it.type == "OrderStatus" && it.role == "customer" }

        emit(filteredNotifications)
    }

    // Mark a notification as read
    fun markNotificationAsRead(notificationId: String) = liveData(Dispatchers.IO) {
        val updatedNotification = notificationRepository.markAsRead(notificationId)
        emit(updatedNotification)
    }
}