import axios from "../utils/axiosInstanceWithAuth";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/productcategory";

export const createProductCategory = async (productCategory) => {
  const response = await axios.post(baseUrl, productCategory);
  return response.data;
};

export const getAllProductCategories = async () => {
  const response = await axios.get(baseUrl);
  return response.data;
};

export const getProductCategoryById = async (id) => {
  const response = await axios.get(`${baseUrl}/${id}`);
  return response.data;
};

export const updateProductCategory = async (productCategory) => {
  const response = await axios.put(
    `${baseUrl}/${productCategory.id}`,
    productCategory
  );
  return response.data;
};

export const deleteProductCategory = async (id) => {
  const response = await axios.delete(`${baseUrl}/${id}`);
  return response.data;
};
