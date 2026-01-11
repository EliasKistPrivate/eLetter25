using eLetter25.Application.Common.Ports;
using eLetter25.Application.Letters.Contracts;
using eLetter25.Application.Letters.Ports;
using eLetter25.Application.Letters.UseCases.CreateLetter;
using eLetter25.Infrastructure.Auth.Data;
using eLetter25.Infrastructure.Persistence;
using eLetter25.Infrastructure.Persistence.Letters;
using eLetter25.Infrastructure.Persistence.Letters.Mappings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateLetterHandler).Assembly));

builder.Services.AddDbContext<MsIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("users-db")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MsIdentityDbContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("eletter25-db")));



builder.Services.AddScoped<ILetterDomainToDbMapper, LetterDomainToDbMapper>();
builder.Services.AddScoped<ILetterDbToDomainMapper, LetterDbToDomainMapper>();
builder.Services.AddScoped<ILetterRepository, EfLetterRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var appScope = app.Services.CreateScope();
    var appDbContext = appScope.ServiceProvider.GetRequiredService<AppDbContext>();
    appDbContext.Database.Migrate();

    using var identityScope = app.Services.CreateScope();
    var identityDbContext = identityScope.ServiceProvider.GetRequiredService<MsIdentityDbContext>();
    identityDbContext.Database.Migrate();

    var roleManager = identityScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
}

app.UseHttpsRedirection();

// standard middleware registrations

app.MapControllers();
app.MapGet("/", () => "eLetter25.API is running...");

app.MapGet("/health", () => Results.Ok("Healthy"));
app.MapPost("/letters", async (
    CreateLetterRequest request,
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(new CreateLetterCommand(request), ct);
    return Results.Created($"/letters/{result.LetterId}", result);
});

app.Run();