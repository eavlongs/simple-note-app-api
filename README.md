# Simple Note App API

A RESTful API built with .NET 8 for managing notes with user authentication and authorization.

## Features

- **User Authentication**: JWT-based authentication with BCrypt password hashing
- **Note Management**: Create, read, update, and delete notes
- **Database Migrations**: FluentMigrator for database schema management
- **API Documentation**: Swagger/OpenAPI integration
- **CORS Support**: Cross-origin resource sharing enabled
- **SQL Server**: Microsoft SQL Server database integration

## Prerequisites

Before running the application, ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or full version)
- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (optional, for development)

## Database Setup

The application uses SQL Server with the following default connection string:
```
Server=localhost;Database=simple_note_app;Trusted_Connection=true;TrustServerCertificate=True;
```

Make sure your SQL Server is running and accessible. The database will be created automatically when you run migrations.

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd simple-note-app-api
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Database Connection (Optional)

If you need to modify the database connection, update the `ConnectionStrings:DefaultConnection` in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server connection string here"
  }
}
```

### 4. Run Database Migrations

Create and populate the database tables:

```bash
dotnet run -- --migrate-only
```

This will:
- Create the `Users` table
- Create the `Notes` table
- Set up the necessary database schema

### 5. Run the Application

#### Option A: Using the provided batch file (Windows)
```bash
run.bat
```

#### Option B: Using .NET CLI
```bash
dotnet run
```

#### Option C: With migrations (first-time setup)
```bash
dotnet run -- --migrate
```

## Application URLs

Once the application is running, you can access:

- **HTTP**: http://localhost:5233
- **HTTPS**: https://localhost:7095
- **Swagger UI**: https://localhost:7095/swagger (in Development mode)

## CLI Arguments

The application supports the following command-line arguments:

- `--migrate`: Run database migrations and then start the application
- `--migrate-only`: Run database migrations only and exit (don't start the web server)

Examples:
```bash
# Run migrations and start the app
dotnet run -- --migrate

# Run migrations only
dotnet run -- --migrate-only

# Start the app without migrations
dotnet run
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login user

### Notes
- `GET /api/notes` - Get all notes for authenticated user
- `GET /api/notes/{id}` - Get specific note by ID
- `POST /api/notes` - Create a new note
- `PUT /api/notes/{id}` - Update existing note
- `DELETE /api/notes/{id}` - Delete note

## Configuration

### JWT Settings

JWT configuration is in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "ExpirationMinutes": 120,
    "RefreshTokenExpirationInMinutes": 20160
  }
}
```

### Environment Variables

The application uses the `ASPNETCORE_ENVIRONMENT` environment variable:
- `Development`: Enables Swagger UI and detailed error messages
- `Production`: Optimized for production deployment

## Development

### Building the Project

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Project Structure

```
├── Controller/           # API controllers
│   ├── AuthController.cs
│   └── NoteController.cs
├── Dto/                 # Data transfer objects
├── Migrations/          # Database migrations
├── Models/              # Entity models
├── Repository/          # Data access layer
├── Services/            # Business logic layer
├── Settings/            # Configuration classes
└── Properties/          # Launch settings
```

## Technologies Used

- **Framework**: .NET 8
- **Database**: SQL Server with Dapper ORM
- **Authentication**: JWT Bearer tokens
- **Password Hashing**: BCrypt.Net
- **Migrations**: FluentMigrator
- **API Documentation**: Swagger/OpenAPI
- **Dependency Injection**: Built-in .NET DI container

### Development Certificate Issues

If you encounter HTTPS certificate issues during development:

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```