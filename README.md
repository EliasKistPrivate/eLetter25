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
- SQL Server
- Keycloak (JWT-Authentifizierung)
- .NET Aspire (lokale Entwicklung)
- MediatR (CQRS-Pattern)

## 🚀 Schnellstart

```powershell
# Aspire Host starten (startet SQL Server + Keycloak)
dotnet run --project eLetter25.Host

# Datenbank migrieren
dotnet ef database update --project eLetter25.Infrastructure --startup-project eLetter25.API
```

Die API läuft unter `http://localhost:5000`  
Das Aspire Dashboard unter `http://localhost:15000`

## 📖 Dokumentation

Detaillierte Informationen zur Architektur und Entwicklung:

- [Architektur-Dokumentation](docs/Architektur.md)
- [Coding-Guidelines](.github/copilot-instructions.md)

