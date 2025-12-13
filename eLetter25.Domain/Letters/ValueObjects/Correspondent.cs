using eLetter25.Domain.ValueObjects;

namespace eLetter25.Domain.Letters.ValueObjects;

/// <summary>
/// Represents a correspondent involved in the letter exchange.
/// </summary>
/// <param name="Name"></param>
/// <param name="Address"></param>
/// <param name="Email"></param>
/// <param name="Phone"></param>
public sealed record Correspondent(
    string Name,
    Address Address,
    string? Email,
    string? Phone);