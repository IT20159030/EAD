import { useState, useEffect } from "react";
import Button from "react-bootstrap/Button";
import Modal from "react-bootstrap/Modal";
import Form from "react-bootstrap/Form";
import Spinner from "react-bootstrap/Spinner";

const AddEditVendorModal = ({
  show,
  handleClose,
  vendorToEdit,
  handleSaveVendor,
  isInProgress,
}) => {
  const [formData, setFormData] = useState({
    vendorDetails: {
      vendorName: "",
      vendorEmail: "",
      vendorPhone: "",
      vendorAddress: "",
      vendorCity: "",
    },
    vendorAccountDetails: {
      name: "",
      email: "",
      nic: "",
      password: "",
    },
  });

  useEffect(() => {
    if (vendorToEdit) {
      setFormData({
        vendorDetails: {
          vendorName: vendorToEdit.vendorDetails.vendorName || "",
          vendorEmail: vendorToEdit.vendorDetails.vendorEmail || "",
          vendorPhone: vendorToEdit.vendorDetails.vendorPhone || "",
          vendorAddress: vendorToEdit.vendorDetails.vendorAddress || "",
          vendorCity: vendorToEdit.vendorDetails.vendorCity || "",
        },
        vendorAccountDetails: {
          name: vendorToEdit.vendorAccountDetails.name || "",
          email: vendorToEdit.vendorAccountDetails.email || "",
          nic: vendorToEdit.vendorAccountDetails.nic || "",
          password: "",
        },
      });
    } else {
      setFormData({
        vendorDetails: {
          vendorName: "",
          vendorEmail: "",
          vendorPhone: "",
          vendorAddress: "",
          vendorCity: "",
        },
        vendorAccountDetails: {
          name: "",
          email: "",
          nic: "",
          password: "",
        },
        // vendorDetails: {
        //   vendorName: "Test Vendor",
        //   vendorEmail: "info@example.com"
        //   vendorPhone: "123123123",
        //   vendorAddress: "123, Test Street",
        //   vendorCity: "Colombo",
        // },
        // vendorAccountDetails: {
        //   name: "vendor test",
        //   email: "vendortest@example.com",
        //   nic: "123123123123",
        //   password: "password",
        // },
      });
    }
  }, [vendorToEdit]);

  const [errors, setErrors] = useState({});

  const validateForm = () => {
    let newErrors = {};
    let isValid = true;

    // Validate vendorDetails
    Object.keys(formData.vendorDetails).forEach((key) => {
      if (!formData.vendorDetails[key]) {
        newErrors[`vendorDetails.${key}`] = "This field is required";
        isValid = false;
      }
    });

    // Validate vendorAccountDetails
    Object.keys(formData.vendorAccountDetails).forEach((key) => {
      if (
        !formData.vendorAccountDetails[key] &&
        (key !== "password" || !vendorToEdit)
      ) {
        newErrors[`vendorAccountDetails.${key}`] = "This field is required";
        isValid = false;
      }
    });

    // validate phone number
    if (
      formData.vendorDetails.vendorPhone &&
      !/^\d{10}$/.test(formData.vendorDetails.vendorPhone)
    ) {
      newErrors["vendorDetails.vendorPhone"] =
        "Please enter a valid phone number";
      isValid = false;
    }

    // Validate emails
    if (
      formData.vendorDetails.vendorEmail &&
      !/\S+@\S+\.\S+/.test(formData.vendorDetails.vendorEmail)
    ) {
      newErrors["vendorDetails.vendorEmail"] =
        "Please enter a valid email address";
      isValid = false;
    }
    if (
      formData.vendorAccountDetails.email &&
      !/\S+@\S+\.\S+/.test(formData.vendorAccountDetails.email)
    ) {
      newErrors["vendorAccountDetails.email"] =
        "Please enter a valid email address";
      isValid = false;
    }

    // Validate NIC
    const nicRegex = /^(\d{12}|\d{9}[vV])$/;
    if (
      formData.vendorAccountDetails.nic &&
      !nicRegex.test(formData.vendorAccountDetails.nic)
    ) {
      newErrors["vendorAccountDetails.nic"] =
        'NIC must be either a 12-digit number or a 9-digit number ending with "v" or "V"';
      isValid = false;
    }

    setErrors(newErrors);
    return isValid;
  };

  const handleSubmit = async () => {
    if (validateForm()) {
      handleSaveVendor(formData);
      handleClose();
    }
  };

  const handleInputChange = (e) => {
    const { id, value } = e.target;
    const [section, field] = id.split(".");
    setFormData({
      ...formData,
      [section]: {
        ...formData[section],
        [field]: value,
      },
    });
    // Clear the error for this field as the user types
    if (errors[id]) {
      setErrors({ ...errors, [id]: "" });
    }
  };

  return (
    <Modal show={show} onHide={handleClose} className="darkModal" centered>
      <Modal.Header closeButton className="modalHeader">
        <Modal.Title>{vendorToEdit ? "Edit" : "Add"} Vendor</Modal.Title>
      </Modal.Header>

      <Modal.Body className="modalBody">
        <Form>
          <h5>Store Details</h5>
          <Form.Group controlId="vendorDetails.vendorName">
            <Form.Label>Store Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter store name"
              value={formData.vendorDetails.vendorName}
              onChange={handleInputChange}
              isInvalid={!!errors["vendorDetails.vendorName"]}
            />
            <Form.Control.Feedback type="invalid">
              {errors["vendorDetails.vendorName"]}
            </Form.Control.Feedback>
          </Form.Group>
          <Form.Group controlId="vendorDetails.vendorEmail" className="mt-3">
            <Form.Label>Store Email</Form.Label>
            <Form.Control
              type="email"
              placeholder="Enter vendor email"
              value={formData.vendorDetails.vendorEmail}
              onChange={handleInputChange}
              isInvalid={!!errors["vendorDetails.vendorEmail"]}
            />
            <Form.Control.Feedback type="invalid">
              {errors["vendorDetails.vendorEmail"]}
            </Form.Control.Feedback>
          </Form.Group>
          <Form.Group controlId="vendorDetails.vendorPhone" className="mt-3">
            <Form.Label>Store Phone</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter store phone"
              value={formData.vendorDetails.vendorPhone}
              onChange={handleInputChange}
              isInvalid={!!errors["vendorDetails.vendorPhone"]}
            />
            <Form.Control.Feedback type="invalid">
              {errors["vendorDetails.vendorPhone"]}
            </Form.Control.Feedback>
          </Form.Group>
          <Form.Group controlId="vendorDetails.vendorAddress" className="mt-3">
            <Form.Label>Store Address</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter store address"
              value={formData.vendorDetails.vendorAddress}
              onChange={handleInputChange}
              isInvalid={!!errors["vendorDetails.vendorAddress"]}
            />
            <Form.Control.Feedback type="invalid">
              {errors["vendorDetails.vendorAddress"]}
            </Form.Control.Feedback>
          </Form.Group>
          <Form.Group controlId="vendorDetails.vendorCity" className="mt-3">
            <Form.Label>Store City</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter store city"
              value={formData.vendorDetails.vendorCity}
              onChange={handleInputChange}
              isInvalid={!!errors["vendorDetails.vendorCity"]}
            />
            <Form.Control.Feedback type="invalid">
              {errors["vendorDetails.vendorCity"]}
            </Form.Control.Feedback>
          </Form.Group>

          <h5 className="mt-4">Account Details</h5>
          <Form.Group controlId="vendorAccountDetails.name">
            <Form.Label>Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter name"
              value={formData.vendorAccountDetails.name}
              onChange={handleInputChange}
              isInvalid={!!errors["vendorAccountDetails.name"]}
            />
            <Form.Control.Feedback type="invalid">
              {errors["vendorAccountDetails.name"]}
            </Form.Control.Feedback>
          </Form.Group>
          <Form.Group controlId="vendorAccountDetails.email" className="mt-3">
            <Form.Label>Email</Form.Label>
            <Form.Control
              type="email"
              placeholder="Enter email"
              value={formData.vendorAccountDetails.email}
              onChange={handleInputChange}
              isInvalid={!!errors["vendorAccountDetails.email"]}
            />
            <Form.Control.Feedback type="invalid">
              {errors["vendorAccountDetails.email"]}
            </Form.Control.Feedback>
          </Form.Group>
          <Form.Group controlId="vendorAccountDetails.nic" className="mt-3">
            <Form.Label>Account holder NIC</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter NIC"
              value={formData.vendorAccountDetails.nic}
              onChange={handleInputChange}
              isInvalid={!!errors["vendorAccountDetails.nic"]}
            />
            <Form.Control.Feedback type="invalid">
              {errors["vendorAccountDetails.nic"]}
            </Form.Control.Feedback>
          </Form.Group>
          {!vendorToEdit && (
            <Form.Group
              controlId="vendorAccountDetails.password"
              className="mt-3"
            >
              <Form.Label>Password</Form.Label>
              <Form.Control
                type="password"
                placeholder="Enter password"
                value={formData.vendorAccountDetails.password}
                onChange={handleInputChange}
                isInvalid={!!errors["vendorAccountDetails.password"]}
              />
              <Form.Control.Feedback type="invalid">
                {errors["vendorAccountDetails.password"]}
              </Form.Control.Feedback>
            </Form.Group>
          )}
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
              {vendorToEdit ? "Update" : "Add"} Vendor
            </Button>
          </>
        )}
      </Modal.Footer>
    </Modal>
  );
};

export default AddEditVendorModal;
