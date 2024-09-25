import React from "react";
import { Button, Table, Form } from "react-bootstrap";
import { MdDelete, MdEdit, MdDownload } from "react-icons/md";
import CommonTitle from "../../components/common/Title/Title";
import { downloadPDF } from "../../utils/downloadPDF";
import styles from "../styles/Pages.module.css";

const ProductCategory = () => {
  const [search, setSearch] = React.useState("");
  const categories = [
    {
      id: 1,
      name: "Electronics",
    },
    {
      id: 2,
      name: "Clothing",
    },
    {
      id: 3,
      name: "Books",
    },
    {
      id: 4,
      name: "Home & Kitchen",
    },
    {
      id: 5,
      name: "Sports",
    },
  ];

  const handleDownload = () => {
    const columns = ["#", "Category Name"];
    const data = categories.map((category) => [category.id, category.name]);

    downloadPDF("Product Categories Report", columns, data, "report.pdf");
  };

  return (
    <div className={styles.container}>
      <CommonTitle
        title="Product Categories"
        buttonLabel="Add New Category"
        onButtonClick={() => console.log("Add Category clicked")}
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
        <Button
          variant="success"
          className="btn-download"
          onClick={handleDownload}
        >
          <MdDownload className="me-2" /> Download Report
        </Button>
      </div>
      <Table striped hover responsive variant="dark" className={styles.table}>
        <thead>
          <tr>
            <th>#</th>
            <th>Category Name</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {categories.map((category) => (
            <tr key={category.id}>
              <td>{category.id}</td>
              <td>{category.name}</td>
              <td className={styles.action}>
                <Button
                  variant="warning"
                  onClick={() => console.log("Edit", category.id)}
                  className={styles.actionGap}
                >
                  <MdEdit /> Edit
                </Button>
                <Button
                  variant="danger"
                  onClick={() => console.log("Delete", category.id)}
                  className={styles.actionGap}
                >
                  <MdDelete /> Delete
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default ProductCategory;
