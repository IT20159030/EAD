import { useNavigate } from "react-router-dom";
import { useAuth } from "../../provider/authProvider";
import { useEffect } from "react";

const Logout = () => {
  const { setUser } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const handleLogout = () => {
      setUser();
      navigate("/", { replace: true });
    };
    handleLogout();
  }, [navigate, setUser]);

  return <>Logout Page</>;
};

export default Logout;
