# BookRental - Docker Installation Guide

This guide explains how to build and run the BookRental backend (ASP.NET Core) and frontend (React) using Docker.

## Prerequisites

- [Docker](https://www.docker.com/get-started) installed on your machine
- [Docker Compose](https://docs.docker.com/compose/) (usually included with Docker Desktop)

## 1. Clone the Repository

```sh
git clone https://github.com/yourusername/BookRent.git
cd BookRent
```

## 2. Configure Environment Variables

- Edit `BookRental/appsettings.json` and `BookRental/appsettings.Development.json` as needed (e.g., database connection string, JWT secrets).
- Make sure your connection string in `appsettings.json` points to the correct SQL Server instance.  
  If using Docker Compose, the default is usually fine.

## 3. Build and Run with Docker Compose

From the root directory (where `docker-compose.yaml` is located):

```sh
docker-compose up --build
```

This will:

- Build the backend (`BookRental/`) and frontend (`BookRentalUI/book-rental-ui/`) images
- Start the backend API, frontend UI, and a SQL Server database container

## 4. Access the Application

- **Frontend (React UI):**  
  Open [http://localhost:3000](http://localhost:3000) in your browser.
- **Backend (API):**  
  The API is available at [http://localhost:5000/api](http://localhost:5000/api)
- **Swagger API Docs:**  
  Visit [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

## 5. Stopping the Application

Press `Ctrl+C` in your terminal, then run:

```sh
docker-compose down
```

## 6. Troubleshooting

- If you get database connection errors, ensure the SQL Server container is healthy and the connection string matches the container name and credentials.
- For first-time setup, the backend will automatically run migrations to initialize the database.

---

**Note:**  
- The default credentials, ports, and secrets can be changed in the configuration files and `docker-compose.yaml`.
- For production, update secrets and consider using persistent storage for the database.

---

## File Reference

- [docker-compose.yaml](docker-compose.yaml)
- [BookRental/Dockerfile](BookRental/Dockerfile)
-