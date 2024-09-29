import { useEffect } from 'react';
import { useGetAllNotifications } from '../../hooks/notificationHooks';
import { Link } from 'react-router-dom';
import './Notifications.css';

const NotificationsPage = () => {
  const { data: notifications, refetch } = useGetAllNotifications();

  useEffect(() => {
    refetch();
  }, [refetch]);

  return (
    <div className="notificationsPage">
      <h2>All Notifications</h2>
      {notifications?.length > 0 ? (
        notifications.map((notification) => (
          <div key={notification.id} className="notificationItem">
            <Link
              to={notification.type === 'LowStock' ? '/inventory' : '/orders'}
              className="notificationLink"
            >
              <p className="message">{notification.message}</p>
              <span className="notificationDate">
                {new Date(notification.createdAt).toLocaleString()}
              </span>
            </Link>
          </div>
        ))
      ) : (
        <p>No notifications available</p>
      )}
    </div>
  );
};

export default NotificationsPage;
