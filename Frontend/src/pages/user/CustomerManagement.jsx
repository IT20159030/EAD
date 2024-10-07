/* eslint-disable indent */
import { useState } from "react";
import Button from "react-bootstrap/Button";
import Table from "react-bootstrap/Table";
import { Alert, Badge, ButtonGroup, ToggleButton } from "react-bootstrap";
import styles from "../styles/Pages.module.css";
import Form from "react-bootstrap/Form";
import CommonTitle from "../../components/common/Title/Title";
import {
  useCreateCustomerAccount,
  useDeleteCustomerAccount,
  useGetAllCustomerAccounts,
  useUpdateCustomerAccount,
} from "../../hooks/customerManagementHooks";
import { MdDelete, MdEdit } from "react-icons/md";
import LoadingTableBody from "../../components/common/TableLoader/TableLoader";
import AddEditCustomerModal from "../../components/userManagement/AddEditCustomerModal";

/*
 * The CustomerManagement component displays a list of customers and allows
 * admins and csr to add, edit, and delete customers.
 */
const CustomerManagement = () => {
  const [showModal, setShowModal] = useState(false);
  const [customerToEdit, setCustomerToEdit] = useState(null);
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("all");

  const {
    data: customerAccounts,
    isLoading: isLoadingCustomerAccounts,
    isError: isLoadingError,
  } = useGetAllCustomerAccounts();

  const { mutate: createCustomerAccount, isLoading: isCreating } =
    useCreateCustomerAccount();

  const { mutate: updateCustomerAccount, isLoading: isUpdating } =
    useUpdateCustomerAccount();

  const { mutate: deleteCustomerAccount } = useDeleteCustomerAccount();

  const handleSaveCustomer = (customerData) => {
    if (customerToEdit) {
      updateCustomerAccount(
        { ...customerData, id: customerToEdit.id },
        {
          onSuccess: () => {
            setShowModal(false);
          },
          onError: () => {
            alert("Failed to update customer");
          },
        }
      );
    } else {
      createCustomerAccount(customerData, {
        onSuccess: () => {
          setShowModal(false);
        },
        onError: () => {
          alert("Failed to add customer");
        },
      });
    }
  };

  const handleDeleteCustomer = (customerId) => {
    if (window.confirm("Are you sure you want to delete this customer?")) {
      deleteCustomerAccount(customerId, {
        onSuccess: () => {
          alert("Customer deleted successfully!");
        },
        onError: () => {
          alert("Failed to delete customer");
        },
      });
    }
  };

  const handleModalOpen = () => {
    setCustomerToEdit(null);
    setShowModal(true);
  };

  if (isLoadingError) {
    return (
      <div>
        <CommonTitle title="Customers" />
        <Alert variant="danger">Failed to load customers</Alert>
      </div>
    );
  }

  return (
    <div>
      <CommonTitle
        title="Customers"
        buttonLabel="Add New Customer Account"
        onButtonClick={handleModalOpen}
      />
      <div className={styles.pageActions}>
        <Form className={`${styles.controls}`}>
          <Form.Control
            className={styles.search}
            type="text"
            placeholder="Search..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </Form>
        <ButtonGroup className="mb-2">
          {[
            {
              name: "All",
              value: "all",
            },
            {
              name: "Active",
              value: "Active",
            },
            {
              name: "Deactivated",
              value: "Deactivated",
            },
            {
              name: "Pending Approval",
              value: "Unapproved",
            },
            {
              name: "Rejected",
              value: "Rejected",
            },
          ].map((radio, idx) => (
            <ToggleButton
              key={idx}
              id={`radio-${idx}`}
              type="radio"
              variant={filter === radio.value ? "primary" : "outline-primary"}
              name="radio"
              value={radio.value}
              checked={filter === radio.value}
              onChange={(e) => setFilter(e.currentTarget.value)}
            >
              {radio.name}
            </ToggleButton>
          ))}
        </ButtonGroup>
      </div>
      <Table striped hover responsive className={styles.table}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Email</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        {isLoadingCustomerAccounts ? (
          <LoadingTableBody loading={isLoadingCustomerAccounts} colSpan="5" />
        ) : (
          <tbody>
            {customerAccounts
              .filter((customer) => {
                if (filter === "all") {
                  return true;
                }
                return customer.status === filter;
              })
              .filter(
                (customer) =>
                  customer.name.toLowerCase().includes(search.toLowerCase()) ||
                  customer.email.toLowerCase().includes(search.toLowerCase())
              )
              .map((customer, index) => (
                <tr key={index}>
                  <td>{index + 1}</td>
                  <td>{customer.name}</td>
                  <td>{customer.email}</td>
                  <td>
                    {customer.status === "Unapproved" ? (
                      <Badge bg="warning">Pending Approval</Badge>
                    ) : customer.status === "Rejected" ? (
                      <Badge bg="danger">Rejected</Badge>
                    ) : customer.status === "Active" ? (
                      <Badge bg="success">Active</Badge>
                    ) : (
                      <Badge bg="secondary">Deactivated</Badge>
                    )}
                  </td>
                  <td className={styles.actions}>
                    <Button
                      className={styles.button}
                      onClick={() => {
                        setCustomerToEdit(customer);
                        setShowModal(true);
                      }}
                    >
                      <MdEdit />
                      <span className="ms-2">Edit</span>
                    </Button>
                    <Button
                      className={styles.button}
                      onClick={() => handleDeleteCustomer(customer.id)}
                    >
                      <MdDelete />
                      <span className="ms-2">Delete</span>
                    </Button>
                  </td>
                </tr>
              ))}
          </tbody>
        )}
      </Table>

      {showModal && (
        <AddEditCustomerModal
          show={showModal}
          handleClose={() => setShowModal(false)}
          handleSaveCustomer={handleSaveCustomer}
          customerToEdit={
            customerToEdit
              ? {
                  firstName: customerToEdit.name.split(" ")[0],
                  lastName: customerToEdit.name.split(" ")[1],
                  nic: customerToEdit.nic,
                  email: customerToEdit.email,
                  password: " ",
                  role: customerToEdit.role,
                  status: customerToEdit.status,
                }
              : null
          }
          isInProgress={isCreating || isUpdating}
        />
      )}
    </div>
  );
};
export default CustomerManagement;
