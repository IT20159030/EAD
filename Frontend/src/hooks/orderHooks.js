/**
 * @fileoverview orderHooks.js is a file that contains custom hooks to interact with the order API.
 */

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import * as order from '../api/orderApi';

export const useGetAllOrders = () => {
  return useQuery({
    queryKey: ['orders'],
    queryFn: order.getAllOrders,
  });
};

export const useGetOrderById = (id) => {
  return useQuery({
    queryKey: ['order', id],
    queryFn: () => order.getOrderById(id),
  });
};

export const useGetOrderByVendorId = (id) => {
  return useQuery({
    queryKey: ['orders', id],
    queryFn: () => order.getOrderByVendorId(id),
  });
};

export const useUpdateOrder = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: order.updateOrder,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });
};

export const useDeleteOrder = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: order.deleteOrder,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });
};

export const useMarkOrderReady = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: order.markOrderReady,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });
};

export const useMarkOrderDelivered = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: order.markOrderDelivered,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });
};

export const useMarkOrderRejected = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: order.markOrderRejected,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });
};

export const useMarkOrderCancelled = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: order.markOrderCancelled,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });
};

export const useGetOrderCancellationDetails = (id) => {
  return useQuery({
    queryKey: ['cancellationRequests'],
    queryFn: () => order.getOrderCancellationDetails(id),
  });
};
