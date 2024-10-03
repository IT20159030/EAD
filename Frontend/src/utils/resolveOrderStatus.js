const resolveOrderStatus = (code) => {
  switch (code) {
    case 0:
      return 'Pending';
    case 1:
      return 'Partial';
    case 2:
      return 'Approved';
    case 3:
      return 'Rejected';
    case 4:
      return 'Completed';
    case 5:
      return 'Cancel Requested';
    case 6:
      return 'Cancelled';
    default:
      return 'Unknown';
  }
};

export default resolveOrderStatus;
