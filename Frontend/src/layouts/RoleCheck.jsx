import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../provider/authProvider";

const RoleCheck = ({ roles }) => {
  const { user } = useAuth();

  if (!user || !roles.includes(user.role)) {
    return <Navigate to="/unauthorized" />;
  }
  return <Outlet />;
};

export default RoleCheck;
