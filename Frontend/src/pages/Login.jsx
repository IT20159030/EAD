import { useEffect, useState } from "react";
import {
  Form,
  Button,
  Container,
  // Image
} from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../provider/authProvider";
import { login } from "../api/authApi";

import "./styles/Login.css";

// import logo from "./logo.png"; // Import your logo image

const Login = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });
  const { user, setUser } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (user) {
      navigate("/", { replace: true });
    }
  }, [navigate, user]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      setError("");
      setLoading(true);
      const res = await login(formData);
      setUser({
        token: res.accessToken,
        name: res.profile.name,
        email: res.profile.email,
        role: res.profile.role,
        id: res.profile.userId,
        nic: res.profile.nic,
      });
      navigate("/", { replace: true });
    } catch (error) {
      setError(error.response.data);
      setLoading(false);
      console.log(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container className="login-container">
      <div className="text-center mb-4">
        {/* <Image src={logo} alt="Logo" fluid className="logo" /> */}
      </div>
      <h2 className="text-center">Login</h2>
      <Form className="login-form" onSubmit={handleSubmit}>
        <Form.Group controlId="formBasicEmail">
          <Form.Label>Email address</Form.Label>
          <Form.Control
            type="email"
            placeholder="Enter email"
            value={formData.email}
            onChange={(e) =>
              setFormData({ ...formData, email: e.target.value })
            }
            required
          />
        </Form.Group>

        <Form.Group controlId="formBasicPassword" className="mt-3">
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            placeholder="Password"
            value={formData.password}
            onChange={(e) =>
              setFormData({ ...formData, password: e.target.value })
            }
            required
          />
        </Form.Group>

        {error && <p className="text-danger mt-2">{error}</p>}
        <Button
          variant="primary"
          type="submit"
          className="w-100 mt-3"
          disabled={loading}
        >
          Login
        </Button>
      </Form>
      {/* <div className="text-center mt-3">
        <a href="/forgot-password" className="text-muted">
          Forgot Password?
        </a>
      </div>
      <div className="text-center mt-2">
        <span> {"Don't have an account?"} </span>
        <a href="/register" className="text-primary">
          Create one
        </a>
      </div> */}

      <div className="">
        For development:
        <br />
        <Button
          variant="primary"
          className="w-100 mt-3"
          onClick={() => {
            setFormData({
              email: "yoda@example.com",
              password: "password",
            });
          }}
        >
          Admin
        </Button>
        <Button
          variant="primary"
          className="w-100 mt-3"
          onClick={() => {
            setFormData({
              email: "anakinskywalker@example.com",
              password: "password",
            });
          }}
        >
          Vendor
        </Button>
        <Button
          variant="primary"
          className="w-100 mt-3"
          onClick={() => {
            setFormData({
              email: "landocalrissian@example.com",
              password: "password",
            });
          }}
        >
          Csr
        </Button>
      </div>
    </Container>
  );
};

export default Login;
