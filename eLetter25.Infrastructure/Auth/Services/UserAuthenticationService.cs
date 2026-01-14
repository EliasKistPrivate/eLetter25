using eLetter25.Application.Auth.Ports;
using eLetter25.Infrastructure.Auth.Data;
using Microsoft.AspNetCore.Identity;

namespace eLetter25.Infrastructure.Auth.Services;

public sealed class UserAuthenticationService(UserManager<ApplicationUser> userManager)
    : IUserAuthenticationService
{
    public async Task<(string UserId, string Email, IEnumerable<string> Roles)?> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || !await userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        return (user.Id, user.Email!, roles);
    }
}