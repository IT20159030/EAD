import axios from "../utils/axiosInstanceWithAuth";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/customer-management";

export const createCustomer = async (customerData) => {
  const response = await axios.post(baseUrl, customerData);
  return response.data;
};

export const getAllCustomer = async () => {
  const response = await axios.get(baseUrl);
  return response.data.customers;
};

export const getCustomerById = async (id) => {
  const response = await axios.get(`${baseUrl}/${id}`);
  return response.data.customer;
};

export const updateCustomer = async (customerData) => {
  const response = await axios.put(
    `${baseUrl}/${customerData.id}`,
    customerData
  );
  return response.data;
};

export const deleteCustomer = async (id) => {
  const response = await axios.delete(`${baseUrl}/${id}`);
  return response.data;
};

export const updateCustomerStatus = async (customerData) => {
  const response = await axios.put(
    `${baseUrl}/${customerData.id}/account-status`,
    {
      status: customerData.status,
    }
  );
  return response.data;
};
