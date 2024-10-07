import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createStaff,
  getAllStaff,
  getStaffById,
  updateStaff,
  deleteStaff,
  updateStaffStatus,
} from "../api/staffManagementApi";

/*
 * This file contains hooks for staff management.
 * These hooks are used to fetch, create, update, delete staff accounts and update staff status.
 */

export const useCreateStaffAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createStaff,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["staff"] });
    },
  });
};

export const useGetAllStaffAccounts = () => {
  return useQuery({
    queryKey: ["staff"],
    queryFn: getAllStaff,
  });
};

export const useGetStaffAccountById = (id) => {
  return useQuery({
    queryKey: ["staff", id],
    queryFn: () => getStaffById(id),
  });
};

export const useUpdateStaffAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateStaff,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["staff"] });
    },
  });
};

export const useDeleteStaffAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteStaff,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["staff"] });
    },
  });
};

export const useUpdateStaffStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateStaffStatus,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["staff"] });
    },
  });
};
