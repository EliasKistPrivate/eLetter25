using System.Text;
using eLetter25.Application.Auth.Options;
using eLetter25.Application.Auth.Ports;
using eLetter25.Application.Common.Ports;
using eLetter25.Application.Letters.Ports;
using eLetter25.Application.Letters.UseCases.CreateLetter;
using eLetter25.Infrastructure.Auth.Data;
using eLetter25.Infrastructure.Auth.Services;
using eLetter25.Infrastructure.Persistence;
using eLetter25.Infrastructure.Persistence.Letters;
using eLetter25.Infrastructure.Persistence.Letters.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Options Pattern für JWT-Konfiguration mit Validierung
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey), "Jwt:SecretKey fehlt")
    .ValidateOnStart();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateLetterHandler).Assembly));

builder.Services.AddDbContext<MsIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("users-db")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MsIdentityDbContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("eletter25-db")));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((options, jwtOpt) =>
    {
        var jwt = jwtOpt.Value;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,
            ValidAudience = jwt.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });


builder.Services.AddAuthorization();

// Application Services
builder.Services.AddScoped<ILetterDomainToDbMapper, LetterDomainToDbMapper>();
builder.Services.AddScoped<ILetterDbToDomainMapper, LetterDbToDomainMapper>();
builder.Services.AddScoped<ILetterRepository, EfLetterRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Auth Services
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();

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

// Middleware and Endpoints

app.MapGet("/", () => "eLetter25.API is running...");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();