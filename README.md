# eLetter25

Eine ASP.NET Core-Anwendung zur Verwaltung von Briefen und deren Metadaten.

## 🏗️ Architektur

Das Projekt folgt **Clean Architecture** und **Domain-Driven Design (DDD)** mit vier Schichten:

- **Domain** – Geschäftslogik, Entities, Value Objects
- **Application** – Use Cases, Commands/Handlers (MediatR)
- **Infrastructure** – Persistenz (EF Core, SQL Server)
- **API** – REST Endpoints (Minimal APIs)

## 🛠️ Technologien

- .NET 10.0
- Entity Framework Core 10
- SQL Server (Briefe-Datenbank)
- PostgreSQL (Identity-Datenbank)
- ASP.NET Core Identity (JWT-Authentifizierung)
- .NET Aspire (lokale Entwicklung)
- MediatR (CQRS-Pattern)

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

## 📖 Dokumentation

Detaillierte Informationen zur Architektur und Entwicklung:

- [Architektur-Dokumentation](docs/Architektur.md)
- [Coding-Guidelines](.github/copilot-instructions.md)

