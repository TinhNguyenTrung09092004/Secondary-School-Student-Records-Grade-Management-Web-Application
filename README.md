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
JWT_ISSUER=https://localhost:5244
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
- HTTP: `http://localhost:5244`
- HTTPS: `https://localhost:7187`

### Production Mode
```bash
dotnet run --configuration Release
```

## API Documentation

When running in development mode, you can access the Swagger UI documentation at:
```
https://localhost:5244/swagger
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

### Authentication & Security
- **JWT Authentication**: Secure token-based authentication with role-based access control
- **OAuth Integration**: Google and Microsoft external login support
- **Password Management**:
  - Self-service password change for all authenticated users
  - Forgot password with email-based reset
  - Account setup flow - new users set their own passwords via email link
- **CAPTCHA Protection**: Automatic CAPTCHA requirement after failed login attempts
- **Face Recognition**: Biometric authentication and student identification

### User Management
- **Multi-Role Support**: Admin, Principal, AcademicAffairs, DepartmentHead, SubjectTeacher, Homeroom
- **User CRUD Operations**: Create, read, update users with email-based account setup
- **Role Management**: Assign and manage multiple roles per user
- **Account Lockout**: Manually lock/unlock user accounts
- **Scheduled Deletion**: 30-day soft delete with cancellation option
- **Account Cleanup**: Automatic removal of expired accounts and setup tokens
- **Audit Logging**: Comprehensive audit trail for all user management actions

### Academic Management
- **Student Management**: Complete student records with personal information, ethnicity, religion, occupation tracking
- **Teacher Management**: Teacher profiles with department and subject assignments
- **Class Management**: Class creation, student enrollment, and homeroom teacher assignment
- **Class Assignment**: Automated and manual student-to-class assignment
- **Teaching Assignment**: Department heads assign teachers to classes and subjects
- **Grade Management**: Multi-type grading (regular, midterm, final) with academic performance tracking
- **Conduct Grading**: Student behavior and conduct assessment
- **Academic Rankings**: Automated performance rankings and statistics
- **Subject Management**: Subject definitions with grade level associations
- **Department Management**: Subject department organization
- **School Year & Semester Management**: Academic calendar with term management
- **Result Management**: Comprehensive academic results tracking and reporting

### System Administration
- **Backup Service**:
  - Automated database backup scheduling (daily, weekly, monthly)
  - On-demand backup creation
  - Backup history tracking
- **System Settings**: Customizable school logo and branding
- **Principal Dashboard**: Overview statistics and system insights
- **Email Notifications**: Automated emails for:
  - Account setup and password reset
  - Account lockout/unlock notifications
  - Scheduled deletion warnings
  - Deletion cancellation confirmations

### Data Management
- **Ethnicity, Religion, Occupation**: Configurable reference data
- **Grade Levels**: Grade level definitions and management
- **Grade Types**: Customizable grade type configurations
- **Conduct Definitions**: Configurable conduct rating system
- **Academic Performance Levels**: Student performance classification

## API Endpoints

### Authentication (`/api/auth`)
- `POST /login` - User login with CAPTCHA support
- `POST /external-login` - OAuth login (Google/Microsoft)
- `POST /forgot-password` - Request password reset email
- `POST /reset-password` - Reset password with token
- `POST /complete-account-setup` - Complete new account setup
- `POST /change-own-password` - Change own password (authenticated users)
- `GET /captcha` - Generate CAPTCHA challenge

### User Management (`/api/usermanagement`) - Admin Only
- `GET /users` - Get all users
- `GET /users/{id}` - Get user by ID
- `POST /users` - Create new user (sends setup email)
- `PUT /users/{id}` - Update user information
- `DELETE /users/{id}` - Schedule user deletion (30-day soft delete)
- `POST /users/{id}/cancel-deletion` - Cancel scheduled deletion
- `POST /users/{id}/toggle-lockout` - Lock/unlock user account
- `GET /roles` - Get all roles with user counts
- `PUT /users/{id}/roles` - Update user roles

### Student Management (`/api/student`)
- Standard CRUD operations for student records
- Student search and filtering
- Class enrollment management

### Teacher Management (`/api/teacher`)
- Teacher profile management
- Department assignment
- Teaching history

### Class Management (`/api/class`)
- Class CRUD operations
- Student enrollment
- Homeroom teacher assignment
- Class results and statistics

### Grade Management (`/api/grade`)
- Grade entry and modification
- Grade type configuration
- Student grade history

### Academic Performance (`/api/academicperformance`)
- Performance level management
- Student performance tracking
- Performance statistics

### Conduct Management (`/api/conduct`)
- Conduct definition management
- Student conduct grading
- Conduct history

### Teaching Assignment (`/api/teachingassignment`)
- Assign teachers to classes and subjects
- View teaching schedules
- Department head management

### Subject & Department (`/api/subject`, `/api/department`)
- Subject CRUD operations
- Department management
- Subject-department associations

### School Organization (`/api/schoolyear`, `/api/semester`, `/api/gradelevel`)
- School year management
- Semester configuration
- Grade level definitions

### System Features
- `/api/backup` - Database backup management (Admin)
- `/api/systemsettings` - System configuration including logo settings (Admin)
- `/api/principaldashboard` - Dashboard statistics (Principal)
- `/api/facerecognition` - Face enrollment and recognition
- `/api/auditlog` - Audit log viewing (Admin)

### Reference Data
- `/api/ethnicity` - Ethnicity management
- `/api/religion` - Religion management
- `/api/occupation` - Occupation management
- `/api/gradetype` - Grade type management

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
├── Backups/         # Database backup storage
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
