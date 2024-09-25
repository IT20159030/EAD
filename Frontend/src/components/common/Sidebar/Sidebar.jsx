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
import styles from "./Sidebar.module.css";

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
    <div className={styles.sidebar}>
      <div className={styles.sidebarLogo}>
        <h1>Logo</h1>
      </div>
      <div className={styles.sidebarContent}>
        {navLinks.map((link, index) => (
          <div key={index}>
            {link.subLinks ? (
              <div className={styles.navItem} onClick={() => toggleOpen(index)}>
                {link.icon}
                <span className={styles.ms2}>{link.title}</span>
                <span className={styles.toggleIcon}>
                  {openItems[index] ? <BsCaretUpFill /> : <BsCaretDownFill />}
                </span>
              </div>
            ) : (
              <Link
                to={link.path}
                className={`${styles.navItem} ${
                  currentPath === link.path ? styles.active : ""
                }`}
              >
                {link.icon}
                <span className={styles.ms2}>{link.title}</span>
              </Link>
            )}

            {link.subLinks && openItems[index] && (
              <div className={styles.subLinks}>
                {link.subLinks.map((subLink, subIndex) => (
                  <Link
                    to={subLink.path}
                    key={subIndex}
                    className={`${styles.navItem} ${styles.subItem} ${
                      currentPath === subLink.path ? styles.active : ""
                    }`}
                  >
                    {subLink.icon}
                    <span className={styles.ms2}>{subLink.title}</span>
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
