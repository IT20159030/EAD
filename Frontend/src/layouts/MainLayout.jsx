import { Outlet } from "react-router-dom";
import Navbar from "../components/common/Navbar/Navbar";
import Sidebar from "../components/common/Sidebar/Sidebar";
import { useAuth } from "../provider/authProvider";
import { Navigate } from "react-router-dom";

const MainLayout = () => {
  const { user } = useAuth();

  if (!user) {
    return <Navigate to="/login" />;
  }
  return (
    <div className="app-container">
      <Sidebar />
      <div className="main-content">
        <Navbar />
        <main>
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default MainLayout;
