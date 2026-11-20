# Educational Platform API

An ASP.NET Core Web API for managing an educational platform that connects students, teachers, and educational centers. The backend provides account management, role-based access, academic resources, scheduling, enrollments, and online payment workflows.

## Overview

The platform is organized around three main user roles:

- **Admin** — manages educational centers.
- **Center** — manages subjects, students, schedules, learning materials, messages, and teachers.
- **Student** — accesses student-oriented services and can enroll in subjects and initiate payments.

> This repository contains the backend API. A separate frontend application is not included in this solution.

## Features

- User registration and login using ASP.NET Core Identity.
- JWT bearer authentication and role-based authorization.
- Separate access policies for Admin, Center, and Student roles.
- Center management.
- Subject and teacher management.
- Student management.
- Schedule and lesson-material management, including material uploads.
- Messaging between platform users.
- Enrollment and payment tracking.
- OTP-based account operations, including password and phone-number workflows.
- Paymob integration for card and mobile-wallet payment flows.
- Fawaterak integration for invoice and payment-method workflows.
- Payment callback and webhook endpoints.
- Swagger/OpenAPI support for API exploration.

## Technology Stack

| Area | Technologies |
|---|---|
| Language | C# |
| Framework | ASP.NET Core Web API (.NET 8) |
| Data access | Entity Framework Core 8 |
| Database | Microsoft SQL Server |
| Authentication | ASP.NET Core Identity, JWT Bearer |
| Object mapping | AutoMapper |
| API documentation | Swagger / Swashbuckle |
| Payment providers | Paymob, Fawaterak |

## Solution Architecture

The solution is separated into four projects:

```text
EducationalPlatform/
├── Api.PL/          # HTTP API, controllers, configuration and dependency injection
├── Core/            # Domain entities, contracts and specifications
├── Repository/      # EF Core DbContext, configurations and data access
├── Service/         # Business services and external integrations
└── EducationalPlatform.sln
```

### Project responsibilities

- **Api.PL** — exposes REST endpoints, handles HTTP requests, configures authentication and authorization, and registers application dependencies.
- **Core** — contains the core entities and abstractions used across the application.
- **Repository** — implements persistence using Entity Framework Core and SQL Server.
- **Service** — contains business logic, OTP/email functionality, token services, and payment-provider integrations.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Microsoft SQL Server
- Visual Studio 2022 or another .NET-compatible IDE
- Paymob and/or Fawaterak credentials if you want to test payment functionality

### 1. Clone the repository

```bash
git clone https://github.com/MohamedBadawy23/EducationalPlatform.git
cd EducationalPlatform
```

### 2. Configure application settings

Configure the required values using .NET User Secrets or environment variables. Do not commit real credentials, API keys, connection strings, or production secrets to source control.

The application reads configuration for the following integrations:

- `ConnectionStrings:DefaultConnection` — SQL Server connection string.
- `Jwt:key`, `Jwt:Issuer`, `Jwt:Audience` — JWT signing and validation settings.
- `Paymob:APIKey`, `Paymob:PublicKey`, `Paymob:HMAC`, `Paymob:CardIntegrationId`, `Paymob:MobileIntegrationId` — Paymob settings.
- `Fawaterk` — Fawaterak provider options.

Example connection string (replace the server and database values for your environment):

```text
Server=.;Database=EducationalPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;
```

For local development, initialize User Secrets for the API project:

```bash
dotnet user-secrets init --project Api.PL/Api.PL.csproj
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING" --project Api.PL/Api.PL.csproj
dotnet user-secrets set "Jwt:key" "YOUR_LONG_RANDOM_SIGNING_KEY" --project Api.PL/Api.PL.csproj
dotnet user-secrets set "Jwt:Issuer" "YOUR_ISSUER" --project Api.PL/Api.PL.csproj
dotnet user-secrets set "Jwt:Audience" "YOUR_AUDIENCE" --project Api.PL/Api.PL.csproj
```

Add the payment-provider settings in the same way if you are testing those integrations.

### 3. Restore dependencies

```bash
dotnet restore EducationalPlatform.sln
```

### 4. Apply database migrations

The API currently runs Entity Framework Core migrations during application startup. Ensure the SQL Server instance and connection string are available before starting the API.

You can also apply migrations explicitly:

```bash
dotnet ef database update --project Repository/Repository.csproj --startup-project Api.PL/Api.PL.csproj
```

### 5. Run the API

```bash
dotnet run --project Api.PL/Api.PL.csproj
```

When running in the Development environment, Swagger UI is enabled for exploring the available endpoints.

## Authentication and Authorization

The API uses ASP.NET Core Identity for user management and JWT bearer tokens for API authentication.

Protected endpoints require the token in the HTTP Authorization header:

```http
Authorization: Bearer YOUR_ACCESS_TOKEN
```

Authorization is role-based. The application defines the following roles:

- `Admin`
- `Center`
- `Student`

Authenticate through the account endpoints and use the returned access token when calling protected endpoints.

## Payment Integrations

### Paymob

The student payment flow supports card and mobile-wallet integration IDs. The API creates a payment token and returns a redirect URL. Payment callback and server-to-server callback endpoints are also implemented.

### Fawaterak

The API includes endpoints for creating invoices, retrieving payment methods, initiating payments, and receiving payment-provider webhook notifications.

Payment integrations require valid provider credentials and correctly configured callback/webhook URLs. Use sandbox credentials for development and testing.

## API Documentation

Run the API in the Development environment and open the Swagger UI URL printed by the application. Use the **Authorize** control to provide a JWT when testing protected endpoints.

The main API areas include:

- Account and authentication
- Admin and center management
- Teachers and students
- Subjects, schedules, and learning materials
- Messages and enrollments
- Paymob and Fawaterak payments and webhooks

## Security Notes

- Never commit real secrets, API keys, passwords, or production connection strings.
- Use environment variables or .NET User Secrets for local configuration and a secure secrets manager in production.
- Keep payment webhook signature/HMAC validation enabled.
- Do not return user passwords in API responses.
- Do not use hard-coded administrator credentials in production. Provision privileged accounts through a secure deployment or administration process.
- Use HTTPS outside local development.

## Current Scope

This README documents the implementation visible in the repository. Payment-provider behavior, deployment configuration, and endpoint contracts should be validated in the target environment before production use.

## Author

**Mohamed Badawy**

GitHub: [@MohamedBadawy23](https://github.com/MohamedBadawy23)
