using eLetter25.Application.Auth.Ports;
using eLetter25.Infrastructure.Auth.Data;
using eLetter25.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace eLetter25.Infrastructure.Auth.Services;

public sealed class UserRegistrationService(
    UserManager<ApplicationUser> userManager,
    AppDbContext appDbContext) : IUserRegistrationService
{
    public async Task<string> RegisterUserAsync(
        string email,
        string password,
        bool enableNotifications,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellationToken);

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EnableNotifications = enableNotifications
        };

        var identityResult = await userManager.CreateAsync(user, password);
        if (!identityResult.Succeeded)
        {
            var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User registration failed: {errors}");
        }

        var addToRoleResult = await userManager.AddToRoleAsync(user, "User");
        if (!addToRoleResult.Succeeded)
        {
            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Role assignment failed: {errors}");
        }

        await transaction.CommitAsync(cancellationToken);

        return user.Id;
    }
}