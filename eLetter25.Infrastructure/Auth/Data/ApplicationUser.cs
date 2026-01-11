using Microsoft.AspNetCore.Identity;

namespace eLetter25.Infrastructure.Auth.Data;

public sealed class ApplicationUser : IdentityUser
{
    public bool EnableNotifications { get; set; }
}