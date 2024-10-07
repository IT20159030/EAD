import axios from "../utils/axiosInstanceWithAuth";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/web-auth/vendor-info";

export const getVendor = async () => {
  const response = await axios.get(baseUrl);
  return response.data.vendorDetails;
};

export const updateVendor = async (vendorData) => {
  const response = await axios.put(baseUrl, vendorData);
  return response.data.vendor;
};
