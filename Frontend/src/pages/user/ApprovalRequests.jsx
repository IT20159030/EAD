import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Button, Table, Form, Modal, Spinner, Alert } from "react-bootstrap";
import { MdRemoveRedEye } from "react-icons/md";
import CommonTitle from "../../components/common/Title/Title";
import LoadingTableBody from "../../components/common/TableLoader/TableLoader";
import { useAuth } from "../../provider/authProvider";
import {
  useGetAllCustomerAccounts,
  useUpdateCustomerStatus,
  useGetCustomerDetails,
} from "../../hooks/customerManagementHooks";

const CustomerManagement = () => {
  const [search, setSearch] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [selectedCustomerId, setSelectedCustomerId] = useState(null);
  const navigate = useNavigate();
  const { userid } = useParams();

  const {
    user: { id: currentUserId },
  } = useAuth();
  const {
    data: customerAccounts,
    isLoading: isLoadingCustomerAccounts,
    isError: isLoadingError,
  } = useGetAllCustomerAccounts();
  const { mutate: updateCustomerStatus, isPending: isChangingStatus } =
    useUpdateCustomerStatus();

  useEffect(() => {
    if (userid) {
      setSelectedCustomerId(userid);
      setShowModal(true);
    }
  }, [userid]);

  const handleCustomerStatusUpdate = (customerId, status) => {
    updateCustomerStatus(
      {
        id: customerId,
        status: status,
      },
      {
        onSuccess: () => {
          setShowModal(false);
          navigate("/customers/approval-requests");
        },
        onError: () => {
          alert(`Failed to ${status.toLowerCase()} customer`);
        },
      }
    );
  };

  const handleModalOpen = (customerId) => {
    setSelectedCustomerId(customerId);
    setShowModal(true);
    navigate(`/customers/approval-requests/${customerId}`);
  };

  const handleModalClose = () => {
    setShowModal(false);
    setSelectedCustomerId(null);
    navigate("/customers/approval-requests");
  };

  const filteredCustomers =
    customerAccounts
      ?.filter((customer) => customer.status === "Unapproved")
      .filter(
        (customer) =>
          customer.name.toLowerCase().includes(search.toLowerCase()) ||
          customer.email.toLowerCase().includes(search.toLowerCase())
      ) || [];

  if (isLoadingError) {
    return (
      <div>
        <CommonTitle title="Approval Requests" />
        <Alert variant="danger">Failed to load approval requests</Alert>
      </div>
    );
  }

  return (
    <div>
      <CommonTitle title="Approval Requests" />
      <Form className="mb-3">
        <Form.Control
          type="text"
          placeholder="Search..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </Form>
      {isLoadingCustomerAccounts ? (
        <Table striped hover responsive>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Email</th>
              <th>Actions</th>
            </tr>
          </thead>
          <LoadingTableBody loading={true} colSpan="4" />
        </Table>
      ) : filteredCustomers.length === 0 ? (
        <Alert variant="info">No approval requests found</Alert>
      ) : (
        <Table striped hover responsive>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Email</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filteredCustomers.map((customer, index) => (
              <tr key={customer.id}>
                <td>{index + 1}</td>
                <td>
                  {customer.name} {customer.id === currentUserId && "(you)"}
                </td>
                <td>{customer.email}</td>
                <td>
                  <Button
                    onClick={() => handleModalOpen(customer.id)}
                    disabled={isChangingStatus}
                  >
                    <MdRemoveRedEye />
                    <span className="ms-2">Review</span>
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
      {showModal && (
        <ApprovalRequestModal
          show={showModal}
          customerId={selectedCustomerId}
          handleClose={handleModalClose}
          handleStatusUpdate={handleCustomerStatusUpdate}
          isChangingStatus={isChangingStatus}
        />
      )}
    </div>
  );
};

const ApprovalRequestModal = ({
  show,
  customerId,
  handleClose,
  handleStatusUpdate,
  isChangingStatus,
}) => {
  const { data: customerDetails, isLoading } =
    useGetCustomerDetails(customerId);

  if (isLoading) {
    return (
      <Modal show={show} onHide={handleClose}>
        <Modal.Header closeButton>
          <Modal.Title>Loading...</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <p>Loading customer details...</p>
        </Modal.Body>
      </Modal>
    );
  }

  return (
    <Modal show={show} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Customer Approval Request</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p>
          <strong>Name:</strong> {customerDetails.name}
        </p>
        <p>
          <strong>NIC:</strong> {customerDetails.nic}
        </p>
        <p>
          <strong>Email:</strong> {customerDetails.email}
        </p>
        <p>
          <strong>Create At:</strong>{" "}
          {new Date(customerDetails.createdAt).toLocaleDateString()} at{" "}
          {new Date(customerDetails.createdAt).toLocaleTimeString()}
        </p>
        {/* Add any additional customer details here */}
      </Modal.Body>
      <Modal.Footer>
        <Button
          variant="secondary"
          onClick={handleClose}
          disabled={isChangingStatus}
        >
          Close
        </Button>
        <Button
          variant="danger"
          onClick={() => handleStatusUpdate(customerId, "Rejected")}
          disabled={isChangingStatus}
        >
          {isChangingStatus ? (
            <>
              <Spinner
                as="span"
                animation="border"
                size="sm"
                role="status"
                aria-hidden="true"
              />
              <span className="ms-2">Rejecting...</span>
            </>
          ) : (
            "Reject"
          )}
        </Button>
        <Button
          variant="primary"
          onClick={() => handleStatusUpdate(customerId, "Active")}
          disabled={isChangingStatus}
        >
          {isChangingStatus ? (
            <>
              <Spinner
                as="span"
                animation="border"
                size="sm"
                role="status"
                aria-hidden="true"
              />
              <span className="ms-2">Approving...</span>
            </>
          ) : (
            "Approve"
          )}
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default CustomerManagement;
