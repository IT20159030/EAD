import { useState } from "react";
import Button from "react-bootstrap/Button";
import Form from "react-bootstrap/Form";
import Table from "react-bootstrap/Table";
import { MdDelete, MdEdit, MdDownload } from "react-icons/md";
import CommonTitle from "../../components/common/Title/Title";
import LoadingTableBody from "../../components/common/TableLoader/TableLoader"; // Use your TableLoader for the body
import AutoClosingToast from "../../components/common/Toast/AutoClosingToast";
import AddEditCategoryModal from "../../components/product/AddEditCategoryModal";
import { downloadPDF } from "../../utils/downloadPDF";
import styles from "../styles/Pages.module.css";
import {
  useGetAllProductCategories,
  useCreateProductCategory,
  useUpdateProductCategory,
  useDeleteProductCategory,
} from "../../hooks/productCategoryHooks";

const ProductCategory = () => {
  const [search, setSearch] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [categoryToEdit, setCategoryToEdit] = useState(null);
  const [showToast, setShowToast] = useState(false);
  const [toastMessage, setToastMessage] = useState("");
  const [toastType, setToastType] = useState("s"); // Control the toast type

  const { data: categories, isLoading: isLoadingProductCategories } =
    useGetAllProductCategories();

  const { mutate: createProductCategory, isLoading: isCreating } =
    useCreateProductCategory();

  const { mutate: updateProductCategory, isLoading: isUpdating } =
    useUpdateProductCategory();

  const { mutate: deleteProductCategory } = useDeleteProductCategory();

  const handleToast = (message, type = "success") => {
    setToastMessage(message);
    setToastType(type);
    setShowToast(true);
    setTimeout(() => {
      setShowToast(false);
    }, 3000);
  };

  const handleAddCategory = (categoryData) => {
    createProductCategory(categoryData, {
      onSuccess: () => {
        handleToast("Category added successfully!", "success");
        setShowModal(isCreating);
      },
      onError: () => {
        handleToast("Failed to add category", "bg-danger");
      },
    });
  };

  const handleSaveCategory = (categoryData) => {
    if (categoryToEdit) {
      updateProductCategory(
        { id: categoryToEdit.id, ...categoryData },
        {
          onSuccess: () => {
            handleToast("Category updated successfully!", "success");
            setShowModal(isUpdating);
          },
          onError: () => {
            handleToast("Failed to update category", "bg-danger");
          },
        }
      );
    } else {
      handleAddCategory(categoryData);
    }
  };

  const handleDeleteCategory = (categoryId) => {
    deleteProductCategory(categoryId, {
      onSuccess: () => {
        handleToast("Category deleted successfully!", "success");
      },
      onError: () => {
        handleToast("Failed to delete category", "bg-danger");
      },
    });
  };

  const handleModalOpen = () => {
    setCategoryToEdit(null);
    setShowModal(true);
  };

  const handleDownload = () => {
    // title, columns, data, fileName
    downloadPDF(
      "Product Categories",
      ["ID", "Category Name"],
      categories.map((category, index) => [index + 1, category.name]),
      "Product categories.pdf"
    );
  };

  return (
    <div className={styles.container}>
      <CommonTitle
        title="Product Categories"
        buttonLabel="Add New Category"
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
        <Button
          variant="primary"
          className={styles.button}
          onClick={handleDownload}
        >
          <MdDownload className="me-2" /> Download Report
        </Button>
      </div>
      <Table striped hover responsive className={styles.table}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Category Name</th>
            <th>Actions</th>
          </tr>
        </thead>
        {isLoadingProductCategories ? (
          <LoadingTableBody loading={isLoadingProductCategories} />
        ) : (
          <tbody>
            {categories &&
              categories
                .filter((category) =>
                  category.name.toLowerCase().includes(search.toLowerCase())
                )
                .map((category, index) => (
                  <tr key={category.id}>
                    <td>{index + 1}</td>
                    <td>{category.name}</td>
                    <td className={styles.actions}>
                      <Button
                        className={styles.button}
                        onClick={() => {
                          setCategoryToEdit(category);
                          setShowModal(true);
                        }}
                      >
                        <MdEdit />
                        <span className="ms-2">Edit</span>
                      </Button>
                      <Button
                        className={styles.button}
                        onClick={() => handleDeleteCategory(category.id)}
                      >
                        <MdDelete />
                        <span className="ms-2">Delete</span>
                      </Button>
                    </td>
                  </tr>
                ))}
          </tbody>
        )}
      </Table>

      {showModal && (
        <AddEditCategoryModal
          show={showModal}
          handleClose={() => setShowModal(false)}
          handleSaveCategory={handleSaveCategory}
          categoryToEdit={categoryToEdit}
        />
      )}

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

export default ProductCategory;
