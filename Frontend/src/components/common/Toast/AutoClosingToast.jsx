import { useEffect, useState } from "react";
import Toast from "react-bootstrap/Toast";
import "./AutoClosingToast.css";

const AutoClosingToast = ({ type, title, description, onClose }) => {
  const [show, setShow] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => {
      setShow(false);
      onClose();
    }, 3000);
    return () => clearTimeout(timer);
  }, [onClose]);

  const toastTypeClass = type === "success" ? "toastSuccess" : "toastError";

  return (
    <div className="toastContainer">
      {" "}
      <Toast
        show={show}
        onClose={() => {
          setShow(false);
          onClose();
        }}
        className={toastTypeClass}
      >
        <Toast.Header>
          <strong className="me-auto">{title}</strong>
        </Toast.Header>
        <Toast.Body>{description}</Toast.Body>
      </Toast>
    </div>
  );
};

export default AutoClosingToast;
