import React, { useState } from "react";
import { tenantRegister } from "../Services/authService";

export default function RegisterTenant({ onTenantRegister }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [tenantName, setTenantName] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    try {
      const data = await tenantRegister(username, password, tenantName);
      onTenantRegister(data);
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="login-container">
      <h2>Register</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="tenantName"
          placeholder="Tenant Name"
          value={tenantName}
          onChange={(e) => setTenantName(e.target.value)}
          required
        />
        <input
          placeholder="Email"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">Register Tenant</button>
      </form>
      {error && <p className="message">{error}</p>}
    </div>
  );
}
