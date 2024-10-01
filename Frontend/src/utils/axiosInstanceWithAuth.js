import axios from "axios";

const baseUrl = import.meta.env.VITE_BACKEND_URL;
const axiosInstanceWithAuth = axios.create({
  baseURL: baseUrl,
  headers: {
    Authorization: `Bearer ${JSON.parse(localStorage.getItem("user")).token}`,
  },
});

export default axiosInstanceWithAuth;
