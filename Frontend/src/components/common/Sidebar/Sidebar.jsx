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
import { useAuth } from "../../../provider/authProvider";

const Sidebar = () => {
  const location = useLocation();
  const currentPath = location.pathname;
  const [openItems, setOpenItems] = useState({});
  const {
    user: { role },
  } = useAuth();

  const toggleOpen = (index) => {
    setOpenItems((prev) => ({
      ...prev,
      [index]: !prev[index],
    }));
  };

  const navLinks = [
    {
      title: "Dashboard",
      path: "/dashboard",
      icon: <BsGrid />,
      roles: ["admin", "vendor", "csr"],
    },
    {
      title: "Catalog",
      icon: <BsCommand />,
      roles: ["admin", "vendor"],
      subLinks: [
        {
          title: "Products",
          path: "/products",
          icon: <BsGrid3X3Gap />,
          roles: ["admin", "vendor"],
        },
        {
          title: "Categories",
          path: "/categories",
          icon: <BsGrid3X2Gap />,
          roles: ["admin"],
        },
      ],
    },
    {
      title: "Orders",
      path: "/orders",
      icon: <BsBag />,
      roles: ["admin", "vendor"],
    },
    {
      title: "Users",
      icon: <BsPeople />,
      roles: ["admin", "csr"],
      subLinks: [
        {
          title: "Approval Requests",
          path: "/customers/approval-requests",
          icon: <BsPerson />,
          roles: ["admin", "csr"],
        },
        {
          title: "Customers",
          path: "/customers",
          icon: <BsPerson />,
          roles: ["admin", "csr"],
        },
        {
          title: "Vendors",
          path: "/vendors",
          icon: <BsPerson />,
          roles: ["admin"],
        },
        {
          title: "Staff",
          path: "/staff",
          icon: <BsPerson />,
          roles: ["admin"],
        },
      ],
    },
    {
      title: "Inventory",
      path: "/inventory",
      icon: <BsCardList />,
      roles: ["admin", "vendor"],
    },
    {
      title: "Reports",
      path: "/reports",
      icon: <BsClipboardCheck />,
      roles: ["admin"],
    },
  ];

  const filteredNavLinks = navLinks.filter((link) => link.roles.includes(role));

  return (
    <div className="sidebar">
      <div className="sidebarLogo">
        <img src="/logo.png" alt="logo" />
      </div>
      <div className="sidebarContent">
        {filteredNavLinks.map((link, index) => (
          <div key={index}>
            {link.subLinks ? (
              <div className="navItem" onClick={() => toggleOpen(index)}>
                {link.icon}
                <span className="ms-2">{link.title}</span>
                <span className="toggleIcon">
                  {openItems[index] ? <BsCaretUpFill /> : <BsCaretDownFill />}
                </span>
              </div>
            ) : (
              <Link
                to={link.path}
                className={`navItem ${
                  currentPath === link.path ? "active" : ""
                }`}
              >
                {link.icon}
                <span className="ms-2">{link.title}</span>
              </Link>
            )}

            {link.subLinks && openItems[index] && (
              <div className="subLinks">
                {link.subLinks
                  .filter((subLink) => subLink.roles.includes(role))
                  .map((subLink, subIndex) => (
                    <Link
                      to={subLink.path}
                      key={subIndex}
                      className={`navItem subItem ${
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
