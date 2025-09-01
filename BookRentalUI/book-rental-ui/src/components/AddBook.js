import React, { useState } from 'react';
import { addBook } from '../Services/bookService';

export default function AddBook() {
  const [title, setTitle] = useState('');
  const [author, setAuthor] = useState('');
  const [genre, setGenre] = useState('');
  const [isbn, setIsbn] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    await addBook({ title, author, genre, isbn });
    setTitle(''); setAuthor(''); setGenre(''); setIsbn('');
    alert('Book added!');
  };

  return (
    <form onSubmit={handleSubmit}>
      <input placeholder="Title" value={title} onChange={e => setTitle(e.target.value)} required />
      <input placeholder="Author" value={author} onChange={e => setAuthor(e.target.value)} required />
      <input placeholder="Genre" value={genre} onChange={e => setGenre(e.target.value)} required />
      <input placeholder="ISBN" value={isbn} onChange={e => setIsbn(e.target.value)} required />
      <button type="submit">Add Book</button>
    </form>
  );
}