// Register.js
import React, { useState } from "react";
import { registerUser } from "../Services/authService";

export default function Register({ onRegister, onClose }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("Client");
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setMessage("");

    try {
      const data = await registerUser(email, password, role);
      setMessage("✅ Registration successful!");
      onRegister(data);
    } catch (err) {
      setError(err.message || "❌ Registration failed");
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h3>Register</h3>

        <form onSubmit={handleSubmit}>
          <label>
            Email:
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </label>

          <label>
            Password:
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </label>

          <label>
            Role:
            <select value={role} onChange={(e) => setRole(e.target.value)}>
              <option value="Client">Client</option>
              <option value="Admin">Admin</option>
            </select>
          </label>

          <div className="modal-buttons">
            <button type="submit">Register</button>
            <button type="button" onClick={onClose}>
              Cancel
            </button>
          </div>
        </form>

        {error && <p className="message">{error}</p>}
        {message && <p className="message success">{message}</p>}
      </div>
    </div>
  );
}
