import { useState } from 'react';
import * as orderHooks from '../hooks/orderHooks';
import AutoClosingToast from '../components/common/Toast/AutoClosingToast';
import LoadingTableBody from '../components/common/TableLoader/TableLoader';
import styles from './styles/Pages.module.css';
import CommonTitle from '../components/common/Title/Title';
import Table from 'react-bootstrap/Table';
import Button from 'react-bootstrap/Button';
import ViewOrderModal from '../components/order/ViewOrderModal';
import resolveOrderStatus from '../utils/resolveOrderStatus';

/**
 * The pain page for Order management
 * @returns {JSX.Element} JSX.Element
 *
 * Has a table showing all orders
 * Can click on an order to view more details
 * Click will open a modal with more details
 * Can mark entire order as Complete -> marks all items as complete
 * can mark individual items as complete -> mark the order as ready and mark as complete if all items are complete
 *
 */

const Order = () => {
  const [showModal, setShowModal] = useState(false);
  const [orderDetails, setOrderDetails] = useState(null);
  const [showToast, setShowToast] = useState(false);
  const [toastMessage, setToastMessage] = useState('');
  const [toastType, setToastType] = useState('success');

  const { data: orders, isLoading: isLoadingOrders } =
    orderHooks.useGetAllOrders();

  const handleToast = (message, type = 'success') => {
    setToastMessage(message);
    setToastType(type);
    setShowToast(true);
    setTimeout(() => {
      setShowToast(false);
    }, 3000);
  };

  const handleModalOpen = (order) => {
    setOrderDetails(order);
    setShowModal(true);
  };

  return (
    <div className={styles.pageContainer}>
      <CommonTitle title='Orders' />

      <Table striped bordered hover responsive className={styles.table}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Date</th>
            <th>Status</th>
            <th>Customer</th>
            <th>Items</th>
            <th>Price</th>
            <th>Actions</th>
          </tr>
        </thead>
        {isLoadingOrders ? (
          <LoadingTableBody loading={isLoadingOrders} colSpan='7' />
        ) : (
          <tbody>
            {orders.map((order, index) => (
              <tr key={order.id}>
                <td>{index + 1}</td>
                <td>{new Date(order.orderDate).toDateString()}</td>
                <td>{resolveOrderStatus(order.status)}</td>
                <td>{order.customerName}</td>
                <td>{order.orderItems?.length}</td>
                <td>{order.totalPrice}</td>
                <td>
                  <Button
                    variant='primary'
                    onClick={() => {
                      handleModalOpen(order);
                    }}>
                    View
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        )}
      </Table>

      {showModal && (
        <ViewOrderModal
          show={showModal}
          handleClose={() => setShowModal(false)}
          orderDetails={orderDetails}
          handleToast={handleToast}
        />
      )}

      {showToast && (
        <AutoClosingToast
          title='Notification'
          description={toastMessage}
          type={toastType}
        />
      )}
    </div>
  );
};

export default Order;
