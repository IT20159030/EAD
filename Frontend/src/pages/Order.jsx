import { useEffect, useState } from 'react';
import * as orderHooks from '../hooks/orderHooks';
import AutoClosingToast from '../components/common/Toast/AutoClosingToast';
import LoadingTableBody from '../components/common/TableLoader/TableLoader';
import styles from './styles/Pages.module.css';
import CommonTitle from '../components/common/Title/Title';
import Table from 'react-bootstrap/Table';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import ViewOrderModal from '../components/order/ViewOrderModal';
import resolveOrderStatus from '../utils/resolveOrderStatus';
import { useAuth } from '../provider/authProvider';
import { ButtonGroup, ToggleButton } from 'react-bootstrap';

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

const filterOptions = [
  { id: 0, label: 'All' },
  { id: 1, label: 'Pending' },
  { id: 2, label: 'Cancel Requests' },
  { id: 3, label: 'Partial' },
  { id: 4, label: 'Completed' },
  { id: 5, label: 'Cancelled' },
];

const Order = () => {
  const { user } = useAuth();
  const [showModal, setShowModal] = useState(false);
  const [orderDetails, setOrderDetails] = useState(null);
  const [showToast, setShowToast] = useState(false);
  const [toastMessage, setToastMessage] = useState('');
  const [toastType, setToastType] = useState('success');
  const [radioValue, setRadioValue] = useState(0);

  const { data: allOrders, isLoading: isLoadingAllOrders } =
    orderHooks.useGetAllOrders();
  const { data: vendorOrders, isLoading: isLoadingVendorOrders } =
    orderHooks.useGetOrderByVendorId(user?.id);

  const orders = user.role === 'vendor' ? vendorOrders : allOrders;
  const isLoadingOrders =
    user.role === 'vendor' ? isLoadingVendorOrders : isLoadingAllOrders;

  const [filteredOrders, setFilteredOrders] = useState(orders);

  useEffect(() => {
    if (orders) {
      setFilteredOrders(orders);
    }
  }, [allOrders, vendorOrders]);

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

  // filter orders based on status
  // 0 -> All
  // 1 -> Cancel Requested
  // 2 -> Partial
  // 3 -> Completed
  const filterOrders = (selectedIndex) => {
    if (selectedIndex === 0) {
      setFilteredOrders(orders);
      return;
    }

    // Map selectedIndex to status codes
    const statusMap = [0, 0, 5, 1, 4, 6]; // Add more statuses if needed

    // Get the status based on selectedIndex
    const status = statusMap[selectedIndex] || 0;

    const filteredOrders = orders.filter((order) => order.status === status);
    setFilteredOrders(filteredOrders);
  };

  const handleButtonGroupChange = (selectedIndex) => {
    setRadioValue(selectedIndex);
    filterOrders(selectedIndex);
  };

  return (
    <div className={styles.pageContainer}>
      <CommonTitle title='Orders' />

      <ButtonGroup className='mb-3'>
        {filterOptions.map((option) => (
          <ToggleButton
            key={option.id}
            type='radio'
            variant='outline-primary'
            name='radio'
            value={option.id}
            checked={radioValue === option.id}
            onClick={() => handleButtonGroupChange(option.id)}>
            {option.label}
          </ToggleButton>
        ))}
      </ButtonGroup>

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
        {isLoadingOrders || filteredOrders == null ? (
          <LoadingTableBody loading={isLoadingOrders} colSpan='7' />
        ) : (
          <tbody>
            {filteredOrders?.map((order, index) => (
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
