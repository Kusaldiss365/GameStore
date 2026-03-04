# GameStore API

A .NET Web API that integrates with the FreeToGame API and stores
retrieved game data in a SQL Server database for caching.

The API first attempts to retrieve data from the local database. If the
database cache is incomplete, the system fetches data from the
FreeToGame API and stores it locally.

------------------------------------------------------------------------

## Technologies Used

-   .NET 10
-   ASP.NET Core Web API
-   SQL Server
-   ADO.NET (`Microsoft.Data.SqlClient`)
-   Swagger / OpenAPI (`Swashbuckle.AspNetCore`)

------------------------------------------------------------------------

## Library and Framework Choices

**Microsoft.Data.SqlClient (ADO.NET)**  
Used to connect to SQL Server and execute parameterized SQL queries. This satisfies the assignment requirement to avoid using an ORM and instead interact with the database using raw SQL commands.

**Swashbuckle.AspNetCore (Swagger / OpenAPI)**  
Used to automatically generate API documentation and provide an interactive Swagger UI. This allows easy testing and exploration of API endpoints during development.

**HttpClient (Typed HttpClient via Dependency Injection)**  
Used to communicate with the FreeToGame public API. Registering it as a typed HttpClient allows centralized configuration of the API base URL and timeout, while enabling efficient connection reuse.

**ILogger (ASP.NET Core Logging)**  
Used to log important application events such as database cache hits/misses, external API calls, HTTP response statuses and records inserted into the database. This improves traceability and debugging.

**Microsoft.AspNetCore.HttpLogging**  
Used to automatically log HTTP request and response information such as request method, path, status code and duration. This helps monitor API activity during development.

------------------------------------------------------------------------

## Project Architecture

    Controller -> Service -> Repository -> SQL Server

    Service -> FreeToGame API

### Layers

1.  **Controller**\
    Handles API requests and returns API responses.

2.  **Service**\
    Manages business logic, logging, caching decisions and external API
    calls.

3.  **Repository**\
    Interacts with SQL Server using ADO.NET and raw SQL queries.

------------------------------------------------------------------------

## External API

Game data is retrieved from:

https://www.freetogame.com/api/

Endpoints used:

    GET /games?platform=pc
    GET /game?id={id}

------------------------------------------------------------------------

## Database Setup

Create a SQL Server database named:

    GameStoreDb

Connection string used in `appsettings.json`:

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=GameStoreDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
  }
}
```

Create the following table:

``` sql
CREATE TABLE Games (
    Id INT PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Thumbnail NVARCHAR(500),
    ShortDescription NVARCHAR(MAX),
    GameUrl NVARCHAR(500),
    Genre NVARCHAR(100),
    Platform NVARCHAR(100),
    Publisher NVARCHAR(150),
    Developer NVARCHAR(150),
    ReleaseDate DATE NULL,
    FreeToGameProfileUrl NVARCHAR(500)
);
```

------------------------------------------------------------------------

## API Endpoints

### 1. Get All Games

    GET /api/GameCatalog

If the database cache is incomplete, the API fetches games from
FreeToGame and stores them locally.

### 2. Get Cached Games Only

    GET /api/GameCatalog/cached

### 3. Get Game By ID

    GET /api/GameCatalog/{id}

### 4. Clear Database Cache

    DELETE /api/GameCatalog/clear

### 5. Test Database Connection

    GET /api/GameCatalog/test-db

------------------------------------------------------------------------

## Cache Strategy

The service uses a simple cache threshold:

    CacheThreshold = 50

### Behavior

-   If the database contains **50 or more games**, results are served
    from the database.
-   If the database contains **fewer than 50 games**, the system calls
    the FreeToGame API and stores the results locally.

This prevents repeated external API calls once the database cache is
populated.

------------------------------------------------------------------------

## Why a Cache Threshold is Used

The database can become **partially populated** if the system is used
only via the `GET /api/GameCatalog/{id}` endpoint.

For example, requesting 2 or 3 games by ID will cache only those records.
If the "get all" endpoint relied only on `result.Count > 0`, it would
incorrectly treat the cache as complete and return only the few cached
records.

To avoid that, the service uses a **CacheThreshold (currently 50)**. If
the database contains fewer than 50 games, it is treated as incomplete,
and the service triggers a full refresh from the FreeToGame API when
calling `GET /api/GameCatalog`.

This ensures `GET /api/GameCatalog` returns a meaningful dataset and
prevents the API from getting stuck serving only a small partially
cached subset.

------------------------------------------------------------------------

## Logging

Logs important operations using `ILogger`.

Includes:

-   Method execution
-   Database cache hits
-   Database cache misses
-   External API calls
-   Response status codes
-   Records saved to the database

------------------------------------------------------------------------

## Running the Application

From the project directory:

    dotnet restore
    dotnet build
    dotnet run

Open Swagger UI in the browser:

    https://localhost:<port>/swagger

------------------------------------------------------------------------

## Notes

-   The repository layer avoids ORM frameworks and performs manual
    mapping using `SqlDataReader`.
-   All database queries use **parameterized SQL** to prevent SQL
    injection.
-   HTTP calls use a **typed HttpClient** registered in dependency
    injection.

------------------------------------------------------------------------

## Author

**Kusal Dissanayake**\
Software Engineer

Portfolio: https://kusal-dissanayake.vercel.app/
