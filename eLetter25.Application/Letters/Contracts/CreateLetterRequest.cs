using eLetter25.Application.Shared.DTOs;
using eLetter25.Domain.Letters.ValueObjects;

namespace eLetter25.Application.Letters.Contracts;

public sealed record CreateLetterRequest(
    string Subject,
    DateTimeOffset SentDate,
    CorrespondentDto Sender,
    CorrespondentDto Recipient,
    IEnumerable<string> Tags
);