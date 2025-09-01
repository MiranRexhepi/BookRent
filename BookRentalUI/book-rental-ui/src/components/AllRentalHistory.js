import React, { useEffect, useState } from "react";
import { getAllRentalHistory } from "../Services/rentalService"; // make sure this calls /history
import "../css/BookList.css";

export default function AllRentalHistory() {
  const [history, setHistory] = useState([]);
  const [message, setMessage] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);

  const statusMap = {
    1: "Rented",
    2: "Returned",
    3: "Overdue",
  };

  useEffect(() => {
    loadHistory();
  }, [pageNumber]);

  const loadHistory = async () => {
    try {
      const data = await getAllRentalHistory({
        PageNumber: pageNumber,
        PageSize: pageSize,
      });
      setHistory(data.items || []);
      setTotalPages(data.totalPages || 1);
    } catch (err) {
      console.error(err);
      setMessage("❌ Failed to load rental history.");
      setTimeout(() => setMessage(""), 5000);
    }
  };

  return (
    <div className="booklist-container">
      <h2>📚 All Rental History</h2>
      {message && <p className="message">{message}</p>}

      <table className="book-table">
        <thead>
          <tr>
            <th>Rental ID</th>
            <th>User ID</th>
            <th>Book ID</th>
            <th>Status</th>
            <th>Rented At</th>
            <th>Returned At</th>
          </tr>
        </thead>
        <tbody>
          {history.map((r) => (
            <tr key={r.id}>
              <td>{r.id}</td>
              <td>{r.userId}</td>
              <td>{r.bookId}</td>
              <td>
                <span
                  className={`status ${
                    r.rentalStatus === 2 ? "available" : "unavailable"
                  }`}
                >
                  {statusMap[r.rentalStatus]}{" "}
                  {r.rentalStatus === 1
                    ? "✅"
                    : r.rentalStatus === 2
                    ? "✔️"
                    : "⚠️"}
                </span>
              </td>
              <td>{new Date(r.rentedAt).toLocaleString()}</td>
              <td>
                {r.returnedAt ? new Date(r.returnedAt).toLocaleString() : "-"}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="pagination">
        <button
          onClick={() => setPageNumber((prev) => Math.max(prev - 1, 1))}
          disabled={pageNumber === 1}
        >
          ⬅ Prev
        </button>
        <span>
          Page {pageNumber} of {totalPages}
        </span>
        <button
          onClick={() =>
            setPageNumber((prev) => Math.min(prev + 1, totalPages))
          }
          disabled={pageNumber === totalPages}
        >
          Next ➡
        </button>
      </div>
    </div>
  );
}
