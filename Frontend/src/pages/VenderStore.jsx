import { useState, useEffect } from "react";
import { Button, Modal, Form, Alert, Spinner, Card } from "react-bootstrap";
import CommonTitle from "../components/common/Title/Title";

// You'll need to create these custom hooks similar to profileHooks
import {
  useGetVendorStore,
  useUpdateVendorStore,
} from "../hooks/vendorStoreHooks.js";

const VendorStore = () => {
  const [showModal, setShowModal] = useState(false);
  const [editedStore, setEditedStore] = useState({});
  const [errors, setErrors] = useState({});

  const { data: store, isLoading, isError } = useGetVendorStore();
  const updateStoreMutation = useUpdateVendorStore();

  useEffect(() => {
    if (store) {
      setEditedStore(store);
    }
  }, [store]);

  const handleEditClick = () => {
    setShowModal(true);
    setErrors({});
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setEditedStore({ ...editedStore, [name]: value });
    if (errors[name]) {
      setErrors({ ...errors, [name]: "" });
    }
  };

  const validateForm = () => {
    let newErrors = {};
    let isValid = true;

    [
      "vendorName",
      "vendorEmail",
      "vendorPhone",
      "vendorAddress",
      "vendorCity",
    ].forEach((field) => {
      if (!editedStore[field]) {
        newErrors[field] = "This field is required";
        isValid = false;
      }
    });

    if (
      editedStore.vendorEmail &&
      !/\S+@\S+\.\S+/.test(editedStore.vendorEmail)
    ) {
      newErrors.vendorEmail = "Please enter a valid email address";
      isValid = false;
    }

    if (editedStore.vendorPhone && !/^\d{10}$/.test(editedStore.vendorPhone)) {
      newErrors.vendorPhone = "Phone number must be 10 digits";
      isValid = false;
    }

    setErrors(newErrors);
    return isValid;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (validateForm()) {
      updateStoreMutation.mutate(editedStore, {
        onSuccess: () => setShowModal(false),
      });
    }
  };

  if (isLoading) return <Spinner animation="border" />;
  if (isError)
    return (
      <Alert variant="danger">Error loading vendor store information</Alert>
    );

  return (
    <div className="container mt-4">
      <CommonTitle title="Vendor Store Information" />
      <Card className="shadow-sm p-2 border-0">
        <Card.Body>
          <h2 className="mb-3">{store.vendorName}</h2>
          <div className="d-flex flex-column gap-2">
            <div className="d-flex">
              <div className="text-muted" style={{ width: "100px" }}>
                Email
              </div>
              <div className="text-muted mx-2">:</div>
              <div className="text-dark fw-semibold">{store.vendorEmail}</div>
            </div>
            <div className="d-flex">
              <div className="text-muted" style={{ width: "100px" }}>
                Phone
              </div>
              <div className="text-muted mx-2">:</div>
              <div className="text-dark fw-semibold">{store.vendorPhone}</div>
            </div>
            <div className="d-flex">
              <div className="text-muted" style={{ width: "100px" }}>
                Address
              </div>
              <div className="text-muted mx-2">:</div>
              <div className="text-dark fw-semibold">{store.vendorAddress}</div>
            </div>
            <div className="d-flex">
              <div className="text-muted" style={{ width: "100px" }}>
                City
              </div>
              <div className="text-muted mx-2">:</div>
              <div className="text-dark fw-semibold">{store.vendorCity}</div>
            </div>
          </div>
          <div className="d-flex justify-content-end mt-3">
            <Button variant="primary" onClick={handleEditClick}>
              Edit Store Information
            </Button>
          </div>
        </Card.Body>
      </Card>

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Edit Store Information</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Store Name</Form.Label>
              <Form.Control
                type="text"
                name="vendorName"
                value={editedStore.vendorName || ""}
                onChange={handleInputChange}
                isInvalid={!!errors.vendorName}
              />
              <Form.Control.Feedback type="invalid">
                {errors.vendorName}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                name="vendorEmail"
                value={editedStore.vendorEmail || ""}
                onChange={handleInputChange}
                isInvalid={!!errors.vendorEmail}
              />
              <Form.Control.Feedback type="invalid">
                {errors.vendorEmail}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Phone</Form.Label>
              <Form.Control
                type="tel"
                name="vendorPhone"
                value={editedStore.vendorPhone || ""}
                onChange={handleInputChange}
                isInvalid={!!errors.vendorPhone}
              />
              <Form.Control.Feedback type="invalid">
                {errors.vendorPhone}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Address</Form.Label>
              <Form.Control
                type="text"
                name="vendorAddress"
                value={editedStore.vendorAddress || ""}
                onChange={handleInputChange}
                isInvalid={!!errors.vendorAddress}
              />
              <Form.Control.Feedback type="invalid">
                {errors.vendorAddress}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>City</Form.Label>
              <Form.Control
                type="text"
                name="vendorCity"
                value={editedStore.vendorCity || ""}
                onChange={handleInputChange}
                isInvalid={!!errors.vendorCity}
              />
              <Form.Control.Feedback type="invalid">
                {errors.vendorCity}
              </Form.Control.Feedback>
            </Form.Group>
            <Button
              variant="primary"
              type="submit"
              disabled={updateStoreMutation.isLoading}
            >
              {updateStoreMutation.isLoading ? "Saving..." : "Save Changes"}
            </Button>
          </Form>
        </Modal.Body>
      </Modal>
    </div>
  );
};

export default VendorStore;
