namespace eLetter25.Application.Auth.Contracts;

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    bool EnableNotifications = false);