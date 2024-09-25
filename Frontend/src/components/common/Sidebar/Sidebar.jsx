import { useState } from "react";
import {
  BsGrid,
  BsCommand,
  BsGrid3X3Gap,
  BsGrid3X2Gap,
  BsBag,
  BsCardList,
  BsClipboardCheck,
  BsPeople,
  BsPerson,
  BsCaretDownFill,
  BsCaretUpFill,
} from "react-icons/bs";
import { Link, useLocation } from "react-router-dom";
import "./Sidebar.css";

const Sidebar = () => {
  const location = useLocation();
  const currentPath = location.pathname;
  const [openItems, setOpenItems] = useState({});

  const toggleOpen = (index) => {
    setOpenItems((prev) => ({
      ...prev,
      [index]: !prev[index],
    }));
  };

  const navLinks = [
    { title: "Dashboard", path: "/dashboard", icon: <BsGrid /> },
    {
      title: "Catalog",
      icon: <BsCommand />,
      subLinks: [
        { title: "Products", path: "/products", icon: <BsGrid3X3Gap /> },
        { title: "Categories", path: "/categories", icon: <BsGrid3X2Gap /> },
      ],
    },
    { title: "Orders", path: "/orders", icon: <BsBag /> },
    {
      title: "Users",
      icon: <BsPeople />,
      subLinks: [
        { title: "Customers", path: "/customers", icon: <BsPerson /> },
        { title: "Vendors", path: "/vendors", icon: <BsPerson /> },
        { title: "Staff", path: "/staff", icon: <BsPerson /> },
      ],
    },
    { title: "Inventory", path: "/inventory", icon: <BsCardList /> },
    { title: "Reports", path: "/reports", icon: <BsClipboardCheck /> },
  ];

  return (
    <div className="sidebar">
      <div className="sidebar-logo">
        <h1>Logo</h1>
      </div>
      <div className="sidebar-content">
        {navLinks.map((link, index) => (
          <div key={index}>
            {link.subLinks ? (
              <div className="nav-item" onClick={() => toggleOpen(index)}>
                {link.icon}
                <span className="ms-2">{link.title}</span>
                <span className="toggle-icon">
                  {openItems[index] ? <BsCaretUpFill /> : <BsCaretDownFill />}
                </span>
              </div>
            ) : (
              <Link
                to={link.path}
                className={`nav-item ${
                  currentPath === link.path ? "active" : ""
                }`}
              >
                {link.icon}
                <span className="ms-2">{link.title}</span>
              </Link>
            )}

            {link.subLinks && openItems[index] && (
              <div className="sub-links">
                {link.subLinks.map((subLink, subIndex) => (
                  <Link
                    to={subLink.path}
                    key={subIndex}
                    className={`nav-item sub-item ${
                      currentPath === subLink.path ? "active" : ""
                    }`}
                  >
                    {subLink.icon}
                    <span className="ms-2">{subLink.title}</span>
                  </Link>
                ))}
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

export default Sidebar;
