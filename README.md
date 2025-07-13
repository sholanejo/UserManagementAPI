# UserManagementAPI

## Project Overview

UserManagementAPI is a secure ASP.NET Core Web API for managing users. It supports:
- User registration, update, soft-delete, and restoration
- JWT-based authentication
- Role-based authorization (Admin only actions)
- Basic search and pagination
- Token validation
- Rate limiting for sensitive endpoints

---

##  Setup Instructions

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server or SQLite
- Optional: Postman or Swagger UI for testing

### Clone and Run
```bash
git clone https://github.com/your-username/UserManagementAPI.git
cd UserManagementAPI
dotnet build
dotnet ef database update   # if using EF Core migrations
dotnet run

### Configuration
Update your appsettings.Development.json or secrets with:

json
Copy
Edit
{
  "JwtSettings": {
    "Secret": "YourSuperSecretKey",
    "Issuer": "UserManagementAPI",
    "Audience": "UserManagementClient",
    "ExpiryMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Your DB connection string"
  }
}

API Usage
AuthController
POST /api/auth/login

{
  "email": "admin@system.com",
  "password": "StrongPassword1$"
}
POST /api/auth/validate
"eyJhbGciOiJIUzI1NiIsIn..."

UsersController (Requires JWT Bearer Token)
GET /api/users?page=1&pageSize=10&search=tim
GET /api/users/{id}
POST /api/users
Role: Admin
{
  "firstName": "Tim",
  "lastName": "Cook",
  "email": "tim@apple.com",
  "password": "Apple123!",
  "role": "Admin"
}
PUT /api/users/{id}
Role: Admin

DELETE /api/users/{id}
Role: Admin + Rate Limited

POST /api/users/{id}/restore
Role: Admin + Rate Limited

GET /api/users/profile

Test Instructions
Swagger
Run the API.

Navigate to https://localhost:{port}/swagger.

Postman
Import UserManagementAPI.postman_collection.json.

Set environment variable: base_url = https://localhost:{port}.

Authenticate via /auth/login, then use token for secured routes.

