import { Outlet } from "react-router-dom";
import Navbar from "../components/common/Navbar/Navbar";
import Sidebar from "../components/common/Sidebar/Sidebar";

const MainLayout = () => {
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
