namespace eLetter25.Application.Auth.Ports;

public interface IUserRegistrationService
{
    Task<string> RegisterUserAsync(
        string email,
        string password,
        bool enableNotifications,
        CancellationToken cancellationToken = default);
}