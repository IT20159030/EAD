import { Outlet } from "react-router-dom";
import Footer from "../components/common/Footer/Footer";

const AuthLayout = () => {
  return (
    <div className="auth-container">
      <main>
        <Outlet />
      </main>
      <Footer />
    </div>
  );
};

export default AuthLayout;
