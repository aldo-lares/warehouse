# Warehouse API - Authentication & Authorization Implementation

## Overview
Complete implementation of JWT-based authentication and role-based authorization system for a .NET 8 WebAPI.

## Features Implemented

### 🔐 Authentication
- **JWT Token Generation**: Secure token creation with custom claims
- **BCrypt Password Hashing**: Industry-standard password security
- **Login Endpoint**: `/api/auth/login` with email/password validation
- **User Information Endpoint**: `/api/auth/me` for authenticated user details

### 🔒 Authorization
- **Role-Based Access Control**: Admin, User, and Viewer roles
- **Authorization Policies**: Granular permission control
- **Protected Endpoints**: Different access levels per endpoint

### 📊 API Endpoints

#### Authentication Endpoints
- `POST /api/auth/login` - User login
- `GET /api/auth/me` - Current user information

#### Warehouse Endpoints
- `GET /api/warehouse/inventory` - View inventory (authenticated users)
- `POST /api/warehouse/inventory` - Add inventory item (User+ role)
- `DELETE /api/warehouse/inventory/{id}` - Delete inventory item (Admin only)
- `GET /api/warehouse/admin/stats` - System statistics (Admin only)

### 🧪 Testing
- **20 Comprehensive Tests**: 100% pass rate
- **Unit Tests**: AuthService functionality
- **Integration Tests**: Full API endpoint testing
- **Authorization Tests**: Role-based access validation

### 🔧 Technical Stack
- **.NET 8**: Latest LTS framework
- **JWT Bearer Authentication**: Industry standard
- **Entity Framework Core**: In-memory database
- **BCrypt.Net**: Secure password hashing
- **Swagger/OpenAPI**: Complete API documentation
- **xUnit**: Testing framework

## Test Users
- **Admin**: admin@warehouse.com / admin123 (Admin, User roles)
- **User**: user@warehouse.com / user123 (User role)

## Security Features
- ✅ Secure password hashing with BCrypt
- ✅ JWT tokens with expiration (24 hours)
- ✅ Role-based authorization
- ✅ Input validation and sanitization
- ✅ Proper error handling
- ✅ Swagger UI with JWT bearer token support

## Test Coverage
All authentication and authorization flows are covered:
- ✅ Valid/invalid login attempts
- ✅ Token validation
- ✅ Role-based access control
- ✅ Unauthorized/Forbidden responses
- ✅ User information retrieval

## API Documentation
Complete Swagger documentation available at `/swagger` when running in development mode.

## Getting Started
```bash
cd WarehouseAPI
dotnet run
```

Then navigate to `http://localhost:5000/swagger` to explore the API.