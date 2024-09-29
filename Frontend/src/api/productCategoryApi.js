import axios from "axios";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/api/v1";

export const createProductCategory = async (productCategory) => {
  const response = await axios.post(
    `${baseUrl}/productcategory`,
    productCategory
  );
  return response.data;
};

export const getAllProductCategories = async () => {
  const response = await axios.get(`${baseUrl}/productcategory`);
  return response.data;
};

export const getProductCategoryById = async (id) => {
  const response = await axios.get(`${baseUrl}/productcategory/${id}`);
  return response.data;
};

export const updateProductCategory = async (productCategory) => {
  const response = await axios.put(
    `${baseUrl}/productcategory/${productCategory.id}`,
    productCategory
  );
  return response.data;
};

export const deleteProductCategory = async (id) => {
  const response = await axios.delete(`${baseUrl}/productcategory/${id}`);
  return response.data;
};
