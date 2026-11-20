
# Educational Platform API

A comprehensive **Educational Platform Backend API** built with **ASP.NET Core 8**, designed to manage educational centers, students, teachers, subjects, lessons, bookings, enrollments, and online payments through a secure and scalable RESTful architecture.

The platform provides role-based access control, JWT authentication, OTP-based account operations, and payment gateway integrations to support real-world educational workflows.

---

## 🚀 Key Features

### 🔐 Authentication & Authorization
- User registration and login.
- JWT-based authentication.
- Access token and refresh token generation.
- Role-based authorization using ASP.NET Core Identity.
- Support for Admin, Student, and Center roles.
- OTP-based account verification and password operations.

### 🏫 Educational Center Management
- Manage educational centers.
- Create and manage subjects.
- Manage teachers and their assigned subjects.
- Manage students associated with educational centers.
- Manage classrooms and rooms.

### 📚 Learning Management
- Manage lessons and educational materials.
- Schedule lessons.
- Manage student lesson bookings.
- Track student enrollments.
- Support educational content organization.

### 💳 Payment Integration
- Integrated with Paymob for online payments.
- Support for card and mobile wallet payments.
- Integrated with Fawaterak payment services.
- Generate payment tokens and payment invoices.
- Handle payment callbacks and webhooks.
- Track payment and enrollment statuses.
- Verify payment callback signatures using HMAC.

### ⚙️ Backend Capabilities
- RESTful API architecture.
- Generic Repository Pattern.
- Specification Pattern for query organization.
- Dependency Injection.
- AutoMapper for object mapping.
- Entity Framework Core for data access.
- SQL Server database integration.
- Swagger / OpenAPI documentation.

---

## 🛠️ Technologies & Tools

| Technology | Purpose |
|---|---|
| C# | Backend Programming Language |
| .NET 8 | Backend Framework |
| ASP.NET Core Web API | RESTful API Development |
| Entity Framework Core | ORM |
| SQL Server | Relational Database |
| ASP.NET Core Identity | User Management |
| JWT | Authentication |
| AutoMapper | Object Mapping |
| Swagger | API Documentation |
| Paymob | Online Payment Integration |
| Fawaterak | Payment Gateway Integration |
| Repository Pattern | Data Access Abstraction |
| Specification Pattern | Query Organization |

---

## 🏗️ Project Architecture

The solution is organized into four main projects to separate responsibilities and improve maintainability.

```text
EducationalPlatform/
│
├── Api.PL/
│   ├── Controllers/
│   ├── DTO/
│   ├── Helper/
│   └── Program.cs
│
├── Core/
│   ├── Entities/
│   ├── Repository/
│   ├── ServiceContract/
│   └── Specifications/
│
├── Repository/
│   ├── Data/
│   │   └── Configurations/
│   ├── Repository/
│   └── EducationalPlatformDbContext.cs
│
├── Service/
│   ├── ServiceContract/
│   ├── PaymentService.cs
│   ├── OtpService.cs
│   └── TokenServices.cs
│
└── EducationalPlatform.sln
```

### Architecture Responsibilities

- **Api.PL:** Handles HTTP requests, controllers, DTOs, authentication configuration, and API responses.
- **Core:** Contains domain entities, repository abstractions, service contracts, and specifications.
- **Repository:** Handles database operations, Entity Framework Core configurations, and repository implementations.
- **Service:** Contains business logic, authentication services, OTP operations, and external payment integrations.

---

## 👥 User Roles

| Role | Responsibilities |
|---|---|
| Admin | Administrative platform operations |
| Center | Manage educational center resources |
| Student | Access educational services and enroll in subjects |

---

## 💳 Payment Workflow

The platform supports online payments through Paymob and Fawaterak.

### Paymob Workflow

1. Student selects a subject.
2. The API creates an enrollment with a pending payment status.
3. The backend requests an authentication token from Paymob.
4. A payment key is generated.
5. The API returns a payment redirect URL.
6. Paymob processes the transaction.
7. The backend receives a payment callback.
8. The payment and enrollment statuses are updated.

### Fawaterak Integration

- Retrieve available payment methods.
- Create electronic invoices.
- Process payment requests.
- Handle payment notifications and callbacks.

---

## ⚙️ Getting Started

### Prerequisites

Make sure you have installed:

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or later
- Git

### 1. Clone the Repository

```bash
git clone https://github.com/MohamedBadawy23/EducationalPlatform.git
```

### 2. Navigate to the Project

```bash
cd EducationalPlatform
```

### 3. Configure the Database

Update the connection string in your development configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=EducationalPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 4. Configure Application Secrets

Configure the required credentials for:

- JWT authentication.
- Paymob API credentials.
- Fawaterak API credentials.
- OTP and email services.

**Important:** Never commit API keys, passwords, or sensitive credentials to source control.

### 5. Apply Database Migrations

```bash
dotnet ef database update --project Repository --startup-project Api.PL
```

### 6. Run the Application

```bash
dotnet run --project Api.PL
```

### 7. Explore API Documentation

Once the application is running in Development mode, open:

```text
https://localhost:<port>/swagger
```

---

## 🔒 Security Considerations

The project incorporates:

- JWT token validation.
- Role-based authorization.
- ASP.NET Core Identity.
- OTP verification.
- HMAC-based payment callback verification.

Production deployments should additionally use secure secret management, HTTPS, proper payment webhook validation, and rate limiting.

---

## 🔮 Future Improvements

Potential improvements for future releases:

- Unit and integration testing.
- Docker containerization.
- CI/CD pipeline integration.
- Redis caching.
- Structured logging and monitoring.
- Improved global exception handling.
- API versioning.
- Advanced reporting and analytics.

---

## 👨‍💻 Developer

**Mohamed Badawy**

Backend .NET Developer

- GitHub: [MohamedBadawy23](https://github.com/MohamedBadawy23)
- Project Repository: [EducationalPlatform](https://github.com/MohamedBadawy23/EducationalPlatform)

---

## 📄 License

This project is available for educational and portfolio purposes. Add a repository license file if you intend to publish formal usage and distribution terms.

---

**Built with C# and ASP.NET Core 8.**

