namespace eLetter25.Application.Auth.Ports;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, IEnumerable<string> roles);
}