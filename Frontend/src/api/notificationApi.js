import axios from "../utils/axiosInstanceWithAuth";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/notification";

/*
 * Notification API calls
 */

export const getAllNotifications = async () => {
  const response = await axios.get(baseUrl);
  return response.data;
};

export const getNotificationByRecipientId = async (recipientId) => {
  const response = await axios.get(`${baseUrl}/recipient/${recipientId}`);
  return response.data;
};

export const getNotificationById = async (id) => {
  const response = await axios.get(`${baseUrl}/${id}`);
  return response.data;
};

export const createNotification = async (notification) => {
  const response = await axios.post(baseUrl, notification);
  return response.data;
};

export const deleteNotification = async (id) => {
  const response = await axios.delete(`${baseUrl}/${id}`);
  return response.data;
};

export const updateNotification = async (notification) => {
  const response = await axios.put(
    `${baseUrl}/${notification.id}`,
    notification
  );
  return response.data;
};

export const markAsRead = async (id) => {
  const response = await axios.put(`${baseUrl}/${id}/read`);
  return response.data;
};
