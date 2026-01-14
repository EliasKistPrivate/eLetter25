using System.Security.Claims;
using System.Text;
using eLetter25.Infrastructure.Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace etter25.API.Features;

public static class LoginUser
{
    public record Request(string Email, string Password);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("login", async (Request request, UserManager<ApplicationUser> userManager,
            IOptions<JwtOptions> jwtOptions
        ) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Results.Unauthorized();
            }


            var options = jwtOptions.Value;

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var roles = await userManager.GetRolesAsync(user);

            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                ..roles.Select(r => new Claim(ClaimTypes.Role, r))
            ];

            // Dynamisch Issuer und Audience aus der aktuellen Request-URL ermitteln (für Aspire)

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(options.ExpirationMinutes),
                Issuer = options.Issuer,
                Audience = options.Audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JsonWebTokenHandler();
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);

            return Results.Ok(new { accessToken });
        });
    }
}