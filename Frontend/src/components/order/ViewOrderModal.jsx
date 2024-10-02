import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import Spinner from 'react-bootstrap/Spinner';
import { Table } from 'react-bootstrap';
import resolveOrderStatus from '../../utils/resolveOrderStatus';
import * as orderHooks from '../../hooks/orderHooks';

/**
 * A modal to view order details
 *
 * Show the list of order items
 * Individual item can be set as delivered -> mark order as partially delivered
 * Mark entire order as delivered -> mark order as delivered
 */

const ViewOrderModal = ({ show, handleClose, orderDetails, handleToast }) => {
  const [orderItems, setOrderItems] = useState([]);
  const [orderStatus, setOrderStatus] = useState(0);

  const { mutate: updateOrder, isLoading: isUpdatingOrder } =
    orderHooks.useUpdateOrder();
  const { mutate: markOrderReady, isLoading: isMarkingReady } =
    orderHooks.useMarkOrderReady();
  const { mutate: markOrderDelivered, isLoading: isMarkingDelivered } =
    orderHooks.useMarkOrderDelivered();

  useEffect(() => {
    if (orderDetails) {
      setOrderItems(orderDetails.orderItems);
      setOrderStatus(orderDetails.status);
    }
  }, [orderDetails]);

  const handleOrderItemStatusChange = (item) => {
    const updatedOrderItems = orderItems.map((orderItem) => {
      if (orderItem.id === item.id) {
        return { ...orderItem, status: 4 };
      }
      return orderItem;
    });

    // update order details
    updateOrder(
      { ...orderDetails, orderItems: updatedOrderItems },
      {
        onSuccess: () => {
          handleToast('Order item marked as delivered');
          setOrderItems(updatedOrderItems);
        },
        onError: () => {
          handleToast('Failed to mark order item as delivered', 'bg-danger');
        },
      }
    );

    // if order is not ready, mark it as ready
    if (orderDetails.status !== 1) {
      markOrderReady(orderDetails.id, {
        onSuccess: () => {
          setOrderStatus(1);
        },
        onError: () => {
          handleToast('Failed to mark order as ready', 'bg-danger');
          console.log('Failed to mark order as ready : ' + orderDetails.id);
        },
      });
    }

    // if all items are delivered, mark order as delivered
    const allItemsDelivered = updatedOrderItems.every(
      (item) => item.status === 4
    );
    if (allItemsDelivered) {
      markOrderDelivered(orderDetails.id, {
        onSuccess: () => {
          setOrderStatus(4);
        },
        onError: () => {
          handleToast('Failed to mark order as delivered', 'bg-danger');
        },
      });
    }
  };

  return (
    <Modal show={show} onHide={handleClose} size='lg' centered>
      <Modal.Header closeButton>
        <Modal.Title>Order Details</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <h6>Order ID: {orderDetails?.orderId}</h6>
        <h6>Vendor ID: {orderDetails?.vendorId}</h6>
        <h6>Order Status: {resolveOrderStatus(orderStatus)}</h6>
        <h6>Order Items:</h6>

        <Table striped bordered hover responsive>
          <thead>
            <tr>
              <th>ID</th>
              <th>Product Name</th>
              <th>Quantity</th>
              <th>Price</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {orderItems.map((item, index) => (
              <tr key={item.id}>
                <td>{index + 1}</td>
                <td>{item.productName}</td>
                <td>{item.quantity}</td>
                <td>{item.price}</td>
                <td>
                  {item.status === 0 ? (
                    <Button
                      variant='primary'
                      onClick={() => handleOrderItemStatusChange(item)}
                      disabled={
                        isUpdatingOrder || isMarkingReady || isMarkingDelivered
                      }>
                      {isUpdatingOrder ||
                      isMarkingReady ||
                      isMarkingDelivered ? (
                        <Spinner animation='border' size='sm' />
                      ) : (
                        'Mark as Delivered'
                      )}
                    </Button>
                  ) : (
                    'Delivered'
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
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
