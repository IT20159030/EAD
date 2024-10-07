import axios from "axios";

const baseUrl = import.meta.env.VITE_BACKEND_URL + "/web-auth/login";

/*
 * Login API calls
 */

export const login = async (loginData) => {
  const response = await axios.post(baseUrl, loginData);
  return response.data;
};
