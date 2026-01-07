using eLetter25.Application.Common.Ports;
using eLetter25.Application.Letters.Contracts;
using eLetter25.Application.Letters.Ports;
using eLetter25.Application.Letters.UseCases.CreateLetter;
using eLetter25.Infrastructure.Persistence;
using eLetter25.Infrastructure.Persistence.Letters;
using eLetter25.Infrastructure.Persistence.Letters.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateLetterHandler).Assembly));

builder.Services.AddAuthentication().AddKeycloakJwtBearer(
    serviceName: "keycloak",
    realm: "eLetter25API",
    options =>
    {
        options.Audience = "eLetter25.API";

        if (builder.Environment.IsDevelopment())
        {
            options.RequireHttpsMetadata = false;
        }
    });

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("eletter25db");

    if (string.IsNullOrEmpty(cs))
    {
        throw new InvalidOperationException("Connection string 'eletter25db' not found.");
    }

    options.UseSqlServer(cs);
});

builder.Services.AddScoped<ILetterDomainToDbMapper, LetterDomainToDbMapper>();
builder.Services.AddScoped<ILetterDbToDomainMapper, LetterDbToDomainMapper>();
builder.Services.AddScoped<ILetterRepository, EfLetterRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// standard middleware registrations

app.MapControllers();
app.MapGet("/", () => "eLetter25.API is running...");

app.MapGet("/health", () => Results.Ok("Healthy")).RequireAuthorization();
app.MapPost("/letters", async (
    CreateLetterRequest request,
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(new CreateLetterCommand(request), ct);
    return Results.Created($"/letters/{result.LetterId}", result);
});

app.Run();