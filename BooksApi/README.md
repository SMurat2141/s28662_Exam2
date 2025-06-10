# Books API (.NET 8)

Simple REST API built with ASP.NET Core 8.0 and Entity Framework Core (Code‑First).  
No external CQRS helpers, mapping libraries, or validation frameworks were used—just plain EF Core and MVC controllers.

## Running locally

```bash
dotnet restore
dotnet run --project BooksApi
```

The API starts on **https://localhost:5001** (or the next available port).  
Swagger UI is enabled in `Development` environment.

## Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/books?fromReleaseDate=2024-01-01&toReleaseDate=2024-12-31` | Returns books filtered by release date. Sorted by release date (desc) then publishing house (asc). |
| POST | `/api/books` | Adds a new book. Creates the publishing house automatically if it doesn't exist. |

See `Models/DTOs` for request/response contracts.

---

Generated 2025-06-10T09:42:07.521669 UTC
