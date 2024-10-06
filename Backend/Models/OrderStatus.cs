/**
The OrderStatus enum is used to represent the status of an order in the application. It contains the following values:
    Pending: the order is pending approval.
    Ready: the order is ready for processing.
    Approved: the order has been approved.
    Rejected: the order has been rejected.
    Completed: the order has been completed.
    CancelRequested: a request to cancel the order has been made.
    Cancelled: the order has been cancelled.
*/

namespace Backend.Models
{
    public enum OrderStatus
    {
        Pending,
        Ready,
        Approved,
        Rejected,
        Completed,
        CancelRequested,
        Cancelled
    }
}