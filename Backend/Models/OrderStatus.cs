/**
The OrderStatus enum is used to represent the status of an order in the application. It contains the following values:
    Pending: the order is pending approval.
    Approved: the order has been approved.
    Rejected: the order has been rejected.
    Completed: the order has been completed.
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