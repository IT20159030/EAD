import { useState } from "react";
import Button from "react-bootstrap/Button";
import Form from "react-bootstrap/Form";
import Table from "react-bootstrap/Table";
import { MdDelete, MdEdit, MdDownload } from "react-icons/md";
import CommonTitle from "../../components/common/Title/Title";
import LoadingTableBody from "../../components/common/TableLoader/TableLoader";
import AutoClosingToast from "../../components/common/Toast/AutoClosingToast";
import AddEditProductModal from "../../components/product/AddEditProductModal";
import { downloadPDF } from "../../utils/downloadPDF";
import styles from "../styles/Pages.module.css";
import { useAuth } from "../../provider/authProvider";
import {
  useGetAllProducts,
  useGetProductsByVendor,
  useCreateProduct,
  useActivateProduct,
  useDeactivateProduct,
  useUpdateProduct,
  useDeleteProduct,
} from "../../hooks/productHooks";
import { useGetAllProductCategories } from "../../hooks/productCategoryHooks";

const Products = () => {
  const { user } = useAuth();
  const [search, setSearch] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [productToEdit, setProductToEdit] = useState(null);
  const [showToast, setShowToast] = useState(false);
  const [toastMessage, setToastMessage] = useState("");
  const [toastType, setToastType] = useState("success");

  const { data: vendorProducts, isLoading: isLoadingVendorProducts } =
    useGetProductsByVendor(user?.id);
  const { data: allProducts, isLoading: isLoadingAllProducts } =
    useGetAllProducts();

  const products = user.role === "vendor" ? vendorProducts : allProducts;

  const isLoadingProducts =
    user.role === "vendor" ? isLoadingVendorProducts : isLoadingAllProducts;

  const { mutate: createProduct, isLoading: isCreating } = useCreateProduct();

  const { mutate: activateProduct, isLoading: isActivating } =
    useActivateProduct();

  const { mutate: deactivateProduct, isLoading: isDeactivating } =
    useDeactivateProduct();

  const { mutate: updateProduct, isLoading: isUpdating } = useUpdateProduct();

  const { mutate: deleteProduct } = useDeleteProduct();

  const { data: categories } = useGetAllProductCategories();

  const handleToast = (message, type = "success") => {
    setToastMessage(message);
    setToastType(type);
    setShowToast(true);
    setTimeout(() => {
      setShowToast(false);
    }, 3000);
  };

  const handleAddProduct = (productData) => {
    createProduct(productData, {
      onSuccess: () => {
        handleToast("Product added successfully!", "success");
        setShowModal(isCreating);
      },
      onError: () => {
        handleToast("Failed to add product", "bg-danger");
      },
    });
  };

  const handleSaveProduct = (productData) => {
    if (productToEdit) {
      updateProduct(
        { id: productToEdit.id, ...productData },
        {
          onSuccess: () => {
            handleToast("Product updated successfully!", "success");
            setShowModal(isUpdating);
          },
          onError: () => {
            handleToast("Failed to update product", "bg-danger");
          },
        }
      );
    } else {
      handleAddProduct(productData);
    }
  };

  const handleDeleteProduct = (productId) => {
    deleteProduct(productId, {
      onSuccess: () => {
        handleToast("Product deleted successfully!", "success");
      },
      onError: () => {
        handleToast("Failed to delete product", "bg-danger");
      },
    });
  };

  const handleModalOpen = () => {
    setProductToEdit(null);
    setShowModal(true);
  };

  const handleDownload = () => {
    // title, columns, data, fileName
    downloadPDF(
      "Products",
      ["ID", "Name", "Category", "Price", "Status"],
      products.map((product, index) => [
        index + 1,
        product.name,
        product.category,
        product.price,
        product.isActive ? "Active" : "Inactive",
      ]),
      "Products.pdf"
    );
  };

  return (
    <div className={styles.container}>
      <CommonTitle
        title="Products"
        buttonLabel="Add New Product"
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
            <th>Product Name</th>
            <th>Category</th>
            <th>Price (Rs.)</th>
            <th>Stock</th>
            <th>Vendor</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        {isLoadingProducts ? (
          <LoadingTableBody loading={isLoadingProducts} colSpan="8" />
        ) : (
          <tbody>
            {products &&
              products
                .filter((product) =>
                  product.name.toLowerCase().includes(search.toLowerCase())
                )
                .map((product, index) => (
                  <tr key={product.id}>
                    <td>{index + 1}</td>
                    <td>{product.name}</td>
                    <td>{product.categoryName}</td>
                    <td>Rs. {product.price}</td>
                    <td>
                      {product.stock <
                      import.meta.env.VITE_LOW_STOCK_THRESHOLD ? (
                        <span className="text-danger fw-bold">
                          {" "}
                          {product.stock}
                        </span>
                      ) : (
                        <span className="text-success fw-bold">
                          {" "}
                          {product.stock}
                        </span>
                      )}
                    </td>
                    <td>{product.vendorName}</td>
                    <td>
                      <div className={styles.switch}>
                        <Form.Check
                          type="switch"
                          id={`custom-switch-${product.id}`}
                          label=""
                          checked={product.isActive}
                          onChange={() =>
                            product.isActive
                              ? deactivateProduct(product.id)
                              : activateProduct(product.id)
                          }
                          disabled={isActivating || isDeactivating}
                        />
                      </div>
                    </td>
                    <td className={styles.actions}>
                      <Button
                        className={styles.button}
                        onClick={() => {
                          setProductToEdit(product);
                          setShowModal(true);
                        }}
                      >
                        <MdEdit />
                        <span className="ms-2">Edit</span>
                      </Button>
                      <Button
                        className={styles.button}
                        onClick={() => handleDeleteProduct(product.id)}
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
        <AddEditProductModal
          show={showModal}
          handleClose={() => setShowModal(false)}
          productToEdit={productToEdit}
          handleSaveProduct={handleSaveProduct}
          isInProgress={isCreating || isUpdating}
          categories={categories}
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

export default Products;
