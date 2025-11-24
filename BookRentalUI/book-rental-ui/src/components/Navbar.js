import React from "react";
import { Link } from "react-router-dom";

export default function Navbar({ user, onLogout, onShowRegister }) {
  return (
    <nav className="navbar">
      <h2 className="navbar-logo">📚 BookRental</h2>
      <div className="navbar-links">
        <Link to="/">Books</Link>
        {user && <Link to="/books/infinite">Browse (Infinite)</Link>}

        {user ? (
          <>
            {user.role === "Admin" ? (
              <>
                <span
                  className="navbar-link"
                  onClick={onShowRegister}
                  style={{
                    cursor: "pointer",
                    color: "#61dafb",
                    margin: "0 1rem",
                  }}
                >
                  Register
                </span>
                <Link to="/rentals/all">Rental History</Link>
              </>
            ) : (
              <Link to="/rentals">My Rentals</Link>
            )}
            <button onClick={onLogout}>Logout</button>
          </>
        ) : (
          <>
            <Link to="/login">Login</Link>
            <span
              className="navbar-link"
              onClick={onShowRegister}
              style={{ cursor: "pointer", color: "#61dafb", margin: "0 1rem" }}
            >
              Register
            </span>
            <Link to="/tenant/register">Tenant Register</Link>
          </>
        )}
      </div>
    </nav>
  );
}
