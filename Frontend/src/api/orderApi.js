/**
 * @fileoverview orderApi.js is a file that contains functions to interact with the order API.
 */

import axios from '../utils/axiosInstanceWithAuth';

const baseUrl = import.meta.env.VITE_BACKEND_URL;

export const getAllOrders = async () => {
  const response = await axios.get(`${baseUrl}/Order`);
  return response.data;
};

export const getOrderById = async (id) => {
  const response = await axios.get(`${baseUrl}/Order/${id}`);
  return response.data;
};

export const getOrderByVendorId = async (id) => {
  const response = await axios.get(`${baseUrl}/Order/GetByVendorId/${id}`);
  return response.data;
};

export const updateOrder = async (order) => {
  const response = await axios.put(`${baseUrl}/Order/${order.id}`, order);
  return response.data;
};

export const deleteOrder = async (id) => {
  const response = await axios.delete(`${baseUrl}/Order/${id}`);
  return response.data;
};

export const markOrderReady = async (id) => {
  const response = await axios.put(`${baseUrl}/Order/ready/${id}`);
  return response.data;
};

export const markOrderDelivered = async (id) => {
  const response = await axios.put(`${baseUrl}/Order/delivered/${id}`);
  return response.data;
};

export const markOrderRejected = async (id) => {
  const response = await axios.put(`${baseUrl}/Order/reject/${id}`);
  return response.data;
};

export const markOrderCancelled = async (id) => {
  const response = await axios.put(`${baseUrl}/Order/cancel/${id}`);
  return response.data;
};

export const getOrderCancellationDetails = async (id) => {
  const response = await axios.get(
    `${baseUrl}/cancellation-request/order/${id}`
  );
  return response.data;
};

export const updateOrderCancellationDetails = async (
  orderCancellationDetails
) => {
  const response = await axios.put(
    `${baseUrl}/cancellation-request/${orderCancellationDetails.id}`,
    {
      processedBy: orderCancellationDetails.processedBy,
      status: orderCancellationDetails.status,
      decisionNote: orderCancellationDetails.decisionNote,
    },
    {
      headers: {
        'Content-Type': 'application/json',
      },
    }
  );
  return response.data;
};
