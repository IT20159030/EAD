/* eslint-disable indent */
import { useState, useEffect } from "react";
import { Button, Modal, Form, Alert, Spinner, Card } from "react-bootstrap";
import { useAuth } from "../provider/authProvider";
import { useGetProfile, useUpdateProfile } from "../hooks/profileHooks";
import CommonTitle from "../components/common/Title/Title";

/*
 * The Profile component displays the user's profile information and allows them to edit it.
 * It uses the useGetProfile and useUpdateProfile hooks to fetch and update the user's profile data.
 */

const Profile = () => {
  const [showModal, setShowModal] = useState(false);
  const [editedUser, setEditedUser] = useState({});
  const [errors, setErrors] = useState({});
  const { updateUser } = useAuth();

  const { data: user, isLoading, isError } = useGetProfile();
  const updateProfileMutation = useUpdateProfile();

  useEffect(() => {
    if (user) {
      setEditedUser({
        firstName: user.name.split(" ")[0],
        lastName: user.name.split(" ")[1],
        ...user,
      });
    }
  }, [user]);

  const handleEditClick = () => {
    setShowModal(true);
    setErrors({});
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setEditedUser({ ...editedUser, [name]: value });
    if (errors[name]) {
      setErrors({ ...errors, [name]: "" });
    }
  };

  const validateForm = () => {
    let newErrors = {};
    let isValid = true;

    ["firstName", "lastName", "email", "nic"].forEach((field) => {
      if (!editedUser[field]) {
        newErrors[field] = "This field is required";
        isValid = false;
      }
    });

    if (editedUser.firstName && editedUser.firstName.includes(" ")) {
      newErrors.firstName = "First name cannot contain spaces";
      isValid = false;
    }
    if (editedUser.lastName && editedUser.lastName.includes(" ")) {
      newErrors.lastName = "Last name cannot contain spaces";
      isValid = false;
    }

    if (editedUser.email && !/\S+@\S+\.\S+/.test(editedUser.email)) {
      newErrors.email = "Please enter a valid email address";
      isValid = false;
    }

    const nicRegex = /^(\d{12}|\d{9}[vV])$/;
    if (editedUser.nic && !nicRegex.test(editedUser.nic)) {
      newErrors.nic =
        'NIC must be either a 12-digit number or a 9-digit number ending with "v" or "V"';
      isValid = false;
    }

    setErrors(newErrors);
    return isValid;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (validateForm()) {
      const updatedUserData = {
        ...editedUser,
        name: `${editedUser.firstName} ${editedUser.lastName}`,
      };
      updateProfileMutation.mutate(updatedUserData, {
        onSuccess: (updatedUser) => {
          updateUser(updatedUser);
          setShowModal(false);
        },
      });
    }
  };

  if (isLoading) return <Spinner animation="border" />;
  if (isError) return <Alert variant="danger">Error loading profile</Alert>;

  return (
    <div className="container mt-4">
      <CommonTitle title="Profile" />
      <Card className="shadow-sm p-2 border-0">
        <Card.Body>
          <div className="d-flex align-items-center gap-1">
            <h2 className="mb-2">{user.name} </h2>
            <div
              className={`badge ${
                user.role === "admin"
                  ? "bg-danger"
                  : user.role === "csr"
                  ? "bg-warning"
                  : user.role === "vendor"
                  ? "bg-info"
                  : "bg-secondary"
              }`}
            >
              {user.role === "admin"
                ? "Admin"
                : user.role === "csr"
                ? "CSR"
                : user.role === "vendor"
                ? "Vendor"
                : "Error"}
            </div>
          </div>
          <div className="d-flex flex-column gap-2">
            <div className="d-flex">
              <div className="text-muted" style={{ width: "100px" }}>
                Email
              </div>
              <div className="text-muted mx-2">:</div>
              <div className="text-dark fw-semibold">{user.email}</div>
            </div>
            <div className="d-flex">
              <div className="text-muted" style={{ width: "100px" }}>
                NIC
              </div>
              <div className="text-muted mx-2">:</div>
              <div className="text-dark fw-semibold">{user.nic}</div>
            </div>
            <div className="d-flex">
              <div className="text-muted" style={{ width: "100px" }}>
                Joined on
              </div>
              <div className="text-muted mx-2">:</div>
              <div className="text-dark fw-semibold">
                {new Date(user.createdAt).toLocaleDateString()}
              </div>
            </div>
          </div>
          <div className="d-flex justify-content-end mt-2">
            <Button variant="primary" onClick={handleEditClick}>
              Edit Profile
            </Button>
          </div>
        </Card.Body>
      </Card>

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Edit Profile</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>First Name</Form.Label>
              <Form.Control
                type="text"
                name="firstName"
                value={editedUser.firstName || ""}
                onChange={handleInputChange}
                isInvalid={!!errors.firstName}
              />
              <Form.Control.Feedback type="invalid">
                {errors.firstName}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Last Name</Form.Label>
              <Form.Control
                type="text"
                name="lastName"
                value={editedUser.lastName || ""}
                onChange={handleInputChange}
                isInvalid={!!errors.lastName}
              />
              <Form.Control.Feedback type="invalid">
                {errors.lastName}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                name="email"
                value={editedUser.email || ""}
                onChange={handleInputChange}
                isInvalid={!!errors.email}
                disabled
              />
              <Form.Control.Feedback type="invalid">
                {errors.email}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>NIC</Form.Label>
              <Form.Control
                type="text"
                name="nic"
                value={editedUser.nic || ""}
                onChange={handleInputChange}
                isInvalid={!!errors.nic}
              />
              <Form.Control.Feedback type="invalid">
                {errors.nic}
              </Form.Control.Feedback>
            </Form.Group>
            <Button
              variant="primary"
              type="submit"
              disabled={updateProfileMutation.isLoading}
            >
              {updateProfileMutation.isLoading ? "Saving..." : "Save Changes"}
            </Button>
          </Form>
        </Modal.Body>
      </Modal>
    </div>
  );
};

export default Profile;
