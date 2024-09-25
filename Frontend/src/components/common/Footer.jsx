import { Container, Row, Col } from "react-bootstrap";

const Footer = () => {
  return (
    <footer
      style={{
        backgroundColor: "var(--tertiary-color)",
        padding: "10px 0",
        position: "fixed",
        width: "100%",
        bottom: 0,
      }}
    >
      <Container>
        <Row className="d-flex align-items-center justify-content-center">
          <Col md={6} className="text-center">
            <p className="mb-0" style={{ color: "var(--light-text-color)" }}>
              &copy; 2024 Ecommerce store. All rights reserved.
            </p>
          </Col>
        </Row>
      </Container>
    </footer>
  );
};

export default Footer;
