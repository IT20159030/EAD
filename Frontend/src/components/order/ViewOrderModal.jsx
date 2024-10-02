import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import Form from 'react-bootstrap/Form';
import Spinner from 'react-bootstrap/Spinner';

const ViewOrderModal = ({ show, handleClose, orderDetails }) => {
  const [orderItems, setOrderItems] = useState([]);

  useEffect(() => {
    if (orderDetails) {
      setOrderItems(orderDetails.orderItems);
    }
  }, [orderDetails]);

  return (
    <Modal show={show} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Order Details</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <h5>Order ID: {orderDetails?.id}</h5>
        <h5>Vendor ID: {orderDetails?.vendorId}</h5>
        <h5>Order Status: {orderDetails?.status}</h5>
        <h5>Order Items:</h5>
        <ul>
          {orderItems.map((item) => (
            <li key={item.id}>
              {item.productName} - {item.quantity}
            </li>
          ))}
        </ul>
      </Modal.Body>
      <Modal.Footer>
        <Button variant='secondary' onClick={handleClose}>
          Close
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default ViewOrderModal;
