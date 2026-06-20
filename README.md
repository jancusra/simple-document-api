# Simple Document API - multiple storages & response formats

A small ASP.NET Core (.NET 8) service that stores documents (sent as JSON) and returns them in
different formats (JSON / XML / MessagePack) selected via the `Accept` header. The underlying
storage is pluggable (in-memory or SQL Server) and switchable through configuration.

---

## Quick start (from the repository root)

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Optional) SQL Server — only if you run in `sqlserver` storage mode

### Run the app
```powershell
dotnet run --project src\App.Web\App.Web.csproj
```

The app starts in the **Development** profile on port **5000**:

| What | URL |
|------|-----|
| API base | `http://localhost:5000` |
| Swagger UI (Development only) | `http://localhost:5000/swagger` |
| OpenAPI document | `http://localhost:5000/swagger/v1/swagger.json` |
| Health check | `http://localhost:5000/health` |
| Liveness / home | `http://localhost:5000/home` |

> Tip: the quickest way to run without any database is **in-memory** mode — set `"StorageType": "memory"`
> in [appsettings.json](src/App.Web/appsettings.json), or override it for one session:
> ```powershell
> $env:StorageType = "memory"; dotnet run --project src\App.Web\App.Web.csproj
> ```

### Run the tests
```powershell
dotnet test src\App.Api.sln
```
**24 tests** (13 persistence unit tests + 11 web integration tests). Integration tests always use
in-memory storage, so they need no database and are independent of `appsettings.json`.

### Build only
```powershell
dotnet build src\App.Api.sln
```

---

## Storage configuration

Storage is selected by the `StorageType` key in [appsettings.json](src/App.Web/appsettings.json):

```json
"StorageType": "memory"     // or "sqlserver"
```

- **`memory`** — no setup required.
- **`sqlserver`** — the database and table are created automatically on startup (`EnsureCreated`).
  The connection string is **not** stored in the repository, so credentials never get committed.
  Provide it in one of two ways:

  **Option A — environment variable (quick, current session only):**
  ```powershell
  $env:ConnectionString = "Data Source=localhost;Initial Catalog=AppApi;User ID=sa;Password=YOUR_PASSWORD;TrustServerCertificate=true"
  dotnet run --project src\App.Web\App.Web.csproj
  ```

  **Option B — user-secrets (set once, then just `dotnet run`):**
  ```powershell
  # one-time: enable user-secrets for the web project (adds a UserSecretsId to the .csproj)
  dotnet user-secrets init --project src\App.Web

  # one-time: store the connection string outside the repo
  dotnet user-secrets set "ConnectionString" "Data Source=localhost;Initial Catalog=AppApi;User ID=sa;Password=YOUR_PASSWORD;TrustServerCertificate=true" --project src\App.Web

  # from now on it is picked up automatically in Development
  dotnet run --project src\App.Web\App.Web.csproj
  ```
  The secret is stored in your user profile (`%APPDATA%\Microsoft\UserSecrets\<id>\secrets.json`), not in
  the repository, and `Host.CreateDefaultBuilder` loads it automatically while the environment is
  `Development`. Useful commands: `dotnet user-secrets list` / `dotnet user-secrets clear` (both with
  `--project src\App.Web`).

---

## Example requests

**Create a document (POST, JSON):**
```http
POST http://localhost:5000/documents
Content-Type: application/json

{
  "id": "3f2504e0-4f89-41d3-9a0c-0305e82c3301",
  "tags": ["important", ".net"],
  "data": { "some": "data", "optional": "fields", "nested": { "any": "schema" } }
}
```

**Retrieve it in XML (GET, by id):**
```http
GET http://localhost:5000/documents/3f2504e0-4f89-41d3-9a0c-0305e82c3301
Accept: application/xml
```

**Update it (PUT):** same body shape as POST. Supported `Accept` values: `application/json`,
`application/xml`, `application/x-msgpack`.

Error responses share one shape (`ResultCode` + `ResultReason`) and are returned in the requested
format: `400` (missing mandatory field), `404` (document not found), `409` (document already exists).

---

## How the assignment requirements are met

**1. Documents are sent as a JSON POST payload and modified via PUT.**
`GET` / `POST` / `PUT` on `/documents` in [DocumentsController.cs](src/Api/App.Endpoint/Controllers/DocumentsController.cs).
POST returns `409` for a duplicate id, PUT returns `404` for a missing document, both validate the
mandatory fields (`400`).

**2. Documents can be returned in different formats (XML, MessagePack, etc.).**
Content negotiation by `Accept` header: JSON (`application/json`), XML (`application/xml`) and
MessagePack (`application/x-msgpack`) — for both success and error responses.

**3. Easy to add support for new formats.**
Formatters are registered in one place in [Startup.cs](src/App.Web/Startup.cs) (library-based or a
custom formatter) — no controller changes needed.

**4. Easy to add a different underlying storage.**
Strategy pattern over `IDataProvider` ([DataProviderManager.cs](src/Infrastructure/App.Persistence/DataProvider/DataProviderManager.cs)).
Switching between `memory` and `sqlserver` is a config change; adding a new store (cloud, HDD, …) is a
new provider class + enum value.

**5. Built for very high load (mostly reading).**
Fully async pipeline; SQL reads use `AsNoTracking`; responses are compressed (Brotli/Gzip); writes are
**atomic** (no check-then-write race under concurrency); the in-memory store can be optionally bounded
via a `CacheSizeLimit` config key.

**6. Unit tests.**
24 tests targeting the actual code: in-memory and SQL providers (the latter via EF Core InMemory),
all three response formats, error paths (`400`/`404`/`409`), and a concurrency test proving atomic
inserts. See [src/Tests](src/Tests).

**7. Mandatory fields `id`, `tags`, `data` with an arbitrary `data` schema.**
`id`, `tags`, `data` are validated as mandatory. `data` is a schema-free JSON object
([DocumentDto.cs](src/Core/App.Contracts/App.Contracts/Models/DocumentDto.cs)) — it accepts nested
objects, arrays, numbers and booleans. In JSON the structure is preserved natively; in XML and
MessagePack (which cannot represent an arbitrary schema) the data is carried as a JSON-encoded value
and round-trips intact.

> Note: `id` is modeled as a `Guid` (a unique identifier), not a free-form string.

---

## Project structure

```
src/
├─ Api/App.Endpoint            # Controllers (API surface)
├─ Core/
│  ├─ App.Domain               # Entities, exceptions, error middleware, responses
│  ├─ App.Contracts            # DTOs (wire models)
│  └─ App.Mapper               # DTO <-> entity mapping
├─ Infrastructure/App.Persistence  # Data providers (memory / SQL Server), EF DbContext
├─ App.Web                     # Host (Program/Startup, configuration, pipeline)
└─ Tests/                      # xUnit unit + integration tests
```

Cross-cutting error handling lives in `ErrorWrappingMiddleware` (logs unexpected exceptions, returns a
consistent formatted error body).
