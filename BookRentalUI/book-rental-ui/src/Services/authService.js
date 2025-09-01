export async function loginUser(username, password) {
  const response = await fetch(`${API_URL}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: username, password }),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.error || "Login failed");
  localStorage.setItem("token", data.token);
  return data;
}

export async function registerUser(username, password, role) {
  const response = await fetch(`${API_URL}/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${getToken()}`,
    },
    body: JSON.stringify({ Email: username, Password: password, Role: role }),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.error || "Register failed");
  return data;
}

export function getToken() {
  return localStorage.getItem("token");
}

export async function tenantRegister(username, password, name) {
  const response = await fetch(`${API_URL}/tenants`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      UserEmail: username,
      Password: password,
      TenantName: name,
    }),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.error || "Register failed");
  localStorage.setItem("token", data.token);
  return data;
}

export const API_URL = "http://localhost:5000/api";
export const API_URL_WSS = "wss://localhost:5000";
