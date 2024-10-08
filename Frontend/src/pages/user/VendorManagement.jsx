/* eslint-disable indent */
import { useState } from "react";
import Button from "react-bootstrap/Button";
import Table from "react-bootstrap/Table";
import styles from "../styles/Pages.module.css";
import Form from "react-bootstrap/Form";
import CommonTitle from "../../components/common/Title/Title.jsx";
import {
  useCreateVendorAccount,
  useDeleteVendorAccount,
  useGetAllVendorAccounts,
  useUpdateVendorAccount,
  useUpdateVendorStatus,
} from "../../hooks/vendorManagementHooks";
import { MdDelete, MdEdit } from "react-icons/md";
import LoadingTableBody from "../../components/common/TableLoader/TableLoader";
import AddEditVendorModal from "../../components/userManagement/AddEditVendorModal";
import { useAuth } from "../../provider/authProvider.jsx";
import { Alert } from "react-bootstrap";

/*
 * The VendorManagement component displays a list of vendors and allows admins to add, edit, and delete vendors.
 */
const VendorManagement = () => {
  const [showModal, setShowModal] = useState(false);
  const [vendorToEdit, setVendorToEdit] = useState(null);
  const [search, setSearch] = useState("");

  const {
    user: { id: currentUserId },
  } = useAuth();

  const {
    data: vendorAccounts,
    isLoading: isLoadingVendorAccounts,
    isError: isLoadingError,
  } = useGetAllVendorAccounts();

  // const { id, vendorDetails, vendorAccountDetails } = vendorAccounts;

  const { mutate: createVendorAccount, isLoading: isCreating } =
    useCreateVendorAccount();

  const { mutate: updateVendorAccount, isLoading: isUpdating } =
    useUpdateVendorAccount();

  const { mutate: deleteVendorAccount } = useDeleteVendorAccount();

  const { mutate: updateVendorStatus, isPending: isChangingStatus } =
    useUpdateVendorStatus();

  const handleSaveVendor = (vendorData) => {
    if (vendorToEdit) {
      updateVendorAccount(
        { ...vendorData, id: vendorToEdit.id },
        {
          onSuccess: () => {
            setShowModal(false);
          },
          onError: () => {
            alert("Failed to update vendor");
          },
        }
      );
    } else {
      createVendorAccount(vendorData, {
        onSuccess: () => {
          setShowModal(false);
        },
        onError: () => {
          alert("Failed to add vendor");
        },
      });
    }
  };

  const handleDeleteVendor = (vendorId) => {
    if (window.confirm("Are you sure you want to delete this vendor?")) {
      deleteVendorAccount(vendorId, {
        onSuccess: () => {
          alert("Vendor deleted successfully!");
        },
        onError: () => {
          alert("Failed to delete vendor");
        },
      });
    }
  };

  const handleModalOpen = () => {
    setVendorToEdit(null);
    setShowModal(true);
  };

  if (isLoadingError) {
    return (
      <div>
        <CommonTitle title="Vendors" />
        <Alert variant="danger">Failed to load vendor accounts</Alert>
      </div>
    );
  }

  return (
    <div>
      <CommonTitle
        title="Vendors"
        buttonLabel="Add New Vendor"
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
      </div>
      <Table striped hover responsive className={styles.table}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Store Name</th>
            <th>Account name</th>
            <th>Account email</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        {isLoadingVendorAccounts ? (
          <LoadingTableBody loading={isLoadingVendorAccounts} colSpan="6" />
        ) : (
          <tbody>
            {vendorAccounts
              .filter(
                (vendor) =>
                  vendor.vendorAccountDetails.name
                    .toLowerCase()
                    .includes(search.toLowerCase()) ||
                  vendor.vendorAccountDetails.email
                    .toLowerCase()
                    .includes(search.toLowerCase()) ||
                  vendor.vendorDetails.vendorName
                    .toLowerCase()
                    .includes(search.toLowerCase())
              )
              .map((vendor, index) => (
                <tr key={index}>
                  <td>{index + 1}</td>
                  <td>{vendor.vendorDetails.vendorName}</td>
                  <td>
                    {vendor.vendorAccountDetails.name}{" "}
                    {vendor.id === currentUserId && "(you)"}
                  </td>
                  <td>{vendor.vendorAccountDetails.email}</td>
                  {vendor.id !== currentUserId ? (
                    <td>
                      <Form.Check
                        type="switch"
                        id={`custom-switch-${index}`}
                        label=""
                        className="d-flex justify-content-center"
                        checked={
                          vendor.vendorAccountDetails.status === "Active"
                        }
                        onChange={() =>
                          updateVendorStatus({
                            id: vendor.id,
                            status:
                              vendor.vendorAccountDetails.status === "Active"
                                ? "Deactivated"
                                : "Active",
                          })
                        }
                        disabled={isChangingStatus}
                      />
                    </td>
                  ) : (
                    <td>
                      <span className="text-muted">-</span>
                    </td>
                  )}
                  {vendor.id !== currentUserId ? (
                    <td className={styles.actions}>
                      <Button
                        className={styles.button}
                        onClick={() => {
                          setVendorToEdit(vendor);
                          setShowModal(true);
                        }}
                      >
                        <MdEdit />
                        <span className="ms-2">Edit</span>
                      </Button>
                      <Button
                        className={styles.button}
                        onClick={() => handleDeleteVendor(vendor.id)}
                      >
                        <MdDelete />
                        <span className="ms-2">Delete</span>
                      </Button>
                    </td>
                  ) : (
                    <td>
                      <span className="text-muted">-</span>
                    </td>
                  )}
                </tr>
              ))}
          </tbody>
        )}
      </Table>

      {showModal && (
        <AddEditVendorModal
          show={showModal}
          handleClose={() => setShowModal(false)}
          handleSaveVendor={handleSaveVendor}
          vendorToEdit={vendorToEdit}
          isInProgress={isCreating || isUpdating}
        />
      )}
    </div>
  );
};
export default VendorManagement;
