\# Devices API



A REST API for managing device resources, built with .NET 9 and Clean Architecture.



\## Architecture



The solution follows Clean Architecture principles, organized into four layers:



```

src/

├── DevicesAPI.Domain/          — Entities, enums, repository interfaces (no dependencies)

├── DevicesAPI.Application/     — DTOs, service interfaces, business logic, mapping

├── DevicesAPI.Infrastructure/  — EF Core DbContext, repository implementations, migrations

└── DevicesAPI.API/             — Controllers, middleware, Swagger configuration

tests/

└── DevicesAPI.UnitTests/       — Unit tests for service, controller, mapping, and middleware

```



Dependencies flow inward: `API → Application → Domain ← Infrastructure`.



\## Tech Stack



\- \*\*Runtime\*\*: .NET 9 / C# 13

\- \*\*Database\*\*: PostgreSQL 17 (via EF Core + Npgsql)

\- \*\*Documentation\*\*: Swagger / Swashbuckle

\- \*\*Containerization\*\*: Docker + Docker Compose

\- \*\*Testing\*\*: xUnit, Moq, FluentAssertions



\## Prerequisites



\- \[Docker Desktop](https://www.docker.com/products/docker-desktop/)



That's it — no local .NET SDK or PostgreSQL installation required to run the application.



\## Running the Application



```bash

docker-compose up --build

```



The API will be available at `http://localhost:8080`.  

Swagger UI: `http://localhost:8080/swagger`



Database migrations are applied automatically on startup.



To stop the application and preserve data:

```bash

docker-compose down

```



To stop and wipe the database:

```bash

docker-compose down -v

```



\## API Endpoints



| Method | Endpoint | Description |

|--------|----------|-------------|

| `POST` | `/api/devices` | Create a new device |

| `GET` | `/api/devices/{id}` | Get a device by ID |

| `GET` | `/api/devices` | Get all devices |

| `GET` | `/api/devices?brand={brand}` | Filter devices by brand |

| `GET` | `/api/devices?state={state}` | Filter devices by state |

| `PUT` | `/api/devices/{id}` | Fully update a device |

| `PATCH` | `/api/devices/{id}` | Partially update a device |

| `DELETE` | `/api/devices/{id}` | Delete a device |



\### Device States



| Value | Description |

|-------|-------------|

| `Available` | Device is free to use |

| `InUse` | Device is currently in use |

| `Inactive` | Device is inactive |



\### Business Rules



\- `CreationTime` is set server-side on creation and cannot be modified.

\- `Name` and `Brand` cannot be updated while a device is in `InUse` state (returns `409 Conflict`).

\- Devices in `InUse` state cannot be deleted (returns `409 Conflict`).



\### Error Responses



All errors follow the \[RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) Problem Details format:



```json

{

&#x20; "type": "https://httpstatuses.io/404",

&#x20; "title": "Not Found",

&#x20; "status": 404,

&#x20; "detail": "Device with ID '...' was not found."

}

```



\## Running Tests



Requires the \[.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).



```bash

dotnet test tests/DevicesAPI.UnitTests

```



\## Local Development (without Docker)



1\. Ensure you have a PostgreSQL instance running locally.

2\. Update the connection string in `src/DevicesAPI.API/appsettings.Development.json`.

3\. Run the API:

&#x20;  ```bash

&#x20;  dotnet run --project src/DevicesAPI.API

&#x20;  ```



\## Future Improvements



\- \*\*Pagination\*\* — list endpoints should support `page` and `pageSize` query parameters for large datasets.

\- \*\*API Versioning\*\* — support multiple API versions simultaneously via URL or header versioning.

\- \*\*Authentication \& Authorization\*\* — protect endpoints with JWT Bearer tokens.

\- \*\*Rate Limiting\*\* — prevent abuse using ASP.NET Core's built-in rate limiting middleware.

\- \*\*Structured Logging\*\* — replace the default logger with Serilog for JSON-structured logs, compatible with log aggregation platforms (e.g. Grafana Loki, Datadog).

\- \*\*Health Check Endpoints\*\* — expose `/health` and `/health/ready` for liveness and readiness probes.

\- \*\*CI/CD Pipeline\*\* — GitHub Actions workflow to build, test, and push a Docker image on every push to `main`.

\- \*\*Integration Tests\*\* — end-to-end tests using `WebApplicationFactory` and Testcontainers (real PostgreSQL, no mocking).

\- \*\*Soft Deletes\*\* — instead of hard-deleting records, mark them as deleted with a `DeletedAt` timestamp for auditability.

