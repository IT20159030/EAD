/* eslint-disable react-hooks/rules-of-hooks */
import { useState, useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import {
  BsBell,
  BsPersonCircle,
  BsGear,
  BsBoxArrowRight,
} from "react-icons/bs";
import { MdNotificationsActive } from "react-icons/md";
import Overlay from "react-bootstrap/Overlay";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import {
  useGetAllNotifications,
  useMarkAsRead,
  useGetNotificationByRecipientId,
} from "../../../hooks/notificationHooks";
import { useAuth } from "../../../provider/authProvider";
import "./Navbar.css";

const Navbar = () => {
  const [showNotifications, setShowNotifications] = useState(false);
  const [showAvatarMenu, setShowAvatarMenu] = useState(false);
  const notificationRef = useRef(null);
  const avatarRef = useRef(null);
  const [connection, setConnection] = useState(null);
  const { user } = useAuth();
  const userRole = user?.role;

  const { data: notifications, refetch: refetchNotifications } =
    userRole === "vendor"
      ? useGetNotificationByRecipientId(user?.id)
      : useGetAllNotifications();

  const { mutate: markNotificationAsRead } = useMarkAsRead();

  useEffect(() => {
    const connectSignalR = async () => {
      try {
        const connection = new HubConnectionBuilder()
          .withUrl(import.meta.env.VITE_BACKEND_URL + "/notificationHub")
          .configureLogging(LogLevel.Information)
          .build();

        await connection.start();
        console.log("SignalR Connected");

        connection.on("ReceiveNotification", () => {
          refetchNotifications();
        });

        setConnection(connection);
      } catch (err) {
        console.error("Error connecting to SignalR:", err);
      }
    };

    connectSignalR();

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, []);

  const handleNotificationClick = () => {
    setShowNotifications(!showNotifications);
    if (!showNotifications) {
      refetchNotifications();
    }
    setShowAvatarMenu(false);
  };

  const name = user.name;
  const role = user.role;

  const NotificationOverlay = () => (
    <div className="notificationOverlay">
      <div className="notificationHeader">
        <h6 className="text-white">Notifications</h6>
        <Link to="/notifications" className="seeAllLink">
          See All
        </Link>
      </div>
      {notifications?.length > 0 ? (
        notifications
          .filter(
            (notification) =>
              !notification.isRead &&
              !(
                (user.role === "admin" && notification.type === "LowStock") ||
                ((user.role === "csr" || user.role === "admin") &&
                  notification.type === "OrderStatus")
              )
          )
          .slice(0, 5)
          .map((notification) => (
            <div key={notification.id} className="notificationItem">
              {notification.type === "AccountActivated" ? (
                <div
                  onClick={() => {
                    setShowNotifications(false);
                    markNotificationAsRead(notification.id);
                  }}
                  className="notificationLink"
                >
                  <div className="notificationContent">
                    <p className="message">{notification.message}</p>
                    <div className="readIcon">
                      {!notification.isRead && (
                        <MdNotificationsActive
                          onClick={() =>
                            markNotificationAsRead(notification.id)
                          }
                        />
                      )}
                    </div>
                  </div>
                </div>
              ) : (
                <Link
                  to={
                    notification.type === "LowStock"
                      ? "/products"
                      : notification.type === "AccountApproval"
                      ? "/staff"
                      : notification.type === "OrderStatus"
                      ? "/orders"
                      : "/notifications"
                  }
                  onClick={() => {
                    setShowNotifications(false);
                    markNotificationAsRead(notification.id);
                  }}
                  className="notificationLink"
                >
                  <div className="notificationContent">
                    <p className="message">{notification.message}</p>
                    <div className="readIcon">
                      {!notification.isRead && (
                        <MdNotificationsActive
                          onClick={() =>
                            markNotificationAsRead(notification.id)
                          }
                        />
                      )}
                    </div>
                  </div>
                </Link>
              )}
            </div>
          ))
      ) : (
        <p>No new notifications</p>
      )}
    </div>
  );

  const AvatarMenuOverlay = () => (
    <div className="avatarMenuOverlay">
      {[
        { title: "Profile", icon: <BsPersonCircle />, link: "/profile" },
        { title: "Settings", icon: <BsGear />, link: "/settings" },
        {
          title: "Logout",
          icon: <BsBoxArrowRight />,
          link: "/logout",
        },
      ].map((item, index) => (
        <div
          key={index}
          className="avatarMenuItem"
          onClick={item.action ? item.action : () => setShowAvatarMenu(false)}
        >
          <Link to={item.link} className="avatarLink">
            {item.icon}
            <span className="itemTitle">{item.title}</span>
          </Link>
        </div>
      ))}
    </div>
  );

  return (
    <nav className="navbar">
      <div></div>
      <div className="navbarActions">
        <div
          ref={notificationRef}
          onClick={handleNotificationClick}
          className="notificationIcon"
        >
          {notifications?.some((notification) => !notification.isRead) ? (
            <>
              <MdNotificationsActive className="icon" />
              <span className="notificationDot" />
            </>
          ) : (
            <BsBell className="icon" />
          )}
        </div>
        <Overlay
          target={notificationRef.current}
          show={showNotifications}
          placement="bottom"
        >
          <NotificationOverlay />
        </Overlay>
        <div
          ref={avatarRef}
          onClick={() => {
            setShowAvatarMenu(!showAvatarMenu);
            setShowNotifications(false);
          }}
        >
          <BsPersonCircle className="icon" />
        </div>
        <Overlay
          target={avatarRef.current}
          show={showAvatarMenu}
          placement="bottom"
        >
          <AvatarMenuOverlay />
        </Overlay>
        <div className="details">
          <div className="name">{name}</div>
          <div className="role">{role}</div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
