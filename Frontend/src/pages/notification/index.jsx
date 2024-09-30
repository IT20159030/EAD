import { useState, useEffect } from "react";
import { Button, Form, Table } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import CommonTitle from "../../components/common/Title/Title";
import LoadingTableBody from "../../components/common/TableLoader/TableLoader";
import AutoClosingToast from "../../components/common/Toast/AutoClosingToast";
import { MdDelete, MdDownload } from "react-icons/md";
import {
  useGetAllNotifications,
  useDeleteNotification,
} from "../../hooks/notificationHooks";
import { downloadPDF } from "../../utils/downloadPDF";
import { confirmDeletion } from "../../utils/helper";
import styles from "../styles/Pages.module.css";

const Notifications = () => {
  const [search, setSearch] = useState("");
  const [showToast, setShowToast] = useState(false);
  const [toastMessage, setToastMessage] = useState("");
  const [toastType, setToastType] = useState("success");

  const navigate = useNavigate();
  const {
    data: notifications,
    isLoading: isLoadingNotifications,
    refetch,
  } = useGetAllNotifications();
  const { mutate: deleteNotification } = useDeleteNotification();

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
            handleToast("Notification deleted successfully!", "success");
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

  return (
    <div className={styles.container}>
      <CommonTitle title="Notifications" />

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
            {notifications &&
              notifications
                .filter((notification) =>
                  notification.message
                    .toLowerCase()
                    .includes(search.toLowerCase())
                )
                .map((notification, index) => (
                  <tr key={notification.id}>
                    <td>{index + 1}</td>
                    <td>
                      <span
                        className={styles.notificationLink}
                        style={{ cursor: "pointer", color: "#007bff" }}
                        onClick={() =>
                          navigate(
                            notification.type === "LowStock"
                              ? `/inventory/${notification.messageID}`
                              : `/orders/${notification.messageID}`
                          )
                        }
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
                        onClick={() =>
                          handleDeleteNotification(notification.id)
                        }
                      >
                        <MdDelete />
                      </Button>
                    </td>
                  </tr>
                ))}
          </tbody>
        )}
      </Table>

      {showToast && (
        <AutoClosingToast
          title="Notification"
          description={toastMessage}
          type={toastType}
        />
      )}
    </div>
  );
};

export default Notifications;
