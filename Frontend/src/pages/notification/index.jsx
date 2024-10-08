/* eslint-disable react-hooks/rules-of-hooks */
import { useState, useEffect } from "react";
import {
  Button,
  Form,
  Table,
  Pagination,
  ButtonGroup,
  ToggleButton,
} from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import CommonTitle from "../../components/common/Title/Title";
import LoadingTableBody from "../../components/common/TableLoader/TableLoader";
import { MdDelete, MdDownload } from "react-icons/md";
import {
  useGetAllNotifications,
  useGetNotificationByRecipientId,
  useDeleteNotification,
  useMarkAsRead,
} from "../../hooks/notificationHooks";
import { downloadPDF } from "../../utils/downloadPDF";
import { confirmDeletion } from "../../utils/helper";
import styles from "../styles/Pages.module.css";
import { useAuth } from "../../provider/authProvider";

/**
 * Notifications page component
 * shows all notifications based on user role
 * admin - all notifications
 * vendor - notifications for vendor
 * csr - notifications for csr
 *
 */

const Notifications = () => {
  const [search, setSearch] = useState("");
  const [showToast, setShowToast] = useState(false);
  const [toastMessage, setToastMessage] = useState("");
  const [toastType, setToastType] = useState("success");
  const [currentPage, setCurrentPage] = useState(1);
  const [radioValue, setRadioValue] = useState(0);
  const notificationsPerPage = 10;

  const { user } = useAuth();
  const navigate = useNavigate();
  const {
    data: notifications,
    isLoading: isLoadingNotifications,
    refetch,
  } = user.role === "vendor"
    ? useGetNotificationByRecipientId(user.id)
    : useGetAllNotifications();

  const { mutate: deleteNotification } = useDeleteNotification();
  const { mutate: markAsRead } = useMarkAsRead();

  useEffect(() => {
    refetch();
  }, [refetch]);

  const handleToast = (message, type = "success") => {
    setToastMessage(message);
    setToastType(type);
    setShowToast(true);
    setTimeout(() => {
      setShowToast(false);
    }, 3000);
  };

  const handleDeleteNotification = (notificationId) => {
    confirmDeletion().then((result) => {
      if (result.isConfirmed) {
        deleteNotification(notificationId, {
          onSuccess: () => {
            refetch();
          },
          onError: () => {
            handleToast("Failed to delete notification", "bg-danger");
          },
        });
        Swal.fire("Deleted!", "Your notification has been deleted.", "success");
      }
    });
  };

  const handleDownloadReport = () => {
    downloadPDF(
      "Notifications Report",
      ["ID", "Message", "Date", "Type", "Status"],
      notifications.map((notification, index) => [
        index + 1,
        notification.message,
        new Date(notification.createdAt).toLocaleString(),
        notification.type,
        notification.isRead ? "Read" : "Unread",
      ]),
      "Notifications_Report.pdf"
    );
  };

  const handleMessageClick = (notification) => {
    if (!notification.isRead) {
      markAsRead(notification.id, {
        onSuccess: () => {
          refetch();
        },
        onError: () => {
          handleToast("Failed to mark notification as read", "bg-danger");
        },
      });
    }

    switch (notification.type) {
      case "LowStock":
        navigate("/products");
        break;
      case "AccountApproval":
        navigate(`/customers/approval-requests/${notification.recipientId}`);
        break;
      case "AccountActivated":
        navigate("/customers");
        break;
      case "OrderStatus":
        navigate("/orders");
        break;
      default:
        navigate("/products");
        break;
    }
  };

  const roleBasedFilterOptions = {
    admin: [
      { id: 0, label: "All" },
      { id: 1, label: "Account Approval" },
      { id: 2, label: "Account Activated" },
    ],
    csr: [
      { id: 0, label: "All" },
      { id: 1, label: "Account Approval" },
      { id: 2, label: "Account Activated" },
      { id: 4, label: "Order Status" },
      { id: 3, label: "Low Stock" },
    ],
    vendor: [
      { id: 0, label: "All" },
      { id: 4, label: "Order Status" },
      { id: 3, label: "Low Stock" },
    ],
  };

  const handleButtonGroupChange = (selectedIndex) => {
    setRadioValue(selectedIndex);
  };

  const filteredNotifications = notifications
    ?.filter((notification) => {
      if (radioValue === 0) return true;

      const notificationTypeMap = {
        1: "AccountApproval",
        2: "AccountActivated",
        3: "LowStock",
        4: "OrderStatus",
      };

      return notification.type === notificationTypeMap[radioValue];
    })
    .filter((notification) =>
      notification.message.toLowerCase().includes(search.toLowerCase())
    );

  const totalNotifications = filteredNotifications?.length || 0;
  const totalPages = Math.ceil(totalNotifications / notificationsPerPage);
  const indexOfLastNotification = currentPage * notificationsPerPage;
  const indexOfFirstNotification =
    indexOfLastNotification - notificationsPerPage;
  const currentNotifications = filteredNotifications?.slice(
    indexOfFirstNotification,
    indexOfLastNotification
  );

  return (
    <div className={styles.container}>
      <CommonTitle title="Notifications" />

      <ButtonGroup className="mb-3">
        {roleBasedFilterOptions[user.role]?.map((option) => (
          <ToggleButton
            key={option.id}
            type="radio"
            variant="outline-primary"
            name="radio"
            value={option.id}
            checked={radioValue === option.id}
            onClick={() => handleButtonGroupChange(option.id)}
          >
            {option.label}
          </ToggleButton>
        ))}
      </ButtonGroup>

      <div className={styles.pageActions}>
        <Form className={`${styles.controls}`}>
          <Form.Control
            className={styles.search}
            type="text"
            placeholder="Search notifications..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </Form>
        <Button
          variant="primary"
          className={styles.button}
          onClick={handleDownloadReport}
          disabled={isLoadingNotifications || !notifications?.length}
        >
          <MdDownload className="me-2" /> Download Report
        </Button>
      </div>

      <Table striped hover responsive className={styles.table}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Message</th>
            <th>Date</th>
            <th>Type</th>
            <th>Status</th>
            <th className={styles.actions}>Actions</th>
          </tr>
        </thead>
        {isLoadingNotifications ? (
          <LoadingTableBody loading={isLoadingNotifications} colSpan="6" />
        ) : (
          <tbody>
            {currentNotifications &&
              currentNotifications.map((notification, index) => (
                <tr key={notification.id}>
                  <td>{index + 1 + indexOfFirstNotification}</td>
                  <td>
                    <span
                      className={styles.notificationLink}
                      style={{ cursor: "pointer", color: "#007bff" }}
                      onClick={() => handleMessageClick(notification)}
                    >
                      {notification.message}
                    </span>
                  </td>
                  <td>{new Date(notification.createdAt).toLocaleString()}</td>
                  <td>
                    {notification.type.replace(/([a-z])([A-Z])/g, "$1 $2")}
                  </td>
                  <td>{notification.isRead ? "Read" : "Unread"}</td>
                  <td className={styles.actions}>
                    <Button
                      className={styles.deleteButton}
                      onClick={() => handleDeleteNotification(notification.id)}
                    >
                      <MdDelete />
                    </Button>
                  </td>
                </tr>
              ))}
          </tbody>
        )}
      </Table>

      <Pagination className="justify-content-center">
        <Pagination.Prev
          onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
          disabled={currentPage === 1}
        />
        {Array.from({ length: totalPages }, (_, i) => (
          <Pagination.Item
            key={i + 1}
            active={i + 1 === currentPage}
            onClick={() => setCurrentPage(i + 1)}
          >
            {i + 1}
          </Pagination.Item>
        ))}
        <Pagination.Next
          onClick={() =>
            setCurrentPage((prev) => Math.min(prev + 1, totalPages))
          }
          disabled={currentPage === totalPages}
        />
      </Pagination>
    </div>
  );
};

export default Notifications;
