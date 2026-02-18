#  Library Management API

A production-ready .NET 9 Web API built with **Clean Architecture**, **CQRS**, and **Domain-Driven Design** principles. This project is a complete redesign and evolution of my original [BookCollectionAPI](https://github.com/Richmondbh/BookCollectionlAPI), incorporating enterprise patterns and best practices learned from the **Azure AZ-204** certification.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=flat&logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-Cache-DC382D?style=flat&logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?style=flat&logo=docker&logoColor=white)


---

##  Project Background

After passing the **Azure AZ-204 Developer Associate** exam, I wanted to go beyond practice projects and apply the concepts in a real system. This led me to completely redesign my existing BookCollection API, refactoring it into a **Clean Architecture / DDD-oriented modular monolith** with scalability in mind.

### Evolution from BookCollectionAPI

| Aspect | BookCollectionAPI (Before) | LibraryManagement (After) |
|--------|---------------------------|---------------------------|
| Architecture | Traditional N-Tier | Clean Architecture + DDD |
| Patterns | Basic CRUD | CQRS with MediatR |
| Validation | Manual | FluentValidation + Pipeline Behaviors |
| Caching | None | Redis with Cache Behavior |
| Messaging | None | Azure Service Bus + Events |
| Authentication | Basic JWT/Role-based | JWT with Role-Based Authorization + Policies |
| Testing | Minimal | Unit + Integration Tests (65+ tests) |
| Containerization | None | Docker + Docker Compose |

---

##  Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              API Layer                                   │
│                    (Controllers, Middleware, Auth)                       │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                          Application Layer                               │
│              (Commands, Queries, Handlers, Validators,                   │
│               Pipeline Behaviors, Interfaces)                            │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                            Domain Layer                                  │
│                  (Entities, Value Objects, Constants)                    │
└─────────────────────────────────────────────────────────────────────────┘
                                    ▲
                                    │
┌─────────────────────────────────────────────────────────────────────────┐
│                         Infrastructure Layer                             │
│           (EF Core, Repositories, Caching, Messaging,                    │
│            Blob Storage, Identity Services)                              │
└─────────────────────────────────────────────────────────────────────────┘
```

### Project Structure

```
LibraryManagement/
├── src/
│   ├── LibraryManagement.Domain/           # Entities, Constants, Value Objects
│   ├── LibraryManagement.Application/      # CQRS Handlers, Validators, Behaviors
│   ├── LibraryManagement.Infrastructure/   # Data Access, External Services
│   ├── LibraryManagement.API/              # Controllers, Middleware
│   └── LibraryManagement.Functions/        # Azure Functions (Service Bus Triggers)
│
├── tests/
│   ├── LibraryManagement.UnitTests/        # 48 Unit Tests
│   └── LibraryManagement.IntegrationTests/ # 17 Integration Tests
│
├── docker-compose.yml
├── Dockerfile
└── README.md
```

---

##  Features

### Core Features
-  **CRUD Operations** for Books with full validation
-  **JWT Authentication** with secure token generation
-  **Role-Based Authorization** (Admin / User roles)
-  **Book Cover Upload** to Azure Blob Storage
-  **Caching** with Redis (falls back to in-memory)

### Architecture & Patterns
-  **Clean Architecture** with clear layer separation
-  **CQRS Pattern** using MediatR
-  **Pipeline Behaviors** for cross-cutting concerns:
  - `LoggingBehavior` - Request/Response logging
  - `ValidationBehavior` - Automatic FluentValidation
  - `CachingBehavior` - Transparent caching for queries
-  **Repository Pattern** for data access abstraction
-  **Domain Events** for decoupled workflows

### Azure Integration
-  **Azure Service Bus** for event-driven messaging
-  **Azure Functions** triggered by Service Bus queues
-  **Azure Blob Storage** for file uploads
-  **Application Insights** for telemetry and monitoring

### Testing
-  **Unit Tests** (48 tests) - Handlers, Validators, Domain Entities
-  **Integration Tests** (17 tests) - Full API endpoint testing
-  **Test Fixtures** for reusable test data

### DevOps Ready
-  **Docker** containerization
-  **Docker Compose** for local development (API, PostgreSQL, Redis)
-  **Environment-based configuration**

---

##  Technology Stack

| Category | Technology |
|----------|------------|
| **Framework** | .NET 9, ASP.NET Core |
| **Database** | PostgreSQL with EF Core 9 |
| **Caching** | Redis / In-Memory fallback |
| **Messaging** | Azure Service Bus |
| **Storage** | Azure Blob Storage |
| **Authentication** | JWT Bearer Tokens |
| **Validation** | FluentValidation |
| **Mediator** | MediatR 14 |
| **Testing** | xUnit, FluentAssertions, Moq |
| **Containerization** | Docker, Docker Compose |
| **Monitoring** | Application Insights |

---

##  Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, for containerized setup)
- [PostgreSQL](https://www.postgresql.org/download/) (if not using Docker)

### Option 1: Run with Docker Compose (Recommended)

```bash
# Clone the repository
git clone https://github.com/Richmondbh/LibraryManagement.git
cd LibraryManagement

# Start all services (API, PostgreSQL, Redis)
docker-compose up -d

# API will be available at http://localhost:8080/swagger
```

### Option 2: Run Locally

```bash
# Clone the repository
git clone https://github.com/Richmondbh/LibraryManagement.git
cd LibraryManagement

# Set up user secrets for the API project
cd src/LibraryManagement.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=LibraryManagementDb;Username=postgres;Password=yourpassword"
dotnet user-secrets set "JwtSettings:Secret" "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"
dotnet user-secrets set "JwtSettings:Issuer" "LibraryManagement"
dotnet user-secrets set "JwtSettings:Audience" "LibraryManagement"

# Apply database migrations
cd ../..
dotnet ef database update --project src/LibraryManagement.Infrastructure --startup-project src/LibraryManagement.API

# Run the API
dotnet run --project src/LibraryManagement.API

# API will be available at https://localhost:7182/swagger
```

---

##  API Endpoints

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | X |
| POST | `/api/auth/login` | Login & get JWT token | X |
| GET | `/api/auth/me` | Get current user info |  User |
| POST | `/api/auth/register-admin` | Register new admin |  Admin |

### Books

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/books` | Get all books | X |
| GET | `/api/books/{id}` | Get book by ID | X |
| POST | `/api/books` | Create new book |  Admin |
| PUT | `/api/books/{id}` | Update book |  Admin |
| DELETE | `/api/books/{id}` | Delete book |  Admin |
| POST | `/api/books/{id}/cover` | Upload book cover |  User |

### Default Admin Credentials

```
Email: admin@library.com
Password: Admin123!
```

---

##  Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/LibraryManagement.UnitTests

# Run integration tests only
dotnet test tests/LibraryManagement.IntegrationTests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Summary

| Test Type | Count | Coverage |
|-----------|-------|----------|
| Unit Tests | 48 | Handlers, Validators, Domain Entities |
| Integration Tests | 17 | Full API Endpoints |
| **Total** | **65** | |

---

##  Key Implementation Details

### CQRS with MediatR

Commands and Queries are separated for clear intent:

```csharp
// Command - Changes state
public record CreateBookCommand(
    string Title,
    string Author,
    string ISBN,
    int PublishedYear
) : IRequest<Guid>;

// Query - Reads state (cacheable)
public record GetAllBooksQuery : IRequest<IEnumerable<BookResponse>>, ICacheable
{
    public string CacheKey => "books:all";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
```

### Pipeline Behaviors

Cross-cutting concerns handled transparently:

```csharp
// Request flows through behaviors in order:
Request → LoggingBehavior → ValidationBehavior → CachingBehavior → Handler
```

### Role-Based Authorization

```csharp
// Admin only
[Authorize(Policy = "RequireAdminRole")]
[HttpPost]
public async Task<ActionResult<Guid>> Create(CreateBookCommand command) { }

// Any authenticated user
[Authorize]
[HttpPost("{id:guid}/cover")]
public async Task<IActionResult> UploadCover(Guid id, IFormFile file) { }
```

---

##  Configuration

### appsettings.json Structure

```json
{
  "JwtSettings": {
    "Secret": "",
    "Issuer": "LibraryManagement",
    "Audience": "LibraryManagement",
    "ExpirationInMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "",
    "Redis": "",
    "ServiceBus": "",
    "BlobStorage": ""
  },
  "BlobStorage": {
    "ContainerName": "book-covers"
  }
}
```

### Environment Variables (Docker)

```yaml
- ConnectionStrings__DefaultConnection=Host=postgres;Database=LibraryDb;...
- ConnectionStrings__Redis=redis:6379
- JwtSettings__Secret=YourSecretKey
```

---

##  Roadmap

- [x] Clean Architecture setup
- [x] CQRS with MediatR
- [x] FluentValidation + Pipeline Behaviors
- [x] Caching with Redis
- [x] Azure Service Bus integration
- [x] Azure Functions (Service Bus triggers)
- [x] Azure Blob Storage for file uploads
- [x] Application Insights
- [x] JWT Authentication
- [x] Role-Based Authorization
- [x] Unit Tests
- [x] Integration Tests
- [x] Docker containerization
      
### To Be Completed----------------------------------
- [ ] SendGrid email notifications
- [ ] Azure deployment (Container Apps)
- [ ] CI/CD with GitHub Actions
- [ ] API versioning
- [ ] Rate limiting

---
##  Learning Resources

This project applies concepts from:

### Azure & Cloud
- [Azure AZ-204 Developer Associate Certification](https://learn.microsoft.com/en-us/credentials/certifications/azure-developer/?practice-assessment-type=certification) - Functions, Messaging, Storage, and more

### Domain-Driven Design (DDD)
- [Microsoft - DDD-Oriented Microservice](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)
- [Domain-Driven Design: From Customer Ideas to Code](https://medium.com/@codebob75/domain-driven-design-ddd-from-customer-ideas-to-code-a83a005326e9)
- [YouTube - DDD Explained](https://www.youtube.com/watch?v=4rhzdZIDX_k)
- [YouTube - Domain-Driven Design](https://www.youtube.com/watch?v=B5oQ0lMjkrI)

### Validation
- [FluentValidation Documentation](https://docs.fluentvalidation.net/en/latest/start.html)
- [FluentValidation - AbstractValidator Source](https://github.com/FluentValidation/FluentValidation/blob/main/src/FluentValidation/AbstractValidator.cs#L210)
- [Microsoft - ValidationException](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationexception.-ctor?view=net-10.0#system-componentmodel-dataannotations-validationexception-ctor)

### Containerization
- [Docker Official](https://www.docker.com/)
- [Docker - Writing a Dockerfile](https://docs.docker.com/get-started/docker-concepts/building-images/writing-a-dockerfile/)

### Clean Architecture & CQRS
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) by Robert C. Martin
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [CQRS Pattern - Microsoft](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)

### Testing
- [xUnit Documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [FluentAssertions](https://fluentassertions.com/introduction)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)

---

##  Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



##  Author

**Richmond Boakye**

- GitHub: [@Richmondbh](https://github.com/Richmondbh)
- Email: richmondboakye0017@gmail.com

---

##  Acknowledgments

- This project builds upon my original [BookCollectionAPI](https://github.com/Richmondbh/BookCollectionlAPI)
- Thanks to the .NET community for excellent documentation and tutorials
- Inspired by real-world enterprise application patterns

---

 **If you find this project helpful, please give it a star!** 
