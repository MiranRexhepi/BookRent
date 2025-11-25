import { API_URL, getToken } from "./authService";

export async function getDashboardStats() {
  const response = await fetch(`${API_URL}/dashboard/stats`, {
    headers: { Authorization: `Bearer ${getToken()}` },
  });

  if (!response.ok) {
    throw new Error("Failed to fetch dashboard stats");
  }

  return await response.json();
}

export async function getDailyRentalStats() {
  const response = await fetch(`${API_URL}/dashboard/daily-rentals`, {
    headers: { Authorization: `Bearer ${getToken()}` },
  });

  if (!response.ok) {
    throw new Error("Failed to fetch daily rental stats");
  }

  return await response.json();
}

export async function getRentalHistory(params) {
  const query = new URLSearchParams(params).toString();
  const response = await fetch(`${API_URL}/dashboard/rental-history?${query}`, {
    headers: { Authorization: `Bearer ${getToken()}` },
  });

  if (!response.ok) {
    throw new Error("Failed to fetch rental history");
  }

  return await response.json();
}

export async function getAvailableBooks(params) {
  const query = new URLSearchParams(params).toString();
  const response = await fetch(`${API_URL}/dashboard/available-books?${query}`, {
    headers: { Authorization: `Bearer ${getToken()}` },
  });

  if (!response.ok) {
    throw new Error("Failed to fetch available books");
  }

  return await response.json();
}

