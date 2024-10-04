import axios from "../utils/axiosInstanceWithAuth";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/web-auth/profile";

export const getProfile = async () => {
  const response = await axios.get(baseUrl);
  return response.data.profile;
};

export const updateProfile = async (profileData) => {
  const response = await axios.put(baseUrl, profileData);
  return response.data.profile;
};
