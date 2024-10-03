import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import Spinner from 'react-bootstrap/Spinner';
import Badge from 'react-bootstrap/Badge';
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
  const { mutate: markOrderCancelled } = orderHooks.useMarkOrderCancelled();
  const { data: orderCancellationDetails, isLoadingCancellationDetails } =
    orderHooks.useGetOrderCancellationDetails(orderDetails.id);
  const {
    mutate: updateOrderCancellationDetails,
    isUpdatingCancellationRequest,
  } = orderHooks.useUpdateOrderCancellationDetails();

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

  const handleOrderCancellationRequestAccept = () => {
    updateOrderCancellationDetails(
      {
        id: orderCancellationDetails.id,
        processedBy: 'Vendor',
        status: 'Approved',
        decisionNote: 'Order cancellation request accepted by CSR',
      },
      {
        onSuccess: () => {
          markOrderCancelled(orderDetails.id, {
            onSuccess: () => {
              handleToast('Order cancellation request accepted');
              setOrderStatus(6);
            },
          });
        },
        onError: () => {
          handleToast(
            'Failed to accept order cancellation request',
            'bg-danger'
          );
        },
      }
    );
  };

  return (
    <Modal show={show} onHide={handleClose} size='lg' centered>
      <Modal.Header closeButton>
        <Modal.Title className='text-primary'>Order Details</Modal.Title>
      </Modal.Header>
      <Modal.Body className='p-4'>
        <div className='mb-3'>
          <p>
            <strong>Order ID:</strong> {orderDetails?.orderId}
          </p>
          <p>
            <strong>Vendor ID:</strong> {orderDetails?.vendorId}
          </p>
          <p>
            <strong>Order Status:</strong>
            <Badge
              bg={
                resolveOrderStatus(orderStatus) === 'Delivered'
                  ? 'success'
                  : 'warning'
              }>
              {resolveOrderStatus(orderStatus)}
            </Badge>
          </p>
        </div>

        <div className='mb-4'>
          <h6 className='text-secondary'>Order Items</h6>
          <Table striped bordered hover responsive className='table-sm'>
            <thead className='table-light'>
              <tr>
                <th>#</th>
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
                  <td>${item.price.toFixed(2)}</td>
                  <td>
                    {item.status === 0 ? (
                      <Button
                        variant={
                          orderStatus === 5 || orderStatus === 6
                            ? 'secondary'
                            : 'primary'
                        }
                        size='sm'
                        onClick={() => handleOrderItemStatusChange(item)}
                        disabled={
                          isUpdatingOrder ||
                          isMarkingReady ||
                          isMarkingDelivered ||
                          orderStatus === 5 ||
                          orderStatus === 6
                        }>
                        {isUpdatingOrder ||
                        isMarkingReady ||
                        isMarkingDelivered ? (
                          <Spinner animation='border' size='sm' />
                        ) : (
                          <i className='bi bi-check-circle'>
                            Mark as Delivered
                          </i>
                        )}
                      </Button>
                    ) : (
                      <Badge bg='success'>Delivered</Badge>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
        </div>

        {(orderDetails?.status === 5 || orderDetails?.status === 6) && (
          <div className='border p-3 rounded'>
            <h6 className='text-danger'>Order Cancellation Details</h6>
            {isLoadingCancellationDetails ? (
              <Spinner animation='border' size='sm' />
            ) : (
              <>
                <p>
                  <strong>Reason for cancellation:</strong>{' '}
                  {orderCancellationDetails?.reason}
                </p>
                {orderDetails?.status === 5 && (
                  <Button
                    disabled={isUpdatingCancellationRequest}
                    onClick={handleOrderCancellationRequestAccept}
                    variant={
                      isUpdatingCancellationRequest ? 'secondary' : 'success'
                    }>
                    {isUpdatingCancellationRequest ? (
                      <Spinner animation='border' size='sm' />
                    ) : (
                      <i className='bi bi-check-circle'>Approve Cancellation</i>
                    )}
                  </Button>
                )}
              </>
            )}
          </div>
        )}
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
