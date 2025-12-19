# Secondary School Student Records & Grade Management API

Backend API for the Secondary School Student Records & Grade Management Web Application.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (version 12 or higher)
- A code editor (Visual Studio, VS Code, or Rider)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/TinhNguyenTrung09092004/Secondary-School-Student-Records-Grade-Management-Web-Application.git
cd api
```

2. Restore dependencies:
```bash
dotnet restore
```

## Configuration

1. Copy the example environment file:
```bash
cp env.example .env
```

2. Update the `.env` file with your configuration:

### Database Configuration
```
DB_HOST=localhost
DB_PORT=5432
DB_NAME=school_management
DB_USER=your_postgres_user
DB_PASSWORD=your_postgres_password
```

### JWT Configuration
```
JWT_KEY=your_secret_key_here_at_least_32_characters
JWT_ISSUER=https://localhost:7000
JWT_AUDIENCE=https://localhost:4200
```

### OAuth Configuration (Optional)
```
GOOGLE_CLIENT_ID=your_google_client_id
GOOGLE_CLIENT_SECRET=your_google_client_secret
MICROSOFT_CLIENT_ID=your_microsoft_client_id
MICROSOFT_CLIENT_SECRET=your_microsoft_client_secret
```

### Email Configuration (Optional)
```
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_app_password
SMTP_FROM_EMAIL=School@gmail.com
SMTP_FROM_NAME=School Management System
```

### Frontend URL
```
FRONTEND_URL=http://localhost:4200
```

## Database Setup

The application automatically runs migrations and seeds initial data on startup. Ensure your PostgreSQL server is running and the connection details in `.env` are correct.

To manually run migrations:
```bash
dotnet ef database update
```

## Running the Application

### Development Mode
```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:7000`

### Production Mode
```bash
dotnet run --configuration Release
```

## API Documentation

When running in development mode, you can access the Swagger UI documentation at:
```
https://localhost:7000/swagger
```

This provides interactive API documentation where you can test all endpoints.

## Authentication

The API uses JWT Bearer token authentication. To authenticate:

1. Register or login through the `/api/auth/register` or `/api/auth/login` endpoints
2. Include the JWT token in the Authorization header for protected endpoints:
```
Authorization: Bearer <your_token_here>
```

OAuth authentication is also available through Google and Microsoft.

## Key Features

- **User Management**: Authentication, authorization, and role-based access control
- **Student Management**: Student records and information
- **Teacher Management**: Teacher profiles and department assignments
- **Class Management**: Class creation and student assignments
- **Grade Management**: Academic performance tracking and grading
- **Subject Management**: Subject definitions and configurations
- **School Year & Semester Management**: Academic calendar management
- **Conduct & Behavior Tracking**: Student conduct and behavior records
- **Academic Rankings**: Performance rankings and statistics
- **Backup Service**: Automated database backup scheduling
- **Email Notifications**: SMTP-based email service
- **Face Recognition**: Student identification service

## Project Structure

```
API/
├── Controllers/      # API endpoint controllers
├── Models/          # Database entity models
├── DTOs/            # Data transfer objects
├── Services/        # Business logic services
├── Repositories/    # Data access layer
├── Data/            # Database context and configurations
├── Middleware/      # Custom middleware components
├── Helpers/         # Utility and helper classes
├── Migrations/      # Entity Framework migrations
└── Program.cs       # Application entry point
```

## Development

### Adding Migrations
```bash
dotnet ef migrations add MigrationName
```

### Rolling Back Migrations
```bash
dotnet ef database update PreviousMigrationName
```

### Building the Project
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

## CORS Configuration

The API is configured to allow requests from `http://localhost:4200` (Angular frontend). Update the CORS policy in `Program.cs` if you need to allow different origins.

## Troubleshooting

### Database Connection Issues
- Ensure PostgreSQL is running
- Verify database credentials in `.env`
- Check if the database exists and user has proper permissions

### Authentication Issues
- Ensure JWT_KEY is at least 32 characters long
- Verify JWT_ISSUER and JWT_AUDIENCE match your configuration
- Check token expiration settings

### Migration Issues
- Delete the Migrations folder and database, then recreate:
```bash
dotnet ef database drop
dotnet ef migrations add Initial
dotnet ef database update
```
