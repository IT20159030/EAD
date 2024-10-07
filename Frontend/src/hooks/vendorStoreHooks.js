import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getVendor, updateVendor } from "../api/vendorStoreApi";

export const useGetVendorStore = () => {
  return useQuery({
    queryKey: ["vendor-info"],
    queryFn: getVendor,
  });
};

export const useUpdateVendorStore = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateVendor,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vendor-info"] });
    },
  });
};
