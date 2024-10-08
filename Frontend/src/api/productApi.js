import axios from "../utils/axiosInstanceWithAuth";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/product";

/*
 * Product API calls
 */

export const createProduct = async (productData) => {
  const response = await axios.post(baseUrl, productData);
  return response.data;
};

export const getAllProducts = async () => {
  const response = await axios.get(baseUrl);
  return response.data;
};

export const getProductById = async (id) => {
  const response = await axios.get(`${baseUrl}/${id}`);
  return response.data;
};

export const getActiveProducts = async () => {
  const response = await axios.get(`${baseUrl}/active`);
  return response.data;
};

export const getProductsByVendor = async (vendorId) => {
  const response = await axios.get(`${baseUrl}/vendor/${vendorId}`);
  return response.data;
};

export const getActiveProductsByVendor = async (vendorId) => {
  const response = await axios.get(`${baseUrl}/vendor/${vendorId}/active`);
  return response.data;
};

export const getInactiveProductsByVendor = async (vendorId) => {
  const response = await axios.get(`${baseUrl}/vendor/${vendorId}/inactive`);
  return response.data;
};

export const getActiveProductsByCategory = async (categoryId) => {
  const response = await axios.get(`${baseUrl}/category/${categoryId}/active`);
  return response.data;
};

export const searchProducts = async (query) => {
  const response = await axios.get(`${baseUrl}/search`, { params: { query } });
  return response.data;
};

export const updateProduct = async (productData) => {
  const response = await axios.put(`${baseUrl}/${productData.id}`, productData);
  return response.data;
};

export const deleteProduct = async (id) => {
  const response = await axios.delete(`${baseUrl}/${id}`);
  return response.data;
};

export const activateProduct = async (id) => {
  const response = await axios.put(`${baseUrl}/${id}/activate`);
  return response.data;
};

export const deactivateProduct = async (id) => {
  const response = await axios.put(`${baseUrl}/${id}/deactivate`);
  return response.data;
};
