import React from "react";
import { Link } from "react-router-dom";

export default function Navbar({ user, onLogout }) {
  return (
    <nav className="navbar">
      <h2 className="navbar-logo">📚 BookRental</h2>
      <div className="navbar-links">
        <Link to="/">Books</Link>
        {user ? (
          <>
            {user.role === "Admin" && <Link to="/add-book">Add Book</Link>}
            <Link to="/rentals">My Rentals</Link>
            <button onClick={onLogout}>Logout</button>
          </>
        ) : (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
            <Link to="/tenant/register">Tenant Register</Link>
          </>
        )}
      </div>
    </nav>
  );
}
