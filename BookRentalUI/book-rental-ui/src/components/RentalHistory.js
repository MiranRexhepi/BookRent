import React, { useEffect, useState } from "react";
import { getRentalHistory, returnBook } from "../Services/rentalService";
import "../css/BookList.css";

export default function RentalHistory() {
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
      const data = await getRentalHistory({
        PageNumber: pageNumber,
        PageSize: pageSize,
      });
      setHistory(data.items || data);
      setTotalPages(data.totalPages || 1);
    } catch {
      setMessage("❌ Failed to load rental history.");
      setTimeout(() => setMessage(""), 5000);
    }
  };

  const handleReturn = async (id, bookId) => {
    try {
      await returnBook(id, bookId);
      setMessage("✅ Book returned successfully!");
      loadHistory();
    } catch {
      setMessage("❌ Failed to return book.");
    }
    setTimeout(() => setMessage(""), 5000);
  };

  return (
    <div className="booklist-container">
      <h2>📚 Rental History</h2>
      {message && <p className="message">{message}</p>}

      <table className="book-table">
        <thead>
          <tr>
            <th>Rental ID</th>
            <th>Book ID</th>
            <th>Status</th>
            <th>Rented At</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {history.map((r) => (
            <tr key={r.id}>
              <td>{r.id}</td>
              <td>{r.bookId}</td>
              <td>
                <span
                  className={`status ${
                    r.rentalStatus === 1 ? "available" : "unavailable"
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
                {r.rentalStatus === 1 || r.rentalStatus === 3 ? (
                  <button onClick={() => handleReturn(r.id, r.bookId)}>
                    Return
                  </button>
                ) : null}
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
