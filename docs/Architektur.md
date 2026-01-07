# eLetter25 – Softwarearchitektur

**Version:** 1.0  
**Stand:** 2026-01-07  
**Zielgruppe:** Erfahrene Softwareentwickler (Onboarding)

---

## 1. Überblick

**eLetter25** ist eine ASP.NET Core-Anwendung zur Verwaltung von Briefen (Letters) und deren Metadaten. Die Architektur folgt den Prinzipien von **Clean Architecture** und **Domain-Driven Design (DDD)**, mit expliziter Trennung zwischen Geschäftslogik, Anwendungslogik und Infrastruktur.

**Kernmerkmale:**
- .NET 10.0
- Clean Architecture mit 4 Schichten (Domain, Application, Infrastructure, API)
- Command-Handler-Pattern via MediatR
- Entity Framework Core 10 + SQL Server
- JWT-basierte Authentifizierung (Keycloak)
- .NET Aspire für lokale Entwicklung/Orchestrierung

---

## 2. Architekturprinzipien

### 2.1 Clean Architecture (Ports & Adapters)

Die Dependency Rule wird strikt eingehalten: **Abhängigkeiten zeigen nur nach innen**.

```
┌─────────────────────────────────────┐
│        API (Presentation)           │  ← HTTP, Controllers, Minimal APIs
└──────────────┬──────────────────────┘
               │ depends on
┌──────────────▼──────────────────────┐
│       Infrastructure                │  ← EF Core, SQL Server, Mapper
└──────────────┬──────────────────────┘
               │ depends on
┌──────────────▼──────────────────────┐
│        Application                  │  ← Use Cases, Ports, DTOs
└──────────────┬──────────────────────┘
               │ depends on
┌──────────────▼──────────────────────┐
│          Domain                     │  ← Entities, Value Objects, Business Rules
└─────────────────────────────────────┘
```

**Warum Clean Architecture?**
- **Testbarkeit:** Domain-Logik kann ohne Infrastruktur getestet werden
- **Austauschbarkeit:** Persistenz (EF Core) kann gegen In-Memory-Implementierung getauscht werden
- **Wartbarkeit:** Änderungen in der Infrastruktur betreffen die Domain nicht
- **Verständlichkeit:** Klare Verantwortlichkeiten pro Schicht

### 2.2 Domain-Driven Design (DDD)

**Konzepte im Einsatz:**
- **Entities:** `Letter`, `LetterDocument` (mit eindeutiger ID, `DomainEntity`-Basisklasse)
- **Value Objects:** `Correspondent`, `Tag`, `Address`, `Email`, `PhoneNumber`
- **Aggregates:** `Letter` ist Aggregate Root (verwaltet Tags)
- **Factories:** Statische Factory-Methode `Letter.Create(...)`
- **Domain Exceptions:** Geschäftsregel-Verletzungen werfen Domain-Exceptions

**Wichtig:** Value Objects sind teilweise **mutable** (z.B. `Correspondent` hat `set;`). Das ist eine bewusste Abweichung von striktem DDD, vermutlich für EF Core-Kompatibilität.

### 2.3 CQRS-ähnliches Pattern (via MediatR)

Keine strikte Read/Write-Trennung, aber:
- **Commands:** `CreateLetterCommand` → Schreiboperation
- **Queries:** (noch nicht implementiert, aber Struktur vorbereitet)
- **Handler:** Je ein Handler pro Command/Query (`CreateLetterHandler`)

**Warum MediatR?**
- Entkopplung zwischen API und Application-Layer
- Einheitliche Pipeline für Cross-Cutting Concerns (z.B. Validierung, Logging – aktuell nicht implementiert)
- Erweiterbarkeit durch Behaviors

---

## 3. Projekt-Struktur

### 3.1 Projekte und Abhängigkeiten

| Projekt                  | Typ        | Abhängigkeiten                          | Verantwortung                              |
|--------------------------|------------|----------------------------------------|--------------------------------------------|
| `eLetter25.Domain`       | Class Lib  | —                                      | Geschäftslogik, Entities, Value Objects    |
| `eLetter25.Application`  | Class Lib  | `Domain`, `MediatR`                    | Use Cases, Ports, DTOs                     |
| `eLetter25.Infrastructure` | Class Lib | `Application`, `Domain`, `EF Core`     | Persistenz, Mapper, Repository             |
| `eLetter25.API`          | Web API    | `Infrastructure`                       | HTTP-Endpunkte, DI-Konfiguration, Auth     |
| `eLetter25.Host`         | Aspire Host | `API`, `Infrastructure` (⚠️)          | Lokale Orchestrierung (Keycloak, SQL)      |

**⚠️ Warnung:** Der Aspire Host sollte nur `API` referenzieren. Die Referenz auf `Infrastructure` erzeugt Build-Warnung `ASPIRE004`.

### 3.2 Ordnerstruktur (Beispiel)

```
eLetter25.Domain/
  ├── Common/
  │   ├── DomainEntity.cs          # Basis-Klasse für Entities
  │   └── ExceptionBase.cs         # Basis-Klasse für Domain-Exceptions
  ├── Letters/
  │   ├── Letter.cs                # Aggregate Root
  │   ├── LetterDocument.cs        # Entity
  │   ├── Enums/                   # Enumerationen
  │   ├── Exceptions/              # Domain-spezifische Exceptions
  │   └── ValueObjects/
  │       ├── Tag.cs               # readonly record struct
  │       └── Correspondent.cs     # sealed record
  └── Shared/
      └── ValueObjects/
          ├── Address.cs
          ├── Email.cs
          └── PhoneNumber.cs

eLetter25.Application/
  ├── Common/
  │   └── Ports/
  │       └── IUnitOfWork.cs
  ├── Letters/
  │   ├── Contracts/
  │   │   └── CreateLetterRequest.cs
  │   ├── Ports/
  │   │   └── ILetterRepository.cs
  │   └── UseCases/
  │       └── CreateLetter/
  │           ├── CreateLetterCommand.cs
  │           ├── CreateLetterHandler.cs
  │           └── CreateLetterResult.cs
  └── Shared/
      └── DTOs/
          ├── AddressDto.cs
          └── CorrespondentDto.cs

eLetter25.Infrastructure/
  ├── Persistence/
  │   ├── AppDbContext.cs
  │   ├── EfUnitOfWork.cs
  │   └── Letters/
  │       ├── EfLetterRepository.cs
  │       ├── LetterDbEntity.cs
  │       ├── LetterTagDbEntity.cs
  │       ├── Mappings/
  │       │   ├── ILetterDomainToDbMapper.cs
  │       │   ├── ILetterDbToDomainMapper.cs
  │       │   ├── LetterDomainToDbMapper.cs
  │       │   └── LetterDbToDomainMapper.cs
  │       └── Configurations/
  │           ├── LetterConfiguration.cs
  │           └── LetterTagConfiguration.cs
  └── Migrations/
      └── YYYYMMDDHHMMSS_Initial.cs
```

### 3.3 Namenskonventionen

| Konzept           | Namensschema                         | Beispiel                          |
|-------------------|--------------------------------------|-----------------------------------|
| Entity            | `{Name}`                             | `Letter`, `LetterDocument`        |
| Value Object      | `{Name}` (record/record struct)      | `Tag`, `Correspondent`, `Email`   |
| Command           | `{Action}{Entity}Command`            | `CreateLetterCommand`             |
| Handler           | `{Action}{Entity}Handler`            | `CreateLetterHandler`             |
| Result            | `{Action}{Entity}Result`             | `CreateLetterResult`              |
| Port (Interface)  | `I{Capability}` oder `I{Name}Port`   | `ILetterRepository`, `IUnitOfWork` |
| DB Entity         | `{Name}DbEntity`                     | `LetterDbEntity`, `LetterTagDbEntity` |
| DTO               | `{Name}Dto` oder `{Action}{Name}Request` | `CorrespondentDto`, `CreateLetterRequest` |
| Mapper            | `I{Source}To{Target}Mapper`          | `ILetterDomainToDbMapper`         |

---

## 4. Schicht-Details

### 4.1 Domain (eLetter25.Domain)

**Verantwortung:**
- Geschäftslogik und Invarianten
- Keine Abhängigkeiten auf andere Projekte oder Frameworks (außer .NET BCL)

**Konzepte:**

#### 4.1.1 Entities
```csharp
public abstract class DomainEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}

public class Letter : DomainEntity
{
    public string Subject { get; private set; }
    public IReadOnlyCollection<Tag> Tags { get; private set; }
    public DateTimeOffset SentDate { get; private set; }
    public Correspondent? Sender { get; private set; }
    public Correspondent? Recipient { get; private set; }
    
    // Factory-Methode
    public static Letter Create(Correspondent sender, Correspondent recipient, DateTimeOffset sentDate)
    {
        // Validierung + Instanziierung
    }
    
    // Mutationen als Fluent API
    public Letter SetSubject(string subject) { /* ... */ return this; }
    public Letter AddTag(Tag tag) { /* ... */ return this; }
}
```

**Wichtig:**
- Private Setter → Änderungen nur über Methoden
- Factory-Methode erzwingt gültige Initialisierung
- Fluent API (`return this;`) ermöglicht Method Chaining
- Parameterloser privater Konstruktor für EF Core

#### 4.1.2 Value Objects
```csharp
// Immutable Value Object (record struct)
public readonly record struct Tag
{
    public string Value { get; }
    
    public Tag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(...);
        Value = value;
    }
}

// Mutable Value Object (für EF Core)
public sealed record Correspondent
{
    public string Name { get; set; }
    public Address Address { get; set; }
    public Email? Email { get; set; }
    public PhoneNumber? Phone { get; set; }
}
```

**Achtung:** `Correspondent` ist **nicht immutable** (`set;` statt `init;`). Das ist eine pragmatische Entscheidung für EF Core-Mapping, widerspricht aber DDD-Best-Practices.

#### 4.1.3 Domain Exceptions
```csharp
public abstract class ExceptionBase : Exception
{
    // Basis-Klasse für alle Domain-Exceptions
}
```

Beispiele: `InvalidLetterStateException`, `DuplicateTagException` (noch nicht implementiert).

---

### 4.2 Application (eLetter25.Application)

**Verantwortung:**
- Orchestrierung von Use Cases
- Definition von Ports (Schnittstellen für Infrastructure)
- DTOs für Input/Output

**Abhängigkeiten:**
- `eLetter25.Domain`
- `MediatR` (14.0.0)

#### 4.2.1 Use Case Pattern

**Struktur:**
1. **Request/Command:** Input-DTO
2. **Handler:** Geschäftslogik-Orchestrierung
3. **Result:** Output-DTO

**Beispiel: CreateLetter**
```csharp
// 1. Command
public sealed record CreateLetterCommand(CreateLetterRequest Request) 
    : IRequest<CreateLetterResult>;

// 2. Handler
public sealed class CreateLetterHandler(
    ILetterRepository letterRepository, 
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateLetterCommand, CreateLetterResult>
{
    public async Task<CreateLetterResult> Handle(
        CreateLetterCommand command, 
        CancellationToken cancellationToken)
    {
        // 1. DTO → Domain Mapping
        var sender = MapToDomain(command.Request.Sender);
        var recipient = MapToDomain(command.Request.Recipient);
        
        // 2. Domain-Logik
        var letter = Letter.Create(sender, recipient, command.Request.SentDate)
            .SetSubject(command.Request.Subject);
        
        letter = command.Request.Tags
            .Select(tagName => new Tag(tagName))
            .Aggregate(letter, (current, tag) => current.AddTag(tag));
        
        // 3. Persistenz
        await letterRepository.AddAsync(letter, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return new CreateLetterResult(letter.Id);
    }
    
    private static Correspondent MapToDomain(CorrespondentDto dto) { /* ... */ }
}

// 3. Result
public sealed record CreateLetterResult(Guid LetterId);
```

**Wichtig:**
- Handler sind `sealed` (keine Vererbung)
- Primary Constructor für DI (`ILetterRepository`, `IUnitOfWork`)
- Mapping-Logik im Handler (alternative: separater Mapper)

#### 4.2.2 Ports (Interfaces)

**Repository:**
```csharp
public interface ILetterRepository
{
    Task AddAsync(Letter letter, CancellationToken cancellationToken = default);
    Task<Letter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
```

**Unit of Work:**
```csharp
public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
```

**Warum Ports?**
- Dependency Inversion: Application definiert Interfaces, Infrastructure implementiert sie
- Testbarkeit: Mocking ohne Infrastructure-Abhängigkeit

---

### 4.3 Infrastructure (eLetter25.Infrastructure)

**Verantwortung:**
- Implementierung der Application-Ports
- Persistenz (EF Core, SQL Server)
- Mapping zwischen Domain und DB-Modell

**Abhängigkeiten:**
- `eLetter25.Application`
- `eLetter25.Domain`
- `Microsoft.EntityFrameworkCore.SqlServer` (10.0.1)

#### 4.3.1 DbContext

```csharp
public sealed class AppDbContext : DbContext
{
    public DbSet<LetterDbEntity> Letters { get; set; }
    public DbSet<LetterTagDbEntity> LetterTags { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API Konfigurationen via IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

**Wichtig:**
- Konfigurationen **nicht** im DbContext, sondern in separaten `IEntityTypeConfiguration<T>`-Klassen
- Pattern: `LetterConfiguration.cs`, `LetterTagConfiguration.cs`

#### 4.3.2 Repository-Implementierung

```csharp
public sealed class EfLetterRepository(
    AppDbContext dbContext,
    ILetterDomainToDbMapper domainToDbMapper,
    ILetterDbToDomainMapper dbToDomainMapper) 
    : ILetterRepository
{
    public async Task AddAsync(Letter letter, CancellationToken ct = default)
    {
        var entity = domainToDbMapper.MapToDbEntity(letter);
        await dbContext.Letters.AddAsync(entity, ct);
    }
    
    public async Task<Letter?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.Letters
            .Include(l => l.Tags)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, ct);
        
        return entity is null ? null : dbToDomainMapper.MapToDomain(entity);
    }
}
```

**Wichtig:**
- Explizite Mapper statt AutoMapper (mehr Kontrolle, weniger "Magie")
- `AsNoTracking()` für Read-Operationen
- `Include()` für Eager Loading von Related Data

#### 4.3.3 Persistence Model (DB Entities)

**Denormalisierung:**
Das Persistence-Modell ist **nicht 1:1 zum Domain-Modell**. `Correspondent` wird denormalisiert in Spalten:

```csharp
public sealed class LetterDbEntity
{
    public Guid Id { get; set; }
    public string Subject { get; set; }
    public DateTimeOffset SentDate { get; set; }
    
    // Denormalisiert: Sender
    public string SenderName { get; set; }
    public string SenderStreet { get; set; }
    public string SenderPostalCode { get; set; }
    public string SenderCity { get; set; }
    public string SenderCountry { get; set; }
    public string? SenderEmail { get; set; }
    public string? SenderPhone { get; set; }
    
    // Denormalisiert: Recipient
    public string RecipientName { get; set; }
    // ... analog
    
    // Relation: Tags
    public List<LetterTagDbEntity> Tags { get; set; } = [];
}
```

**Warum Denormalisierung?**
- Performance (keine Joins für Correspondent-Daten)
- Simplicität (keine separaten Correspondence-Tabellen)

**Achtung:** Bei häufig wiederholten Korrespondenten ist das ineffizient. Normalisierung wäre dann besser.

#### 4.3.4 Unit of Work

```csharp
public sealed class EfUnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public Task CommitAsync(CancellationToken ct = default)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}
```

**Warum UnitOfWork?**
- Explizite Transaktionsgrenze
- Entkopplung von EF Core (`SaveChangesAsync` ist Implementierungsdetail)

---

### 4.4 API (eLetter25.API)

**Verantwortung:**
- HTTP-Schnittstellen (Minimal APIs + Controllers)
- Dependency Injection-Konfiguration
- Authentifizierung/Autorisierung
- OpenAPI-Doku

**Abhängigkeiten:**
- `eLetter25.Infrastructure`

#### 4.4.1 DI-Konfiguration (Program.cs)

```csharp
// MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateLetterHandler).Assembly));

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("eletter25db");
    if (string.IsNullOrEmpty(cs))
        throw new InvalidOperationException("Connection string 'eletter25db' not found.");
    options.UseSqlServer(cs);
});

// Ports → Adapters
builder.Services.AddScoped<ILetterDomainToDbMapper, LetterDomainToDbMapper>();
builder.Services.AddScoped<ILetterDbToDomainMapper, LetterDbToDomainMapper>();
builder.Services.AddScoped<ILetterRepository, EfLetterRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Auth (Keycloak)
builder.Services.AddAuthentication().AddKeycloakJwtBearer(
    serviceName: "keycloak",
    realm: "eLetter25API",
    options =>
    {
        options.Audience = "eLetter25.API";
        if (builder.Environment.IsDevelopment())
            options.RequireHttpsMetadata = false;
    });
```

**Wichtig:**
- Registrierungen sind `Scoped` (nicht `Singleton`), da EF Core DbContext nicht thread-safe ist
- Connection String wird zur Laufzeit erwartet (Aspire injiziert ihn)

#### 4.4.2 Endpoints

**Minimal APIs:**
```csharp
app.MapGet("/", () => "eLetter25.API is running...");

app.MapGet("/health", () => Results.Ok("Healthy"))
    .RequireAuthorization(); // ⚠️ Für Ops-Tools evtl. problematisch

app.MapPost("/letters", async (
    CreateLetterRequest request,
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(new CreateLetterCommand(request), ct);
    return Results.Created($"/letters/{result.LetterId}", result);
});
```

**Wichtig:**
- `Results.Created()` statt `Results.Ok()` für POST (REST-Konvention)
- CancellationToken wird automatisch gebunden
- `/health` sollte **nicht** autorisiert sein (LoadBalancer/Kubernetes brauchen Zugriff)

---

### 4.5 Host (eLetter25.Host)

**Verantwortung:**
- Lokale Orchestrierung (Keycloak, SQL Server, API)
- Service Discovery
- Telemetrie/Logging (OTLP)

**Aspire Resources:**
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithOtlpExporter();

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var database = sqlServer.AddDatabase("eletter25db");

var letterAPI = builder.AddProject<Projects.eLetter25_API>("letterAPI")
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
```

**Wichtig:**
- `WaitFor()` erzwingt Start-Reihenfolge
- Connection String wird automatisch injiziert
- Keycloak-Konfiguration muss manuell erfolgen (Realm, Client-ID)

---

## 5. Daten- und Kontrollfluss

### 5.1 Typischer Request-Flow (CreateLetter)

```
┌──────────────────────────────────────────────────────────────────────────┐
│  1. HTTP POST /letters                                                   │
│     Body: CreateLetterRequest (JSON)                                     │
└────────────────────────┬─────────────────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────────────────────┐
│  2. API Layer (Minimal API)                                            │
│     - Model Binding: CreateLetterRequest request                       │
│     - Injected: IMediator mediator                                     │
│     - Send Command: mediator.Send(new CreateLetterCommand(request))    │
└────────────────────────┬───────────────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────────────────────┐
│  3. Application Layer (CreateLetterHandler)                            │
│     - Map DTO → Domain: CorrespondentDto → Correspondent               │
│     - Call Factory: Letter.Create(sender, recipient, sentDate)         │
│     - Apply Mutations: letter.SetSubject(...).AddTags(...)             │
│     - Validate Domain Rules (in Entity-Methoden)                       │
└────────────────────────┬───────────────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────────────────────┐
│  4. Application → Infrastructure (via Ports)                           │
│     - letterRepository.AddAsync(letter)                                │
│     - unitOfWork.CommitAsync()                                    │
└────────────────────────┬───────────────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────────────────────┐
│  5. Infrastructure Layer                                               │
│     - Map Domain → DB: ILetterDomainToDbMapper.MapToDbEntity(letter)   │
│     - EF Core: dbContext.Letters.AddAsync(entity)                      │
│     - Commit: dbContext.SaveChangesAsync()                             │
└────────────────────────┬───────────────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────────────────────┐
│  6. Database (SQL Server)                                              │
│     - INSERT INTO Letters (...)                                        │
│     - INSERT INTO LetterTags (...)                                     │
└────────────────────────┬───────────────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────────────────────┐
│  7. Response                                                           │
│     - CreateLetterResult(letter.Id)                                    │
│     - HTTP 201 Created, Location: /letters/{id}                        │
└────────────────────────────────────────────────────────────────────────┘
```

### 5.2 Wichtige Entscheidungspunkte

| Punkt | Entscheidung | Warum |
|-------|--------------|-------|
| **Mapping** | DTO → Domain im Handler | Keine Abhängigkeit auf AutoMapper, explizit |
| **Validierung** | In Domain-Methoden (`Letter.Create()`, `SetSubject()`) | Single Source of Truth, Business Rules in der Domain |
| **Transaktionsgrenze** | `unitOfWork.CommitAsync()` | Explizit, Handler entscheidet wann Commit |
| **ID-Generierung** | `Guid.NewGuid()` im Entity-Konstruktor | Verteilte Systeme, keine DB-Abhängigkeit |

---

## 6. Design Patterns

### 6.1 Implementierte Patterns

| Pattern | Einsatz | Beispiel |
|---------|---------|----------|
| **Repository** | Persistenz-Abstraktion | `ILetterRepository`, `EfLetterRepository` |
| **Unit of Work** | Transaktionsgrenze | `IUnitOfWork`, `EfUnitOfWork` |
| **Mediator** | Entkopplung API ↔ Application | MediatR (`CreateLetterCommand` → `CreateLetterHandler`) |
| **Factory Method** | Entity-Erzeugung | `Letter.Create(...)` |
| **Fluent Interface** | Entity-Mutationen | `letter.SetSubject(...).AddTag(...)` |
| **Mapper** | Domain ↔ DB Konvertierung | `ILetterDomainToDbMapper` |
| **Dependency Injection** | Alle Schichten | ASP.NET Core DI Container |
| **CQRS-Light** | Command/Query-Trennung (vorbereitet) | Commands via MediatR |

### 6.2 Nicht implementierte Patterns (Optional für Zukunft)

- **CQRS (vollständig):** Separate Read/Write-Models
- **Event Sourcing:** Domain Events statt State Persistence
- **Specification Pattern:** Komplexe Queries
- **Domain Events:** z.B. `LetterCreatedEvent`

---

## 7. Do's & Don'ts

### ✅ Do's

#### Domain Layer
- **Immer Factory-Methoden verwenden:** `Letter.Create(...)` statt `new Letter()`
- **Geschäftsregeln in Entity-Methoden:** Validierung in `SetSubject()`, `AddTag()`, etc.
- **Keine Infrastruktur-Abhängigkeiten:** Kein EF Core, kein SQL, kein HTTP
- **Value Objects für komplexe Typen:** `Email`, `Address` statt primitive Strings

#### Application Layer
- **Ein Handler pro Use Case:** Nicht mehrere Commands in einem Handler
- **Mapping im Handler:** DTO → Domain und Domain → Result
- **Ports definieren:** Interfaces für alle Infrastruktur-Abhängigkeiten
- **DTOs sind dumm:** Keine Logik, nur Daten

#### Infrastructure Layer
- **Explizite Mapper:** Keine "Magie" (AutoMapper), klare Mapping-Logik
- **Entity Configurations in separaten Klassen:** `IEntityTypeConfiguration<T>`
- **AsNoTracking() für Reads:** Performance-Optimierung
- **Include() für Related Data:** Eager Loading statt Lazy Loading

#### API Layer
- **HTTP-Statuscodes korrekt verwenden:** 201 Created, 404 NotFound, etc.
- **CancellationToken propagieren:** Alle async-Methoden
- **Dependency Injection nutzen:** Nicht `new`
- **Validation in Endpoint oder via FluentValidation:** Nicht in Handler

---

### ❌ Don'ts

#### Domain Layer
- **❌ Keine Setter ohne Validierung:** Immer `private set;` + Methode
- **❌ Keine Anemic Domain Models:** Properties + Getter/Setter ist Anti-Pattern
- **❌ Keine EF Core-Annotationen:** `[Required]`, `[MaxLength]` gehören in Infrastructure
- **❌ Keine DTOs in der Domain:** Domain kennt keine API-Konzepte

#### Application Layer
- **❌ Keine direkte DbContext-Nutzung:** Immer via Repository-Port
- **❌ Keine Geschäftslogik im Handler:** Handler orchestriert, Domain entscheidet
- **❌ Keine HTTP-Concerns:** Keine `HttpContext`, `ActionResult`, etc.
- **❌ Keine SQL-Queries:** EF Core-Queries gehören in Infrastructure

#### Infrastructure Layer
- **❌ Keine Abhängigkeit auf API-Layer:** Nie `using eLetter25.API`
- **❌ Keine Domain-Logik in Mappern:** Mapper sind dumm
- **❌ Keine Lazy Loading:** Explizites `Include()` verwenden
- **❌ Keine Migrations im Produktiv-Betrieb:** Separate Deployment-Pipeline

#### API Layer
- **❌ Keine Geschäftslogik in Endpoints:** Immer an Handler delegieren
- **❌ Keine direkte Repository-Nutzung:** Immer via MediatR
- **❌ Keine DB-Entities in Responses:** Immer DTOs/Results verwenden

---

## 8. Erweiterungen (Roadmap)

### 8.1 Fehlende Komponenten (Critical)

1. **Tests**
   - Unit Tests für Domain (z.B. `LetterTests.cs`)
   - Integration Tests für Repositories
   - API Tests (z.B. WebApplicationFactory)

2. **Validation**
   - FluentValidation für Commands
   - MediatR Pipeline Behavior für Validation

3. **Error Handling**
   - Global Exception Handler (Middleware)
   - ProblemDetails (RFC 7807)
   - Domain Exceptions → HTTP-Statuscodes Mapping

4. **Logging**
   - Structured Logging (Serilog)
   - MediatR Logging Behavior

### 8.2 Feature-Erweiterungen

- **Query Use Cases:** `GetLetterByIdQuery`, `SearchLettersQuery`
- **Update/Delete:** `UpdateLetterCommand`, `DeleteLetterCommand`
- **Pagination:** `PagedResult<T>`, `PagingParameters`
- **Domain Events:** `ILetterCreatedEvent`, Event Handler
- **Authorization:** Policy-based (z.B. nur eigene Briefe editieren)

### 8.3 Infrastruktur-Verbesserungen

- **Health Checks:** EF Core, SQL Server, Keycloak
- **Caching:** Distributed Cache (Redis) für Queries
- **Background Jobs:** z.B. Hangfire für asynchrone Verarbeitung
- **API Versioning:** URL-based oder Header-based

---

## 9. Bekannte Probleme

| Problem | Severity | Beschreibung | Lösung |
|---------|----------|--------------|--------|
| **ASPIRE004 Warning** | Low | Host referenziert `Infrastructure` | Referenz entfernen |
| **Health Endpoint geschützt** | Medium | `/health` benötigt Auth | `RequireAuthorization()` entfernen |
| **Fehlende Email-Validierung** | Medium | `Email` hat nur simple Prüfung | Regex oder externe Library |
| **Mutable Value Objects** | Low | `Correspondent` hat `set;` | `init;` verwenden (Breaking Change für EF Core) |
| **Keine Tests** | Critical | Keine Testprojekte | Testprojekte anlegen |
| **RootNamespace inkonsistent** | Low | `etter25.API` statt `eLetter25.API` | `.csproj` korrigieren |

---

## 10. Checkliste für neue Features

Beim Hinzufügen neuer Features diese Schritte befolgen:

### Domain
- [ ] Entity oder Value Object in `Domain/Letters/` oder `Domain/Shared/` anlegen
- [ ] Factory-Methode für Entity (`Create(...)`)
- [ ] Geschäftsregeln in Methoden, nicht Properties
- [ ] Domain Exceptions für Regel-Verletzungen
- [ ] Unit Tests für Entity-Logik

### Application
- [ ] Port (Interface) in `Application/*/Ports/` definieren
- [ ] Command/Query in `Application/*/UseCases/{Feature}/` anlegen
- [ ] Handler implementieren
- [ ] Result-DTO definieren
- [ ] Mapping DTO → Domain im Handler
- [ ] FluentValidation-Validator (optional)

### Infrastructure
- [ ] DB Entity in `Infrastructure/Persistence/*/` anlegen
- [ ] Mapper: Domain ↔ DB Entity
- [ ] Repository-Implementierung
- [ ] `IEntityTypeConfiguration<T>` für EF Core
- [ ] Migration erstellen (`Add-Migration`)
- [ ] Integration Tests

### API
- [ ] Endpoint in `Program.cs` oder Controller
- [ ] Request/Response DTOs
- [ ] HTTP-Statuscodes korrekt setzen
- [ ] Authorization-Policy (falls nötig)
- [ ] OpenAPI-Doku (Swagger Attributes)
- [ ] API Tests (WebApplicationFactory)

---

## 11. Tooling & Build

### 11.1 Entwicklung

- **IDE:** JetBrains Rider (oder Visual Studio 2025)
- **.NET SDK:** 10.0
- **Docker:** Für SQL Server + Keycloak (via Aspire)
- **EF Core Tools:** `dotnet ef` für Migrations

### 11.2 Nützliche Befehle

```powershell
# Solution bauen
dotnet build

# Migrations erstellen
dotnet ef migrations add <Name> --project eLetter25.Infrastructure --startup-project eLetter25.API

# Migrations anwenden (Dev)
dotnet ef database update --project eLetter25.Infrastructure --startup-project eLetter25.API

# Aspire Host starten
dotnet run --project eLetter25.Host

# Tests ausführen (wenn vorhanden)
dotnet test
```

---

## 12. Ressourcen

- **Clean Architecture:** [Clean Architecture – Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- **DDD:** [Domain-Driven Design – Eric Evans](https://www.domainlanguage.com/ddd/)
- **MediatR:** [GitHub – jbogard/MediatR](https://github.com/jbogard/MediatR)
- **EF Core:** [Microsoft Docs – Entity Framework Core](https://learn.microsoft.com/ef/core/)
- **.NET Aspire:** [Microsoft Docs – .NET Aspire](https://learn.microsoft.com/dotnet/aspire/)

---

**Ende der Dokumentation**

