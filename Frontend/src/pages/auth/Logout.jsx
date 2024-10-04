import { useNavigate } from "react-router-dom";
import { useAuth } from "../../provider/authProvider";
import { useEffect } from "react";
import queryClient from "../../utils/queryClient";
import axiosInstanceWithAuth from "../../utils/axiosInstanceWithAuth";

const Logout = () => {
  const { setUser } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const handleLogout = () => {
      setUser();
      navigate("/", { replace: true });
      queryClient.clear();
      axiosInstanceWithAuth.defaults.headers.common["Authorization"] = null;
    };
    handleLogout();
  }, [navigate, setUser]);

  return <>Logout Page</>;
};

export default Logout;
