/* eslint-disable indent */
import { useState } from "react";
import Button from "react-bootstrap/Button";
import Table from "react-bootstrap/Table";
import styles from "../styles/Pages.module.css";
import Form from "react-bootstrap/Form";
import CommonTitle from "../../components/common/Title/Title";
import {
  useCreateStaffAccount,
  useDeleteStaffAccount,
  useGetAllStaffAccounts,
  useUpdateStaffAccount,
  useUpdateStaffStatus,
} from "../../hooks/staffManagementHooks";
import { MdDelete, MdEdit } from "react-icons/md";
import LoadingTableBody from "../../components/common/TableLoader/TableLoader";
import AddEditStaffModal from "../../components/userManagement/AddEditStaffModal.jsx";
import { useAuth } from "../../provider/authProvider";
import { Alert } from "react-bootstrap";

/*
 * The StaffManagement component displays a list of staff members and allows
 * admins to add, edit, and delete staff accounts.
 */

const StaffManagement = () => {
  const [showModal, setShowModal] = useState(false);
  const [staffToEdit, setStaffToEdit] = useState(null);
  const [search, setSearch] = useState("");

  const {
    user: { id: currentUserId },
  } = useAuth();

  const {
    data: staffAccounts,
    isLoading: isLoadingStaffAccounts,
    isError: isLoadingError,
  } = useGetAllStaffAccounts();

  const { mutate: createStaffAccount, isLoading: isCreating } =
    useCreateStaffAccount();

  const { mutate: updateStaffAccount, isLoading: isUpdating } =
    useUpdateStaffAccount();

  const { mutate: deleteStaffAccount } = useDeleteStaffAccount();

  const { mutate: updateStaffStatus, isPending: isChangingStatus } =
    useUpdateStaffStatus();

  const handleSaveStaff = (staffData) => {
    if (staffToEdit) {
      updateStaffAccount(
        { ...staffData, id: staffToEdit.id },
        {
          onSuccess: () => {
            setShowModal(false);
          },
          onError: () => {
            alert("Failed to update staff");
          },
        }
      );
    } else {
      createStaffAccount(staffData, {
        onSuccess: () => {
          setShowModal(false);
        },
        onError: () => {
          alert("Failed to add staff");
        },
      });
    }
  };

  const handleDeleteStaff = (staffId) => {
    if (window.confirm("Are you sure you want to delete this staff?")) {
      deleteStaffAccount(staffId, {
        onSuccess: () => {
          alert("Staff deleted successfully!");
        },
        onError: () => {
          alert("Failed to delete staff");
        },
      });
    }
  };

  const handleModalOpen = () => {
    setStaffToEdit(null);
    setShowModal(true);
  };

  if (isLoadingError) {
    return (
      <div>
        <CommonTitle title="Staff" />
        <Alert variant="danger">Failed to load staff accounts</Alert>
      </div>
    );
  }

  return (
    <div>
      <CommonTitle
        title="Staff"
        buttonLabel="Add New Staff Account"
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
            <th>Name</th>
            <th>Email</th>
            <th>Role</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        {isLoadingStaffAccounts ? (
          <LoadingTableBody loading={isLoadingStaffAccounts} colSpan="6" />
        ) : (
          <tbody>
            {staffAccounts
              .filter(
                (staff) =>
                  staff.name.toLowerCase().includes(search.toLowerCase()) ||
                  staff.email.toLowerCase().includes(search.toLowerCase())
              )
              .map((staff, index) => (
                <tr key={index}>
                  <td>{index + 1}</td>
                  <td>
                    {staff.name} {staff.id === currentUserId && "(you)"}
                  </td>
                  <td>{staff.email}</td>
                  <td>
                    {staff.role == "admin" ? (
                      <span className="">Admin</span>
                    ) : (
                      <span className="">CSR</span>
                    )}
                  </td>
                  {staff.id !== currentUserId ? (
                    <td>
                      <Form.Check
                        type="switch"
                        id={`custom-switch-${index}`}
                        label=""
                        className="d-flex justify-content-center"
                        checked={staff.status === "Active"}
                        onChange={() =>
                          updateStaffStatus(
                            {
                              id: staff.id,
                              status:
                                staff.status === "Active"
                                  ? "Deactivated"
                                  : "Active",
                            },
                            {
                              onError: () => {
                                alert("Failed to update staff status");
                              },
                            }
                          )
                        }
                        disabled={isChangingStatus}
                      />
                    </td>
                  ) : (
                    <td>
                      <span className="text-muted">-</span>
                    </td>
                  )}

                  {staff.id !== currentUserId ? (
                    <td className={styles.actions}>
                      <Button
                        className={styles.button}
                        onClick={() => {
                          setStaffToEdit(staff);
                          setShowModal(true);
                        }}
                      >
                        <MdEdit />
                        <span className="ms-2">Edit</span>
                      </Button>
                      <Button
                        className={styles.button}
                        onClick={() => handleDeleteStaff(staff.id)}
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
        <AddEditStaffModal
          show={showModal}
          handleClose={() => setShowModal(false)}
          handleSaveStaff={handleSaveStaff}
          staffToEdit={
            staffToEdit
              ? {
                  firstName: staffToEdit.name.split(" ")[0],
                  lastName: staffToEdit.name.split(" ")[1],
                  nic: staffToEdit.nic,
                  email: staffToEdit.email,
                  password: " ",
                  role: staffToEdit.role,
                }
              : null
          }
          isInProgress={isCreating || isUpdating}
        />
      )}
    </div>
  );
};
export default StaffManagement;
