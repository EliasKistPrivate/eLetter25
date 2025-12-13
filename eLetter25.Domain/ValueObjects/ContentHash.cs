namespace eLetter25.Domain.ValueObjects;

public sealed record ContentHash
{
    public string Value { get; }

    private ContentHash(string value)
    {
        Value = value;
    }

    public static ContentHash FromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            throw new ArgumentException("Content hash must not be empty.", nameof(hex));
        }

        hex = hex.Trim().ToLowerInvariant();

        // SHA-256 = 32 bytes = 64 hex characters
        if (hex.Length != 64)
        {
            throw new ArgumentException("Invalid SHA-256 hash length. Expected 64 hex characters.",
                nameof(hex));
        }

        if (hex.Select(c => c
                is >= '0'
                and <= '9'
                or >= 'a'
                and <= 'f')
            .Any(isHex => !isHex))
        {
            throw new ArgumentException("Content hash contains invalid hex characters.",
                nameof(hex));
        }

        return new ContentHash(hex);
    }

    public override string ToString() => Value;
}