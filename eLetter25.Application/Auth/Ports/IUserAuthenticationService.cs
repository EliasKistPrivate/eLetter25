namespace eLetter25.Application.Auth.Ports;

public interface IUserAuthenticationService
{
    Task<(string UserId, string Email, IEnumerable<string> Roles)?> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}