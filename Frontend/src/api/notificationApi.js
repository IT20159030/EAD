import axios from "axios";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/api/v1";

const getToken =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkNjk0OWE4Ny1kZTJjLTQwYWMtYWM3NC1lZjYzNDEyYTMxZWEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJhZG1pbkBleGFtcGxlLmNvbSIsImp0aSI6ImU3ZDNmMmY0LTBmN2YtNDkzMS1hMmNjLWNhYjk3MmM3N2YwNSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiZDY5NDlhODctZGUyYy00MGFjLWFjNzQtZWY2MzQxMmEzMWVhIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJleHAiOjE3MzAyMTE3OTMsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTE3MyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTE3MyJ9.c896a5VzYULbKUV1hCa9QcLhInCuEXJlZK6LzMfRJv0";

const axiosInstance = axios.create({
  baseURL: baseUrl,
});

axiosInstance.interceptors.request.use((config) => {
  const token = getToken;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const getAllNotifications = async () => {
  const response = await axiosInstance.get("/notification");
  return response.data;
};

export const createNotification = async (notification) => {
  const response = await axiosInstance.post("/notification", notification);
  return response.data;
};

export const deleteNotification = async (id) => {
  const response = await axiosInstance.delete(`/notification/${id}`);
  return response.data;
};

export const updateNotification = async (notification) => {
  const response = await axiosInstance.put(
    `/notification/${notification.id}`,
    notification
  );
  return response.data;
};

export const markAsRead = async (id) => {
  const response = await axiosInstance.put(`/notification/${id}/read`);
  return response.data;
};
