namespace eLetter25.Domain.ValueObjects;

/// <summary>
/// Represents a physical address.
/// </summary>
/// <param name="Street"></param>
/// <param name="PostalCode"></param>
/// <param name="City"></param>
/// <param name="Country"></param>
public sealed record Address(
    string Street,
    string PostalCode,
    string City,
    string Country);