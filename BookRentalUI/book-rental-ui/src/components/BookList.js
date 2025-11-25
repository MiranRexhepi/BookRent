import React, { useEffect, useState, useRef } from "react";
import { getBooks } from "../Services/bookService";
import "../css/BookList.css";
import { rentBook } from "../Services/rentalService";
import { API_URL, API_URL_WSS, getToken, authenticatedFetch } from "../Services/authService";
import ScrollDropdown from "./ScrollDropdown";

export default function BookList({ user }) {
  const [books, setBooks] = useState([]);
  const [search, setSearch] = useState("");
  const [message, setMessage] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);

  const [editingBook, setEditingBook] = useState(null);
  const [modalVisible, setModalVisible] = useState(false);
  const [formData, setFormData] = useState({
    title: "",
    author: "",
    genre: "",
    isbn: "",
  });

  const [titleOptions, setTitleOptions] = useState([]);
  const [authorOptions, setAuthorOptions] = useState([]);
  const [genreOptions, setGenreOptions] = useState([]);

  const [filters, setFilters] = useState({
    Title: "",
    Author: "",
    Genre: "",
  });

  const [titlePage, setTitlePage] = useState(1);
  const [authorPage, setAuthorPage] = useState(1);
  const [genrePage, setGenrePage] = useState(1);

  const [addingBook, setAddingBook] = useState(false);

  const titleRef = useRef();
  const authorRef = useRef();
  const genreRef = useRef();

  const openModal = (book) => {
    setEditingBook(book);
    setFormData({
      title: book.title,
      author: book.author,
      genre: book.genre,
      isbn: book.isbn,
    });
    setModalVisible(true);
  };

  const closeModal = () => {
    setEditingBook(null);
    setModalVisible(false);
  };

  const token = getToken();
  const socketRef = useRef(null);

  useEffect(() => {
    if (!token) {
      setMessage("❌ You must be logged in to view books.");
      return;
    }
    loadBooks();
  }, [pageNumber, search, token]);

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

  useEffect(() => {
    const loadFilters = async () => {
      try {
        const token = getToken();
        if (!token) return;

        const [titlesRes, authorsRes, genresRes] = await Promise.all([
          authenticatedFetch(`${API_URL}/books/columns/Title`),
          authenticatedFetch(`${API_URL}/books/columns/Author`),
          authenticatedFetch(`${API_URL}/books/columns/Genre`),
        ]);

        const [titles, authors, genres] = await Promise.all([
          titlesRes.json(),
          authorsRes.json(),
          genresRes.json(),
        ]);

        setTitleOptions(titles?.items);
        setAuthorOptions(authors?.items);
        setGenreOptions(genres?.items);
      } catch (err) {
        console.error("Failed to load filter options:", err);
      }
    };

    loadFilters();
  }, [token]);

  useEffect(() => {
    const handleScroll = () => {
      const el = titleRef.current;
      if (!el) return;
      if (el.scrollTop + el.clientHeight >= el.scrollHeight - 5) {
        setTitlePage((prev) => prev + 1);
      }
    };
    const el = titleRef.current;
    if (el) el.addEventListener("scroll", handleScroll);
    return () => el?.removeEventListener("scroll", handleScroll);
  }, []);

  useEffect(() => {
    loadFilterItems("Title", titlePage);
  }, [titlePage]);

  useEffect(() => {
    loadFilterItems("Author", authorPage);
  }, [authorPage]);

  useEffect(() => {
    loadFilterItems("Genre", genrePage);
  }, [genrePage]);

  const loadBooks = async (filters = {}) => {
    if (!token) return;

    const params = {
      PageNumber: pageNumber,
      PageSize: pageSize,
      Search: search,
      ...filters,
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

  const handleFilterChange = (type, value) => {
    setPageNumber(1);

    const newFilters = { ...filters, [type]: value };
    setFilters(newFilters);

    // Call loadBooks with latest filters
    loadBooks(newFilters);
  };

  const loadFilterItems = async (type, page) => {
    try {
      if (!token) return;

      const res = await fetch(
        `${API_URL}/books/columns/${type}?PageNumber=${page}&PageSize=10`,
        { headers: { Authorization: `Bearer ${token}` } }
      );
      const data = await res.json();

      if (!data.items) return;

      if (type === "Title") setTitleOptions((prev) => [...prev, ...data.items]);
      if (type === "Author")
        setAuthorOptions((prev) => [...prev, ...data.items]);
      if (type === "Genre") setGenreOptions((prev) => [...prev, ...data.items]);
    } catch (err) {
      console.error("Failed to load filter items", err);
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

  const handleUpdate = async () => {
    if (!editingBook) return;

    try {
      const token = getToken();
      if (!token) {
        setMessage("❌ You must be logged in.");
        return;
      }

      const response = await authenticatedFetch(`${API_URL}/books/${editingBook.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

      if (!response.ok) throw new Error("Failed to update book.");

      const updatedBook = {
        ...editingBook,
        ...formData,
      };
      setBooks((prevBooks) =>
        prevBooks.map((b) => (b.id === editingBook.id ? updatedBook : b))
      );

      setMessage("✅ Book updated successfully!");
      setTimeout(() => setMessage(""), 5000);
      closeModal();
    } catch (err) {
      console.error(err);
      setMessage("❌ Failed to update book.");
    }
  };

  const handleDelete = async (bookId) => {
    try {
      const token = getToken();
      if (!token) {
        setMessage("❌ You must be logged in.");
        return;
      }

      const response = await authenticatedFetch(`${API_URL}/books/${bookId}`, {
        method: "DELETE",
      });

      if (!response.ok) throw new Error("Failed to delete book.");

      setBooks((prevBooks) => prevBooks.filter((b) => b.id !== bookId));
      setMessage("✅ Book deleted successfully!");
      setTimeout(() => setMessage(""), 5000);
    } catch (err) {
      console.error(err);
      setMessage("❌ Failed to delete book.");
    }
  };

  const handleAddBook = async () => {
    try {
      const token = getToken();
      if (!token) {
        setMessage("❌ You must be logged in.");
        return;
      }

      const payload = {
        ...formData,
        available: true,
      };

      const response = await authenticatedFetch(`${API_URL}/books`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) throw new Error("Failed to add book.");
      const newBook = await response.json();

      setBooks((prevBooks) => [{ ...newBook, available: true }, ...prevBooks]);

      setMessage("✅ Book added successfully!");
      setTimeout(() => setMessage(""), 5000);

      closeModal();
    } catch (err) {
      console.error(err);
      setMessage("❌ Failed to add book.");
    }
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
        <div className="scroll-dropdown search-dropdown">
          <input
            type="text"
            className="selected" // same class as the selected div in ScrollDropdown
            placeholder="Search by title, author, or ISBN..."
            value={search}
            onChange={(e) => {
              setPageNumber(1);
              setSearch(e.target.value);
            }}
          />
        </div>
        <div className="filters" style={{ display: "flex", gap: "1rem" }}>
          <div className="filters-horizontal">
            <ScrollDropdown
              items={titleOptions}
              value={filters.Title}
              onChange={(val) => handleFilterChange("Title", val)}
              placeholder="Select Title"
            />

            <ScrollDropdown
              items={authorOptions}
              value={filters.Author}
              onChange={(val) => handleFilterChange("Author", val)}
              placeholder="Select Author"
            />

            <ScrollDropdown
              items={genreOptions}
              value={filters.Genre}
              onChange={(val) => handleFilterChange("Genre", val)}
              placeholder="Select Genre"
            />
          </div>
        </div>
      </div>
      {user?.role === "Admin" && (
        <div className="add-book-btn">
          <button
            onClick={() => {
              setFormData({
                title: "",
                author: "",
                genre: "",
                isbn: "",
                status: "Available",
              });
              setAddingBook(true);
              setModalVisible(true);
            }}
          >
            ➕ Add Book
          </button>
        </div>
      )}

      <table className="book-table">
        <thead>
          <tr>
            <th>📖 Title</th>
            <th>👤 Author</th>
            <th>🎭 Genre</th>
            <th>ISBN</th>
            <th>Status</th>
            {user?.role !== "Admin" && <th>Rent</th>}
            {user?.role === "Admin" && <th>Update</th>}
            {user?.role === "Admin" && <th>Delete</th>}
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

              {user?.role !== "Admin" && (
                <td>
                  <button
                    disabled={!book.available}
                    onClick={() => handleRent(book.id)}
                  >
                    Rent
                  </button>
                </td>
              )}

              {user?.role === "Admin" && (
                <>
                  <td>
                    <button onClick={() => openModal(book)}>Update</button>
                  </td>
                  <td>
                    <button onClick={() => handleDelete(book.id)}>
                      Delete
                    </button>
                  </td>
                </>
              )}
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

      {modalVisible && (
        <div className="modal-overlay">
          <div className="modal">
            <h3>{addingBook ? "Add New Book" : "Edit Book"}</h3>

            <label>
              Title:
              <input
                type="text"
                value={formData.title}
                onChange={(e) =>
                  setFormData({ ...formData, title: e.target.value })
                }
              />
            </label>
            <label>
              Author:
              <input
                type="text"
                value={formData.author}
                onChange={(e) =>
                  setFormData({ ...formData, author: e.target.value })
                }
              />
            </label>
            <label>
              Genre:
              <input
                type="text"
                value={formData.genre}
                onChange={(e) =>
                  setFormData({ ...formData, genre: e.target.value })
                }
              />
            </label>
            <label>
              ISBN:
              <input
                type="text"
                value={formData.isbn}
                onChange={(e) =>
                  setFormData({ ...formData, isbn: e.target.value })
                }
              />
            </label>
            <div className="modal-buttons">
              <button onClick={addingBook ? handleAddBook : handleUpdate}>
                Save
              </button>
              <button onClick={closeModal}>Cancel</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
