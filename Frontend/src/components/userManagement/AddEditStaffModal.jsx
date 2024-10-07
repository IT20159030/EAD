import { useState, useEffect } from "react";
import Button from "react-bootstrap/Button";
import Modal from "react-bootstrap/Modal";
import Form from "react-bootstrap/Form";
import Spinner from "react-bootstrap/Spinner";

/*
 * The AddEditStaffModal component is a form that allows admins to add or edit a staff member.
 */

const AddEditStaffModal = ({
  show,
  handleClose,
  staffToEdit,
  handleSaveStaff,
  isInProgress,
}) => {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    nic: "",
    email: "",
    password: "",
    role: "",
  });

  useEffect(() => {
    if (staffToEdit) {
      setFormData({
        firstName: staffToEdit.firstName || "",
        lastName: staffToEdit.lastName || "",
        nic: staffToEdit.nic || "",
        password: "", // Password should be empty on edit
        email: staffToEdit.email || "",
        role: staffToEdit.role || "",
      });
    } else {
      setFormData({
        firstName: "",
        lastName: "",
        nic: "",
        email: "",
        password: "",
        role: "",
      });
    }
  }, [staffToEdit]);
  const [errors, setErrors] = useState({});
  const validateForm = () => {
    let newErrors = {};
    let isValid = true;

    // Check for required fields
    Object.keys(formData).forEach((key) => {
      if (!formData[key] && (key !== "password" || !staffToEdit)) {
        newErrors[key] = "This field is required";
        isValid = false;
      }
    });

    // Validate first name and last name
    if (formData.firstName && formData.firstName.includes(" ")) {
      newErrors.firstName = "First name cannot contain spaces";
      isValid = false;
    }
    if (formData.lastName && formData.lastName.includes(" ")) {
      newErrors.lastName = "Last name cannot contain spaces";
      isValid = false;
    }

    // Validate email
    if (formData.email && !/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = "Please enter a valid email address";
      isValid = false;
    }

    // Validate NIC
    const nicRegex = /^(\d{12}|\d{9}[vV])$/;
    if (formData.nic && !nicRegex.test(formData.nic)) {
      newErrors.nic =
        'NIC must be either a 12-digit number or a 9-digit number ending with "v" or "V"';
      isValid = false;
    }

    setErrors(newErrors);
    return isValid;
  };

  const handleSubmit = async () => {
    if (validateForm()) {
      handleSaveStaff(formData);
      handleClose();
    }
  };

  const handleInputChange = (e) => {
    const { id, value } = e.target;
    setFormData({ ...formData, [id]: value });
    // Clear the error for this field as the user types
    if (errors[id]) {
      setErrors({ ...errors, [id]: "" });
    }
  };

  return (
    <Modal show={show} onHide={handleClose} className="darkModal" centered>
      <Modal.Header closeButton className="modalHeader">
        <Modal.Title>{staffToEdit ? "Edit" : "Add"} Staff</Modal.Title>
      </Modal.Header>

      <Modal.Body className="modalBody">
        <Form>
          <Form.Group controlId="firstName">
            <Form.Label>First Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter first name"
              value={formData.firstName}
              onChange={handleInputChange}
              isInvalid={!!errors.firstName}
            />
            <Form.Control.Feedback type="invalid">
              {errors.firstName}
            </Form.Control.Feedback>
          </Form.Group>
          <Form.Group controlId="lastName" className="mt-3">
            <Form.Label>Last Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter last name"
              value={formData.lastName}
              onChange={handleInputChange}
              isInvalid={!!errors.lastName}
            />
            <Form.Control.Feedback type="invalid">
              {errors.lastName}
            </Form.Control.Feedback>
          </Form.Group>
          <Form.Group controlId="nic" className="mt-3">
            <Form.Label>NIC</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter NIC"
              value={formData.nic}
              onChange={handleInputChange}
              isInvalid={!!errors.nic}
            />
            <Form.Control.Feedback type="invalid">
              {errors.nic}
            </Form.Control.Feedback>
          </Form.Group>
          <Form.Group controlId="email" className="mt-3">
            <Form.Label>Email</Form.Label>
            <Form.Control
              type="email"
              placeholder="Enter email"
              value={formData.email}
              onChange={handleInputChange}
              isInvalid={!!errors.email}
            />
            <Form.Control.Feedback type="invalid">
              {errors.email}
            </Form.Control.Feedback>
          </Form.Group>
          {staffToEdit ? null : (
            <Form.Group controlId="password" className="mt-3">
              <Form.Label>Password</Form.Label>
              <Form.Control
                type="password"
                placeholder="Enter password"
                value={formData.password}
                onChange={handleInputChange}
                isInvalid={!!errors.password}
              />
              <Form.Control.Feedback type="invalid">
                {errors.password}
              </Form.Control.Feedback>
            </Form.Group>
          )}
          <Form.Group controlId="role" className="mt-3">
            <Form.Label>Role</Form.Label>
            <Form.Control
              as="select"
              value={formData.role}
              onChange={handleInputChange}
              isInvalid={!!errors.role}
            >
              <option value="">Select role</option>
              <option value="admin">Admin</option>
              <option value="csr">CSR (Customer Service Representative)</option>
            </Form.Control>
            <Form.Control.Feedback type="invalid">
              {errors.role}
            </Form.Control.Feedback>
          </Form.Group>
        </Form>
      </Modal.Body>

      <Modal.Footer className="modalFooter">
        {isInProgress ? (
          <Spinner animation="border" variant="primary" />
        ) : (
          <>
            <Button variant="secondary" onClick={handleClose}>
              Close
            </Button>
            <Button variant="primary" onClick={handleSubmit}>
              {staffToEdit ? "Update" : "Add"} Staff
            </Button>
          </>
        )}
      </Modal.Footer>
    </Modal>
  );
};

export default AddEditStaffModal;
