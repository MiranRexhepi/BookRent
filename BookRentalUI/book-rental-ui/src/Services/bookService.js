import { API_URL, authenticatedFetch } from "./authService";

export async function getBooks(params) {
  const query = new URLSearchParams(params).toString();
  const response = await authenticatedFetch(`${API_URL}/books/available?${query}`);

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
  const response = await authenticatedFetch(API_URL + "/books", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(book),
  });
  return await response.json();
}

export async function updateBook(id, book) {
  const response = await authenticatedFetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      title: book.title,
      author: book.author,
      genre: book.genre,
      isbn: book.isbn,
    }),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData?.message || "Failed to update book.");
  }

  return await response.json();
}
