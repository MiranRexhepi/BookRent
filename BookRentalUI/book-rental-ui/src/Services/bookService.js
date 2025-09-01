import { API_URL, getToken } from "./authService";

export async function getBooks(params) {
  const query = new URLSearchParams(params).toString();
  const response = await fetch(`${API_URL}/books/available?${query}`, {
    headers: { Authorization: `Bearer ${getToken()}` },
  });

  const data = await response.json();

  return {
    items: data.items.map((book) => ({
      id: book.id,
      title: book.title,
      author: book.author,
      genre: book.genre,
      isbn: book.isbn,
      available: book.isDeleted === 0,
    })),
    totalItems: data.totalItems,
    pageNumber: data.pageNumber,
    pageSize: data.pageSize,
    totalPages: data.totalPages,
  };
}

export async function addBook(book) {
  const response = await fetch(API_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${getToken()}`,
    },
    body: JSON.stringify(book),
  });
  return await response.json();
}
