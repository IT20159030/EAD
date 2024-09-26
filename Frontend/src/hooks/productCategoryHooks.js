import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createProductCategory,
  deleteProductCategory,
  getAllProductCategories,
  getProductCategoryById,
  updateProductCategory,
} from "../api/productCategoryApi";

export const useCreateProductCategory = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createProductCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["productCategories"] });
    },
  });
};

export const useGetAllProductCategories = () => {
  return useQuery({
    queryKey: ["productCategories"],
    queryFn: getAllProductCategories,
  });
};

export const useGetProductCategoryById = (id) => {
  return useQuery({
    queryKey: ["productCategory", id],
    queryFn: () => getProductCategoryById(id),
  });
};

export const useUpdateProductCategory = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateProductCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["productCategories"] });
    },
  });
};

export const useDeleteProductCategory = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteProductCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["productCategories"] });
    },
  });
};
