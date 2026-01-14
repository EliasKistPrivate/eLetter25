# eLetter25

Eine ASP.NET Core-Anwendung zur Verwaltung von Briefen und deren Metadaten mit JWT-basierter Authentifizierung.

## 🏗️ Architektur

Das Projekt folgt **Clean Architecture** und **Domain-Driven Design (DDD)** mit vier Schichten:

- **Domain** – Geschäftslogik, Entities, Value Objects
- **Application** – Use Cases, Commands/Handlers (MediatR), Ports (Interfaces)
- **Infrastructure** – Persistenz (EF Core, SQL Server), Services (JWT, Identity)
- **API** – REST Controllers (ASP.NET Core MVC)

### Architektur-Pattern

- **CQRS**: Commands/Queries via MediatR
- **Ports & Adapters**: Application definiert Interfaces, Infrastructure implementiert
- **Repository Pattern**: Datenzugriff abstrahiert
- **Unit of Work**: Transaktionsgrenzen explizit

## 🛠️ Technologien

- .NET 10.0
- Entity Framework Core 10
- SQL Server (Briefe-Datenbank)
- PostgreSQL (Identity-Datenbank)
- ASP.NET Core Identity (JWT-Authentifizierung)
- .NET Aspire (lokale Entwicklung)
- MediatR (CQRS-Pattern)

## 📦 Projekt-Struktur

```
eLetter25/
├── eLetter25.Domain/              # Entities, Value Objects, Business Rules
├── eLetter25.Application/         # Use Cases, Commands, Handlers, Ports
│   ├── Auth/                      # Authentication Use Cases
│   │   ├── Contracts/             # Request DTOs
│   │   ├── Ports/                 # Interfaces (IJwtTokenGenerator, etc.)
│   │   └── UseCases/              # RegisterUser, LoginUser
│   └── Letters/                   # Letter Management Use Cases
├── eLetter25.Infrastructure/      # EF Core, SQL Server, Services
│   ├── Auth/                      # Authentication Services & Data
│   │   ├── Data/                  # ApplicationUser, DbContext
│   │   └── Services/              # JwtTokenGenerator, UserRegistrationService
│   └── Persistence/               # Repositories, Mappings
├── eLetter25.API/                 # REST API (Controllers)
│   └── Auth/Controllers/          # RegisterController, LoginController
├── eLetter25.Host/                # .NET Aspire Orchestration
└── eLetter25.Client/              # Angular Frontend
```

## 🚀 Schnellstart

### Voraussetzungen

- **.NET 10.0 SDK** installiert
- **Docker Desktop** installiert und **gestartet** (wird für SQL Server und PostgreSQL benötigt)

### 1. User Secrets konfigurieren

Der JWT SecretKey muss in den User Secrets des API-Projekts gespeichert werden:

```powershell
# JWT Secret Key setzen (mindestens 32 Zeichen für HS256)
dotnet user-secrets set "Jwt:SecretKey" "your-super-secret-key-min-32-chars-long-for-hs256-algorithm" --project eLetter25.API
```

Die JWT Expiration Time wird in der `appsettings.json` des API-Projekts konfiguriert.

### 2. Anwendung starten

```powershell
# Aspire Host starten (startet SQL Server + PostgreSQL + Angular Client)
dotnet run --project eLetter25.Host
```

**Das war's!** Die Datenbank-Migrationen werden automatisch beim Start der API ausgeführt.

- **API:** `https://localhost:7xxx` (Port wird im Terminal angezeigt)
- **Aspire Dashboard:** `http://localhost:15000`
- **Angular Client:** `http://localhost:4200`

## 📡 API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Benutzerregistrierung
- `POST /api/auth/login` - Login (liefert JWT-Token)

### Letters (`/letters`)
- `POST /letters` - Brief erstellen (erfordert Authentifizierung)

Vollständige API-Dokumentation: `https://localhost:7xxx/scalar/v1` (Scalar UI)

## 📖 Dokumentation

Detaillierte Informationen zur Architektur und Entwicklung:

- [Architektur-Dokumentation](docs/Architektur.md)
- [Coding-Guidelines](.github/copilot-instructions.md)

