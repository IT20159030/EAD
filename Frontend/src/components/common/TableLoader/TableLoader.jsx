import Spinner from "react-bootstrap/Spinner";

const TableLoader = ({ loading, colSpan = "3" }) => {
  return (
    <tbody>
      {loading ? (
        <tr>
          <td
            colSpan={colSpan}
            rowSpan="5"
            className="text-center h5 py-5 text-muted"
          >
            <Spinner animation="grow" className="me-2" />
            <Spinner animation="grow" className="me-2" />
            <Spinner animation="grow" className="me-2" />
          </td>
        </tr>
      ) : (
        <tr>
          <td colSpan="3" className="text-center">
            No data available.
          </td>
        </tr>
      )}
    </tbody>
  );
};

export default TableLoader;
