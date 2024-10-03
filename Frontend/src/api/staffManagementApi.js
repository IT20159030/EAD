import axios from "../utils/axiosInstanceWithAuth";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/staff-management";

export const createStaff = async (staffData) => {
  const response = await axios.post(baseUrl, staffData);
  return response.data;
};

export const getAllStaff = async () => {
  const response = await axios.get(baseUrl);
  return response.data.staff;
};

export const getStaffById = async (id) => {
  const response = await axios.get(`${baseUrl}/${id}`);
  return response.data;
};

export const updateStaff = async (staffData) => {
  const response = await axios.put(`${baseUrl}/${staffData.id}`, staffData);
  return response.data;
};

export const deleteStaff = async (id) => {
  const response = await axios.delete(`${baseUrl}/${id}`);
  return response.data;
};

export const updateStaffStatus = async (staffData) => {
  const response = await axios.put(
    `${baseUrl}/${staffData.id}/account-status`,
    {
      status: staffData.status,
    }
  );
  return response.data;
};
