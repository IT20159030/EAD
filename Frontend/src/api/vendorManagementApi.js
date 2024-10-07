import axios from "../utils/axiosInstanceWithAuth";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/vendor-management";

/*
 * Vendor management API calls
 */
export const createVendor = async (vendorData) => {
  const response = await axios.post(baseUrl, vendorData);
  return response.data;
};

export const getAllVendor = async () => {
  const response = await axios.get(baseUrl);
  return response.data.vendors;
};

export const getVendorById = async (id) => {
  const response = await axios.get(`${baseUrl}/${id}`);
  return response.data;
};

export const updateVendor = async (vendorData) => {
  const response = await axios.put(`${baseUrl}/${vendorData.id}`, vendorData);
  return response.data;
};

export const deleteVendor = async (id) => {
  const response = await axios.delete(`${baseUrl}/${id}`);
  return response.data;
};

export const updateVendorStatus = async (vendorData) => {
  const response = await axios.put(
    `${baseUrl}/${vendorData.id}/account-status`,
    {
      status: vendorData.status,
    }
  );
  return response.data;
};
