import React, { useEffect, useState, useRef, useCallback } from "react";
import { getBooks } from "../Services/bookService";
import "../css/BookList.css";
import { rentBook } from "../Services/rentalService";
import { API_URL_WSS, getToken } from "../Services/authService";

export default function BookListInfinite({ user }) {
  const [books, setBooks] = useState([]);
  const [message, setMessage] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [loading, setLoading] = useState(false);
  const [hasMore, setHasMore] = useState(true);
  const [error, setError] = useState(false);

  const observerTarget = useRef(null);
  const token = getToken();
  const socketRef = useRef(null);

  const loadBooks = useCallback(async (page) => {
    setLoading(true);
    setError(false);

    try {
      const data = await getBooks({
        PageNumber: page,
        PageSize: pageSize,
      });

      const items = data.items || [];
      const totalPages = data.totalPages || 1;

      if (items.length === 0 || page >= totalPages) {
        setHasMore(false);
      }

      setBooks((prevBooks) => {
        const existingIds = new Set(prevBooks.map((b) => b.id));
        const newItems = items.filter((item) => !existingIds.has(item.id));
        return [...prevBooks, ...newItems];
      });
    } catch (err) {
      console.error(err);
      setError(true);
      setMessage("❌ Failed to load books.");
      setTimeout(() => setMessage(""), 5000);
    } finally {
      setLoading(false);
    }
  }, [pageSize]);

  useEffect(() => {
    if (!token) {
      setMessage("❌ You must be logged in to view books.");
      return;
    }
    setBooks([]);
    setPageNumber(1);
    setHasMore(true);
    loadBooks(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token]);

  useEffect(() => {
    if (!hasMore) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && hasMore && !loading) {
          setPageNumber((prev) => {
            const nextPage = prev + 1;
            loadBooks(nextPage);
            return nextPage;
          });
        }
      },
      { threshold: 0.1 }
    );

    const currentTarget = observerTarget.current;
    if (currentTarget) {
      observer.observe(currentTarget);
    }

    return () => {
      if (currentTarget) {
        observer.unobserve(currentTarget);
      }
    };
  }, [hasMore, loading, loadBooks]);

  useEffect(() => {
    if (!token) return;

    let ws;
    if (socketRef.current) {
      socketRef.current.close();
      socketRef.current = null;
    }

    ws = new WebSocket(`${API_URL_WSS}/ws?token=${token}`);
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

  if (!token) {
    return <div className="booklist-container">{message}</div>;
  }

  return (
    <div className="booklist-container" style={{ paddingBottom: "2rem" }}>
      <h2>📚 Browse Books (Infinite Scroll)</h2>
      {message && <p className="message">{message}</p>}

      <table className="book-table">
        <thead>
          <tr>
            <th>📖 Title</th>
            <th>👤 Author</th>
            <th>🎭 Genre</th>
            <th>ISBN</th>
            <th>Status</th>
            <th>Rent</th>
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

      <div ref={observerTarget} style={{ minHeight: "1px", marginTop: "1.5rem", marginBottom: "1rem" }}>
        {loading && (
          <div style={{ 
            textAlign: "center", 
            padding: "1rem 0",
            color: "white",
            fontSize: "0.95rem"
          }}>
            <p>Loading more books...</p>
          </div>
        )}
        {!hasMore && books.length > 0 && (
          <div style={{ 
            textAlign: "center", 
            padding: "1rem 0",
            color: "#999",
            fontSize: "0.95rem",
            fontStyle: "italic",
            marginBottom: "1rem"
          }}>
            <p>No more books to load</p>
          </div>
        )}
        {error && (
          <div style={{ 
            textAlign: "center", 
            padding: "1rem 0",
            color: "#ff6b6b",
            fontSize: "0.95rem"
          }}>
            <p>Error loading books. Scroll to retry.</p>
          </div>
        )}
      </div>
    </div>
  );
}

