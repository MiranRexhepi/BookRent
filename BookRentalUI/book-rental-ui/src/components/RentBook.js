import React, { useState, useEffect } from 'react';
import { getBooks } from '../Services/bookService';
import { rentBook } from '../Services/rentalService';

export default function RentBook() {
  const [books, setBooks] = useState([]);

  useEffect(() => {
    getBooks().then(setBooks);
  }, []);

  const handleRent = async (bookId) => {
    await rentBook(bookId);
    alert('Book rented!');
    getBooks().then(setBooks);
  };

  return (
    <div>
      <h2>Rent Books</h2>
      <ul>
        {books.filter(b => b.status === 'Available').map(book => (
          <li key={book.id}>
            {book.title} <button onClick={() => handleRent(book.id)}>Rent</button>
          </li>
        ))}
      </ul>
    </div>
  );
}

