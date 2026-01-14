using eLetter25.Infrastructure.Auth.Data;
using eLetter25.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace etter25.API.Features;

public static class RegisterUser
{
    public record Request(string Email, string Password, bool EnableNotifications = false);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("register",
            async (Request request, AppDbContext appDbContext, UserManager<ApplicationUser> userManager) =>
            {
                await using var transaction = await appDbContext.Database.BeginTransactionAsync();
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    EnableNotifications = request.EnableNotifications
                };

                var identityResult = await userManager.CreateAsync(user, request.Password);
                if (!identityResult.Succeeded)
                {
                    return Results.BadRequest(identityResult.Errors);
                }

                var addToRoleResult = await userManager.AddToRoleAsync(user, "User");
                if (!addToRoleResult.Succeeded)
                {
                    return Results.BadRequest(addToRoleResult.Errors);
                }

                await transaction.CommitAsync();

                return Results.Ok();
            });
    }
}