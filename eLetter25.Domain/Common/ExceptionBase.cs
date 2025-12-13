using System.Diagnostics.CodeAnalysis;

namespace eLetter25.Domain.Common;

/// <summary>
/// Base type for domain-specific exceptions in the eLetter25 domain layer.
/// </summary>
public abstract class ExceptionBase : Exception
{
    /// <summary>
    /// A stable, technical error code that can be used for logging and client mapping.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Optional human-readable details that further describe the error.
    /// </summary>
    public string? Details { get; }

    protected ExceptionBase(string message, string errorCode, string? details)
        : this(message, errorCode, details, null)
    {
    }

    protected ExceptionBase(string message, string errorCode, Exception innerException)
        : this(message, errorCode, null, innerException)
    {
    }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected ExceptionBase(string message, string errorCode, string? details = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Details = details;

        Data["ErrorCode"] = errorCode;
        if (!string.IsNullOrWhiteSpace(details))
        {
            Data["Details"] = details;
        }
    }
}