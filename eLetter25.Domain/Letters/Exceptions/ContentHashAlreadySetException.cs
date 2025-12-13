using eLetter25.Domain.Common;
using eLetter25.Domain.ValueObjects;

namespace eLetter25.Domain.Letters.Exceptions;

/// <summary>
/// Thrown when a <see cref="LetterDocument"/> already has a content hash set
/// and a different hash is attempted to be stored.
/// </summary>
public sealed class ContentHashAlreadySetException : ExceptionBase
{
    private const string DefaultErrorCode = "LETTER_DOCUMENT_CONTENT_HASH_ALREADY_SET";

    /// <summary>
    /// The identifier of the affected document entity.
    /// </summary>
    public Guid DocumentId { get; }

    /// <summary>
    /// The existing content hash value.
    /// </summary>
    public ContentHash ExistingContentHash { get; }

    /// <summary>
    /// The content hash value that was attempted to be set.
    /// </summary>
    public ContentHash AttemptedContentHash { get; }

    public ContentHashAlreadySetException(
        Guid documentId,
        ContentHash existingContentHash,
        ContentHash attemptedContentHash,
        string? details = null,
        Exception? innerException = null)
        : base(
            $"Content hash for document '{documentId}' is already set and differs from the attempted value.",
            DefaultErrorCode,
            details,
            innerException)
    {
        ArgumentNullException.ThrowIfNull(existingContentHash);
        ArgumentNullException.ThrowIfNull(attemptedContentHash);

        DocumentId = documentId;
        ExistingContentHash = existingContentHash;
        AttemptedContentHash = attemptedContentHash;
    }
}