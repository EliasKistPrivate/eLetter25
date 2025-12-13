using eLetter25.Domain.Common;
using eLetter25.Domain.ValueObjects;
using eLetter25.Domain.Letters.Exceptions;

namespace eLetter25.Domain.Letters;

/// <summary>
/// Represents a document associated with a letter.
/// </summary>
public class LetterDocument(DocumentFormat documentFormat) : DomainEntity
{
    public DocumentFormat DocumentFormat { get; private set; } = documentFormat;
    private ContentHash? ContentHash { get; set; }
    public long? SizeInBytes { get; private set; }

    public void SetStoredMetadata(ContentHash contentHash, long sizeInBytes)
    {
        if (ContentHash is not null && ContentHash != contentHash)
        {
            throw new ContentHashAlreadySetException(Id, ContentHash, contentHash);
        }

        ArgumentOutOfRangeException.ThrowIfNegative(sizeInBytes);

        ContentHash = contentHash;
        SizeInBytes = sizeInBytes;
    }
}