export async function loginUser(username, password) {
  const response = await fetch(`${API_URL}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: username, password }),
  });
  const data = await response.json();
  if (!response.ok) throw new Error(data.error || "Login failed");
  localStorage.setItem("token", data.token);
  if (data.refreshToken) {
    localStorage.setItem("refreshToken", data.refreshToken);
  }
  return data;
}

export async function registerUser(username, password, role) {
  const response = await authenticatedFetch(`${API_URL}/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
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

export function getRefreshToken() {
  return localStorage.getItem("refreshToken");
}

export function clearTokens() {
  localStorage.removeItem("token");
  localStorage.removeItem("refreshToken");
}

export async function refreshToken() {
  const refreshToken = getRefreshToken();
  if (!refreshToken) {
    throw new Error("No refresh token available");
  }

  const response = await fetch(`${API_URL}/refresh`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ refreshToken }),
  });

  const data = await response.json();
  
  if (!response.ok) {
    clearTokens();
    throw new Error(data.error || "Token refresh failed");
  }

  localStorage.setItem("token", data.token);
  if (data.refreshToken) {
    localStorage.setItem("refreshToken", data.refreshToken);
  }

  return data;
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
  if (data.refreshToken) {
    localStorage.setItem("refreshToken", data.refreshToken);
  }
  return data;
}

let isRefreshing = false;
let refreshPromise = null;

export async function authenticatedFetch(url, options = {}) {
  const token = getToken();
  const headers = {
    ...options.headers,
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  let response = await fetch(url, {
    ...options,
    headers,
  });

  if (response.status === 401 && getRefreshToken()) {
    if (!isRefreshing) {
      isRefreshing = true;
      refreshPromise = refreshToken().catch((error) => {
        clearTokens();
        if (typeof window !== 'undefined') {
          window.dispatchEvent(new CustomEvent("auth:logout"));
        }
        throw error;
      });
    }

    try {
      await refreshPromise;
      isRefreshing = false;
      refreshPromise = null;

      const newToken = getToken();
      if (newToken) {
        headers.Authorization = `Bearer ${newToken}`;
        response = await fetch(url, {
          ...options,
          headers,
        });
      }
    } catch (error) {
      isRefreshing = false;
      refreshPromise = null;
      throw error;
    }
  }

  return response;
}

export const API_URL = "http://localhost:5000/api";
export const API_URL_WSS = "ws://localhost:5000";
