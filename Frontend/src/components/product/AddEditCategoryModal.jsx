import { useState, useEffect } from "react";
import Button from "react-bootstrap/Button";
import Modal from "react-bootstrap/Modal";
import Form from "react-bootstrap/Form";
import Spinner from "react-bootstrap/Spinner";
import "./AddEditCategoryModal.css";

const AddEditCategoryModal = ({
  show,
  handleClose,
  categoryToEdit,
  handleSaveCategory,
  isInProgress,
}) => {
  const [categoryName, setCategoryName] = useState("");

  useEffect(() => {
    if (categoryToEdit) {
      setCategoryName(categoryToEdit.name);
    } else {
      setCategoryName("");
    }
  }, [categoryToEdit]);

  const handleSubmit = () => {
    const categoryData = { name: categoryName };
    handleSaveCategory(categoryData);
    handleClose();
  };

  return (
    <Modal show={show} onHide={handleClose} className="darkModal" centered>
      <Modal.Header closeButton className="modalHeader">
        <Modal.Title>
          {categoryToEdit ? "Edit" : "Add"} Product Category
        </Modal.Title>
      </Modal.Header>

      <Modal.Body className="modalBody">
        <Form>
          <Form.Group controlId="categoryName">
            <Form.Label>Category Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter category name"
              value={categoryName}
              onChange={(e) => setCategoryName(e.target.value)}
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
              {categoryToEdit ? "Update" : "Add"} Category
            </Button>
          </>
        )}
      </Modal.Footer>
    </Modal>
  );
};

export default AddEditCategoryModal;
