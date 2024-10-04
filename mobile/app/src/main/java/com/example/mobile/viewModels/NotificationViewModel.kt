package com.example.mobile.viewModels

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

    fun getNotifications(recipientId: String) = liveData(Dispatchers.IO) {
        val notifications = notificationRepository.getNotificationsByRecipientId(recipientId)
        val filteredNotifications = notifications.filter { it.type == "OrderStatus" && it.role == "customer" }

        emit(filteredNotifications)
    }

    fun markNotificationAsRead(notificationId: String) = liveData(Dispatchers.IO) {
        val updatedNotification = notificationRepository.markAsRead(notificationId)
        emit(updatedNotification)
    }
}