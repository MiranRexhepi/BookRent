// App.js
import React, { useState, useEffect } from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import "./App.css";
import Navbar from "./components/Navbar";
import Login from "./components/Login";
import Register from "./components/Register";
import RegisterTenant from "./components/TenantRegister";
import RentalHistory from "./components/RentalHistory";
import BookList from "./components/BookList";
import RentBook from "./components/RentBook";
import AllRentalHistory from "./components/AllRentalHistory";
import BookListInfinite from "./components/BookListInfinite";
import Dashboard from "./components/Dashboard";
import { getToken } from "./Services/authService";

function App() {
  const [user, setUser] = useState(null);
  const [showRegisterModal, setShowRegisterModal] = useState(false);

  // Restore user state from localStorage on mount
  useEffect(() => {
    const token = getToken();
    const savedUser = localStorage.getItem("user");
    if (token && savedUser) {
      try {
        setUser(JSON.parse(savedUser));
      } catch (err) {
        console.error("Failed to parse saved user:", err);
        localStorage.removeItem("user");
        localStorage.removeItem("token");
      }
    }
  }, []);

  const handleLogin = (userData) => {
    // Save user data to localStorage
    localStorage.setItem("user", JSON.stringify(userData));
    setUser(userData);
  };

  const handleLogout = () => {
    setUser(null);
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  };

  return (
    <Router>
      <div className="App">
        {user && (
          <Navbar
            user={user}
            onLogout={handleLogout}
            onShowRegister={() => setShowRegisterModal(true)}
          />
        )}

        {showRegisterModal && (
            <Register
            onRegister={(data) => {
              localStorage.setItem("user", JSON.stringify(data));
              setUser(data);
              setShowRegisterModal(false);
            }}
            onClose={() => setShowRegisterModal(false)}
          />
        )}

        {!user ? (
          <div className="centered-container">
            <Routes>
              <Route
                path="/tenant/register"
                element={<RegisterTenant onTenantRegister={(data) => {
                  localStorage.setItem("user", JSON.stringify(data));
                  setUser(data);
                }} />}
              />
              <Route path="/login" element={<Login onLogin={handleLogin} />} />
              <Route path="*" element={<Navigate to="/login" replace />} />
            </Routes>
          </div>
        ) : (
          <Routes>
            <Route path="/" element={<BookList user={user} />} />
            <Route path="/books/infinite" element={<BookListInfinite user={user} />} />
            <Route path="/rentals" element={<RentalHistory />} />
            <Route path="/rentals/all" element={<AllRentalHistory />} />
            <Route path="/rent" element={<RentBook />} />
            {user.role === "Admin" && (
              <Route path="/dashboard" element={<Dashboard />} />
            )}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        )}
      </div>
    </Router>
  );
}

export default App;
