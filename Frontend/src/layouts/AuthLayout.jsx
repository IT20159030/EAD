import { Outlet } from "react-router-dom";

const AuthLayout = () => {
  return (
    <div className="auth-container">
      <main>
        <Outlet />
      </main>
    </div>
  );
};

export default AuthLayout;
