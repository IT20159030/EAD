import { useState, useEffect } from "react";
import Button from "react-bootstrap/Button";
import Modal from "react-bootstrap/Modal";
import Form from "react-bootstrap/Form";
import Spinner from "react-bootstrap/Spinner";
import { openCloudinaryWidget } from "../../utils/uploadFile";
import "./AddEditModal.css";

const AddEditProductModal = ({
  show,
  handleClose,
  productToEdit,
  handleSaveProduct,
  isInProgress,
  categories,
}) => {
  const [productName, setProductName] = useState("");
  const [productImage, setProductImage] = useState("");
  const [productCategory, setProductCategory] = useState("");
  const [productDescription, setProductDescription] = useState("");
  const [productPrice, setProductPrice] = useState("");
  const [productStatus, setProductStatus] = useState(false);
  const [productVendorId, setProductVendorId] = useState(
    "14d6470c-5bf5-466d-9178-6efa2fa9d364"
  );

  useEffect(() => {
    if (productToEdit) {
      setProductName(productToEdit.name);
      setProductCategory(productToEdit.category);
      setProductDescription(productToEdit.description);
      setProductPrice(productToEdit.price);
      setProductStatus(productToEdit.isActive);
      setProductVendorId(productToEdit.vendorId);
      setProductImage(productToEdit.image);
    } else {
      resetForm();
    }
  }, [productToEdit]);

  const resetForm = () => {
    setProductName("");
    setProductCategory("");
    setProductDescription("");
    setProductPrice("");
    setProductStatus(false);
    setProductVendorId("");
    setProductImage(null);
  };

  const handleSubmit = () => {
    const productData = {
      name: productName,
      image: productImage,
      category: productCategory,
      description: productDescription,
      price: productPrice,
      isActive: productStatus,
      vendorId: productVendorId,
    };

    handleSaveProduct(productData);
    handleClose();
  };

  return (
    <Modal show={show} onHide={handleClose} className="darkModal" centered>
      <Modal.Header closeButton className="modalHeader">
        <Modal.Title>{productToEdit ? "Edit" : "Add"} Product</Modal.Title>
      </Modal.Header>

      <Modal.Body className="modalBody">
        <Form>
          <Form.Group controlId="productName">
            <Form.Label>Product Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter product name"
              value={productName}
              onChange={(e) => setProductName(e.target.value)}
            />
          </Form.Group>

          <Form.Group controlId="productImage" className="mt-3">
            <Form.Label>Product Image</Form.Label>
            <div className="imageUploadContainer">
              <Button
                variant="primary"
                onClick={() =>
                  openCloudinaryWidget((image) => setProductImage(image))
                }
              >
                {productImage ? "Change Image" : "Upload Image"}
              </Button>
              {productImage && (
                <img
                  src={productImage}
                  alt="Product"
                  className="productImage"
                />
              )}
            </div>
          </Form.Group>

          <Form.Group controlId="productCategory" className="mt-3">
            <Form.Label>Product Category</Form.Label>
            <Form.Control
              as="select"
              value={productCategory}
              onChange={(e) => setProductCategory(e.target.value)}
            >
              <option value="">Select a category</option>
              {categories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
            </Form.Control>
          </Form.Group>

          <Form.Group controlId="productDescription" className="mt-3">
            <Form.Label>Product Description</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              placeholder="Enter description"
              value={productDescription}
              onChange={(e) => setProductDescription(e.target.value)}
            />
          </Form.Group>

          <Form.Group controlId="productPrice" className="mt-3">
            <Form.Label>Price</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter price"
              value={productPrice}
              onChange={(e) => {
                const value = e.target.value;
                if (/^\d*\.?\d*$/.test(value)) {
                  setProductPrice(value);
                }
              }}
              step="0.01"
            />
          </Form.Group>

          <Form.Group controlId="productVendorId" className="mt-3">
            <Form.Label>Vendor ID</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter vendor ID"
              value={productVendorId}
              onChange={(e) => setProductVendorId(e.target.value)}
            />
          </Form.Group>
        </Form>
      </Modal.Body>

      <Modal.Footer className="modalFooter">
        {isInProgress ? (
          <Spinner animation="border" variant="primary" />
        ) : (
          <>
            <Button variant="secondary" onClick={handleClose}>
              Close
            </Button>
            <Button variant="primary" onClick={handleSubmit}>
              {productToEdit ? "Update" : "Add"} Product
            </Button>
          </>
        )}
      </Modal.Footer>
    </Modal>
  );
};

export default AddEditProductModal;
