import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { Link, Navigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";
import Login from "./pages/Login";
// import Dashboard from "./pages/Dashboard";
import About from "./pages/About";
import ProductCategory from "./pages/product/ProductCategory";
import Products from "./pages/product/Products";
import NotificationsPage from "./pages/notification";
import MainLayout from "./layouts/MainLayout";
import AuthLayout from "./layouts/AuthLayout";
import RoleCheck from "./layouts/RoleCheck";
import AuthProvider from "./provider/authProvider";
import Logout from "./pages/auth/logout";
import StaffManagement from "./pages/user/StaffManagement";
import CustomerManagement from "./pages/user/CustomerManagement";
import ApprovalRequests from "./pages/user/ApprovalRequests";
import VendorManagement from "./pages/user/VendorManagement";
import Profile from "./pages/Profile";
import queryClient from "./utils/queryClient";

import "./App.css";
import Order from "./pages/Order.jsx";
import VenderStore from "./pages/VenderStore.jsx";

function App() {
  return (
    <AuthProvider>
      <QueryClientProvider client={queryClient}>
        <Router>
          <Routes>
            <Route path="/" element={<MainLayout />}>
              <Route index element={<Navigate to="/dashboard" replace />} />
              {/* <Route path="dashboard" element={<Dashboard />} /> */}
              <Route path="profile" element={<Profile />} />

              <Route path="categories" element={<ProductCategory />} />
              <Route path="orders" element={<Order />} />

              <Route path="inventory" element={<About />} />
              <Route path="reports" element={<About />} />
              <Route path="/notifications" element={<NotificationsPage />} />
              <Route element={<RoleCheck roles={["admin", "csr"]} />}>
                <Route path="customers" element={<CustomerManagement />} />
                <Route
                  path="customers/approval-requests"
                  element={<ApprovalRequests />}
                />
                <Route
                  path="customers/approval-requests/:userid"
                  element={<ApprovalRequests />}
                />
              </Route>
              <Route element={<RoleCheck roles={["admin"]} />}>
                <Route path="vendors" element={<VendorManagement />} />
                <Route path="staff" element={<StaffManagement />} />
              </Route>

              <Route element={<RoleCheck roles={["admin", "vendor"]} />}>
                <Route path="products" element={<Products />} />
              </Route>

              <Route element={<RoleCheck roles={["vendor"]} />}>
                <Route path="vendor-store" element={<VenderStore />} />
              </Route>

              <Route
                path="/unauthorized"
                element={
                  <div>
                    <h4>Unauthorized</h4>
                    <Link to="/">Home</Link>
                  </div>
                }
              />
            </Route>
            <Route path="/" element={<AuthLayout />}>
              <Route path="login" element={<Login />} />
              <Route path="logout" element={<Logout />} />
            </Route>

            <Route path="*" element={<div>404 Not Found</div>} />
          </Routes>
        </Router>
        <ReactQueryDevtools initialIsOpen={false} />
      </QueryClientProvider>
    </AuthProvider>
  );
}

export default App;
