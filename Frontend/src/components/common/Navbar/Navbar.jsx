import { useState, useRef } from "react";
import {
  BsBell,
  BsPersonCircle,
  BsGear,
  BsBoxArrowRight,
} from "react-icons/bs";
import Overlay from "react-bootstrap/Overlay";
import { Link } from "react-router-dom";
import "./Navbar.css";

const Navbar = () => {
  const [showNotifications, setShowNotifications] = useState(false);
  const [showAvatarMenu, setShowAvatarMenu] = useState(false);
  const notificationRef = useRef(null);
  const avatarRef = useRef(null);

  // TODO: Replace with real data
  const notifications = [
    {
      id: 1,
      title: "New Order",
      message: "You have a new order from John Doe",
      link: "/orders/123",
    },
    {
      id: 2,
      title: "Low Stock",
      message: "Product X is running low on stock",
      link: "/products/456",
    },
    {
      id: 3,
      title: "New Message",
      message: "You have a new message from Jane Doe",
      link: "/messages/789",
    },
  ];

  const avatarMenuItems = [
    {
      title: "Profile",
      icon: <BsPersonCircle />,
      link: "/profile",
    },
    {
      title: "Settings",
      icon: <BsGear />,
      link: "/settings",
    },
    {
      title: "Logout",
      icon: <BsBoxArrowRight />,
      action: () => {
        console.log("Logging out...");
      },
    },
  ];

  const name = "Jane Smith";
  const role = "Admin";

  const NotificationOverlay = () => (
    <div className="notification-overlay">
      <h6>Notifications</h6>
      {notifications.map((notification) => (
        <div key={notification.id} className="notification-item">
          <Link
            to={notification.link}
            onClick={() => setShowNotifications(false)}
            className="notification-link"
          >
            <strong>{notification.title}</strong>
            <p>{notification.message}</p>
          </Link>
        </div>
      ))}
    </div>
  );

  const AvatarMenuOverlay = () => (
    <div className="avatar-menu-overlay">
      {avatarMenuItems.map((item, index) => (
        <div
          key={index}
          className="avatar-menu-item"
          onClick={item.action ? item.action : () => setShowAvatarMenu(false)}
        >
          <Link to={item.link} className="avatar-link">
            {item.icon}
            <span className="item-title">{item.title}</span>
          </Link>
        </div>
      ))}
    </div>
  );

  return (
    <nav className="navbar">
      <div className="navbar-brand"></div>
      <div className="navbar-actions">
        <div
          ref={notificationRef}
          onClick={() => {
            setShowNotifications(!showNotifications);
            setShowAvatarMenu(false);
          }}
        >
          <BsBell className="icon" />
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
          <BsPersonCircle className="icon" />{" "}
          {/* TODO: Replace with profile picture */}
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
