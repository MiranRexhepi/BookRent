import { API_URL, getToken } from "./authService";

export async function rentBook(bookId) {
  const response = await fetch(`${API_URL}/rentals/books/${bookId}`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${getToken()}`,
    },
  });
  const data = response;
  console.log("🚀 ~ rentssBook ~ ressssponse:", data);

  if (!response.ok) {
    throw { response: await response.json() };
  }

  return await response.json();
}

export async function returnBook(id, bookId) {
  const response = await fetch(`${API_URL}/rentals/${id}/books/${bookId}`, {
    method: "PUT",
    headers: { Authorization: `Bearer ${getToken()}` },
  });
  const data = await response.json();
  return data;
}

export async function getRentalHistory() {
  const response = await fetch(`${API_URL}/rentals/current`, {
    headers: { Authorization: `Bearer ${getToken()}` },
  });
  const data = await response.json();
  return data;
}
