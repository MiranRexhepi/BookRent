export async function loginUser(username, password) {
  const response = await fetch("https://localhost:7032/api/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: username, password }),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.error || "Login failed");
  localStorage.setItem("token", data.token);
  return data;
}

export async function registerUser(username, password) {
  const response = await fetch("https://localhost:7032/api/register", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: username, password }),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.error || "Register failed");
  return data;
}

export function getToken() {
  return localStorage.getItem("token");
}

export async function tenantRegister(username, password, name) {
  const response = await fetch("https://localhost:7032/api/tenants", {
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

export const API_URL = "https://localhost:7032/api";
