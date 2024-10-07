import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createVendor,
  getAllVendor,
  getVendorById,
  updateVendor,
  deleteVendor,
  updateVendorStatus,
} from "../api/vendorManagementApi";

/*
 * This file contains hooks for vendor management.
 * These hooks are used to fetch, create, update, delete vendor accounts and update vendor status.
 */

export const useCreateVendorAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createVendor,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vendor"] });
    },
  });
};

export const useGetAllVendorAccounts = () => {
  return useQuery({
    queryKey: ["vendor"],
    queryFn: getAllVendor,
  });
};

export const useGetVendorAccountById = (id) => {
  return useQuery({
    queryKey: ["vendor", id],
    queryFn: () => getVendorById(id),
  });
};

export const useUpdateVendorAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateVendor,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vendor"] });
    },
  });
};

export const useDeleteVendorAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteVendor,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vendor"] });
    },
  });
};

export const useUpdateVendorStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateVendorStatus,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vendor"] });
    },
  });
};
