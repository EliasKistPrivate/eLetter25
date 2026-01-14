namespace eLetter25.Application.Auth.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpirationMinutes { get; init; } = 2;
}