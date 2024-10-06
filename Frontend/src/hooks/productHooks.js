import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  createProduct,
  getAllProducts,
  getProductById,
  getActiveProducts,
  getProductsByVendor,
  getActiveProductsByVendor,
  getInactiveProductsByVendor,
  getActiveProductsByCategory,
  searchProducts,
  updateProduct,
  deleteProduct,
  activateProduct,
  deactivateProduct,
} from "../api/productApi";

export const useCreateProduct = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createProduct,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      queryClient.invalidateQueries({ queryKey: ["productsByVendor"] });
    },
  });
};

export const useGetAllProducts = () => {
  return useQuery({
    queryKey: ["products"],
    queryFn: getAllProducts,
  });
};

export const useGetProductById = (id) => {
  return useQuery({
    queryKey: ["product", id],
    queryFn: () => getProductById(id),
  });
};

export const useGetActiveProducts = () => {
  return useQuery({
    queryKey: ["activeProducts"],
    queryFn: getActiveProducts,
  });
};

export const useGetProductsByVendor = (vendorId) => {
  return useQuery({
    queryKey: ["productsByVendor", vendorId],
    queryFn: () => getProductsByVendor(vendorId),
  });
};

export const useGetActiveProductsByVendor = (vendorId) => {
  return useQuery({
    queryKey: ["activeProductsByVendor", vendorId],
    queryFn: () => getActiveProductsByVendor(vendorId),
  });
};

export const useGetInactiveProductsByVendor = (vendorId) => {
  return useQuery({
    queryKey: ["inactiveProductsByVendor", vendorId],
    queryFn: () => getInactiveProductsByVendor(vendorId),
  });
};

export const useGetActiveProductsByCategory = (categoryId) => {
  return useQuery({
    queryKey: ["activeProductsByCategory", categoryId],
    queryFn: () => getActiveProductsByCategory(categoryId),
  });
};

export const useSearchProducts = () => {
  return useQuery({
    queryKey: ["searchProducts"],
    queryFn: searchProducts,
  });
};

export const useUpdateProduct = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateProduct,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      queryClient.invalidateQueries({ queryKey: ["productsByVendor"] });
    },
  });
};

export const useDeleteProduct = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteProduct,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      queryClient.invalidateQueries({ queryKey: ["productsByVendor"] });
    },
  });
};

export const useActivateProduct = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: activateProduct,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      queryClient.invalidateQueries({ queryKey: ["productsByVendor"] });
    },
  });
};

export const useDeactivateProduct = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deactivateProduct,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      queryClient.invalidateQueries({ queryKey: ["productsByVendor"] });
    },
  });
};
