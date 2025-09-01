import React, { useEffect, useState, useRef } from "react";
import { getBooks } from "../Services/bookService";
import "../css/BookList.css";
import { rentBook } from "../Services/rentalService";
import { getToken } from "../Services/authService";

export default function BookList() {
  const [books, setBooks] = useState([]);
  const [search, setSearch] = useState("");
  const [genreFilter, setGenreFilter] = useState("All");
  const [message, setMessage] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);

  const token = getToken();
  const socketRef = useRef(null);

  useEffect(() => {
    if (!token) {
      setMessage("❌ You must be logged in to view books.");
      return;
    }
    loadBooks();
  }, [pageNumber, genreFilter, search, token]);

  useEffect(() => {
    if (!token) return;

    let ws;
    if (socketRef.current) {
      socketRef.current.close();
      socketRef.current = null;
    }

    ws = new WebSocket(`wss://localhost:7032/ws?token=${token}`);
    socketRef.current = ws;

    ws.onopen = () => {
      console.log("WS connected");
    };

    ws.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);

        if (data?.BookId !== undefined && data?.Status !== undefined) {
          const available = data.Status === 2;

          setBooks((prevBooks) =>
            prevBooks.map((b) =>
              b.id === data.BookId ? { ...b, available } : b
            )
          );
        }
      } catch (err) {
        console.error("Failed to parse WS message:", err);
      }
    };

    ws.onclose = (event) => {
      console.log("WS disconnected:", event.reason || "closed");
    };

    ws.onerror = (err) => {
      console.error("WS error:", err);
    };

    return () => {
      ws.close();
    };
  }, [token]);

  const loadBooks = async () => {
    if (!token) return;

    const params = {
      PageNumber: pageNumber,
      PageSize: pageSize,
      Search: search,
      Genre: genreFilter !== "All" ? genreFilter : "",
    };

    try {
      const data = await getBooks(params);
      setBooks(data.items);
      setTotalPages(data.totalPages);
    } catch (err) {
      console.error(err);
      setMessage("❌ Failed to load books.");
    }
  };

  const handleRent = async (bookId) => {
    if (!token) {
      setMessage("❌ You must be logged in to rent a book.");
      return;
    }

    try {
      await rentBook(bookId);
      setMessage("✅ Book rented successfully!");
      setBooks((prevBooks) =>
        prevBooks.map((b) => (b.id === bookId ? { ...b, available: false } : b))
      );
    } catch (error) {
      setMessage(
        error?.response?.Error
          ? `❌ ${error.response.Error}`
          : "❌ Failed to rent book."
      );
    }

    setTimeout(() => setMessage(""), 5000);
  };

  const genres = ["All", ...new Set(books.map((b) => b.genre))];

  if (!token) {
    return <div className="booklist-container">{message}</div>;
  }

  return (
    <div className="booklist-container">
      <h2>📚 Book Catalog</h2>
      {message && <p className="message">{message}</p>}

      <div className="filters">
        <input
          type="text"
          placeholder="Search by title, author, or ISBN..."
          value={search}
          onChange={(e) => {
            setPageNumber(1);
            setSearch(e.target.value);
          }}
        />
        <select
          value={genreFilter}
          onChange={(e) => {
            setPageNumber(1);
            setGenreFilter(e.target.value);
          }}
        >
          {genres.map((g, idx) => (
            <option key={idx} value={g}>
              {g}
            </option>
          ))}
        </select>
      </div>

      <table className="book-table">
        <thead>
          <tr>
            <th>📖 Title</th>
            <th>👤 Author</th>
            <th>🎭 Genre</th>
            <th>ISBN</th>
            <th>Status</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {books.map((book) => (
            <tr key={book.id}>
              <td>{book.title}</td>
              <td>{book.author}</td>
              <td>{book.genre}</td>
              <td>{book.isbn}</td>
              <td>
                <span
                  className={`status ${
                    book.available ? "available" : "unavailable"
                  }`}
                >
                  {book.available ? "Available ✅" : "Not Available ❌"}
                </span>
              </td>
              <td>
                <button
                  disabled={!book.available}
                  onClick={() => handleRent(book.id)}
                >
                  Rent
                </button>
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
