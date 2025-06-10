# Books Complex API (.NET 8)

A multi‑project, layered solution:

* **Books.Domain** – entities and repository abstractions
* **Books.Infrastructure** – EF Core DbContext + generic repository & unit‑of‑work implementations
* **Books.Api** – ASP.NET Core Web API (controllers, DTOs)

<div align="center"><strong>No MediatR, AutoMapper, FluentValidation, or similar packages used.</strong></div>

## Run locally

```bash
dotnet restore
dotnet run --project Books.Api
```

Swagger UI will be available at `https://localhost:5001/swagger`.

## Highlights

* Generic repository + Unit‑of‑Work for clean separation.
* Paging & filtering on **GET /api/books** (`page`, `pageSize`, optional release‑date window).
* Full CRUD for books (GET/id, POST, PUT, DELETE).
* Publishing house auto‑creation when posting books.
* In‑memory provider by default for quick start – swap to SQL provider by changing DI.

Generated 2025-06-10T09:47:15.273770 UTC
