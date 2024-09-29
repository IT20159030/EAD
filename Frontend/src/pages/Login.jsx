import { Form, Button, Container, Image } from "react-bootstrap";
import "./styles/Login.css";
// import logo from "./logo.png"; // Import your logo image

const Login = () => {
  return (
    <Container className="login-container">
      <div className="text-center mb-4">
        {/* <Image src={logo} alt="Logo" fluid className="logo" /> */}
      </div>
      <h2 className="text-center">Login</h2>
      <Form className="login-form">
        <Form.Group controlId="formBasicEmail">
          <Form.Label>Email address</Form.Label>
          <Form.Control type="email" placeholder="Enter email" required />
        </Form.Group>

        <Form.Group controlId="formBasicPassword" className="mt-3">
          <Form.Label>Password</Form.Label>
          <Form.Control type="password" placeholder="Password" required />
        </Form.Group>

        <Button variant="primary" type="submit" className="w-100 mt-3">
          Login
        </Button>
      </Form>
      <div className="text-center mt-3">
        <a href="/forgot-password" className="text-muted">
          Forgot Password?
        </a>
      </div>
      <div className="text-center mt-2">
        <span> {"Don't have an account?"} </span>
        <a href="/register" className="text-primary">
          Create one
        </a>
      </div>
    </Container>
  );
};

export default Login;
