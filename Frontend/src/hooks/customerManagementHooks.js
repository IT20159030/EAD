import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createCustomer,
  getAllCustomer,
  getCustomerById,
  updateCustomer,
  deleteCustomer,
  updateCustomerStatus,
} from "../api/customerManagementApi";

export const useCreateCustomerAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createCustomer,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["customer"] });
    },
  });
};

export const useGetAllCustomerAccounts = () => {
  return useQuery({
    queryKey: ["customer"],
    queryFn: getAllCustomer,
  });
};

export const useGetCustomerDetails = (id) => {
  return useQuery({
    queryKey: ["customer", id],
    queryFn: () => getCustomerById(id),
  });
};

export const useGetCustomerAccountById = (id) => {
  return useQuery({
    queryKey: ["customer", id],
    queryFn: () => getCustomerById(id),
  });
};

export const useUpdateCustomerAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateCustomer,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["customer"] });
    },
  });
};

export const useDeleteCustomerAccount = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteCustomer,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["customer"] });
    },
  });
};

export const useUpdateCustomerStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateCustomerStatus,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["customer"] });
    },
  });
};
