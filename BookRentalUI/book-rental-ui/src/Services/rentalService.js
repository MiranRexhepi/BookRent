import { API_URL, authenticatedFetch } from "./authService";

export async function rentBook(bookId) {
  const response = await authenticatedFetch(`${API_URL}/rentals/books/${bookId}`, {
    method: "POST",
  });
  const data = response;
  console.log("🚀 ~ rentssBook ~ ressssponse:", data);

  if (!response.ok) {
    throw { response: await response.json() };
  }

  return await response.json();
}

export async function returnBook(id, bookId) {
  const response = await authenticatedFetch(`${API_URL}/rentals/${id}/books/${bookId}`, {
    method: "PUT",
  });
  const data = await response.json();
  return data;
}

export async function getRentalHistory() {
  const response = await authenticatedFetch(`${API_URL}/rentals/current`);
  const data = await response.json();
  return data;
}

export async function getAllRentalHistory(params) {
  const query = new URLSearchParams(params).toString();
  const response = await authenticatedFetch(`${API_URL}/rentals/history?${query}`);
  const data = await response.json();
  return data;
}
