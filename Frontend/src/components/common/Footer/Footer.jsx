import Container from "react-bootstrap/Container";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";

import "./Footer.css";

const Footer = () => {
  return (
    <footer className="footer">
      <Container>
        <Row className="d-flex align-items-center justify-content-center">
          <Col md={6} className="text-center">
            <p className="footer-text">
              &copy; 2024 QuickMart. All rights reserved.
            </p>
          </Col>
        </Row>
      </Container>
    </footer>
  );
};

export default Footer;
