// App.js
import React, { useState } from "react";
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

function App() {
  const [user, setUser] = useState(null);
  const [showRegisterModal, setShowRegisterModal] = useState(false);

  const handleLogout = () => {
    setUser(null);
    localStorage.removeItem("token");
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
                element={<RegisterTenant onTenantRegister={setUser} />}
              />
              <Route path="/login" element={<Login onLogin={setUser} />} />
              <Route path="*" element={<Navigate to="/login" replace />} />
            </Routes>
          </div>
        ) : (
          <Routes>
            <Route path="/" element={<BookList user={user} />} />
            <Route path="/rentals" element={<RentalHistory />} />
            <Route path="/rentals/all" element={<AllRentalHistory />} />
            <Route path="/rent" element={<RentBook />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        )}
      </div>
    </Router>
  );
}

export default App;
