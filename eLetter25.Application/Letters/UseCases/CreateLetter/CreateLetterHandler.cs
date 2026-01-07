using eLetter25.Application.Common.Ports;
using eLetter25.Application.Letters.Ports;
using eLetter25.Application.Shared.DTOs;
using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.ValueObjects;
using eLetter25.Domain.Shared.ValueObjects;
using MediatR;

namespace eLetter25.Application.Letters.UseCases.CreateLetter;

public sealed class CreateLetterHandler(ILetterRepository letterRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateLetterCommand, CreateLetterResult>
{
    public async Task<CreateLetterResult> Handle(CreateLetterCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var sender = MapToDomain(request.Sender);
        var recipient = MapToDomain(request.Recipient);

        var letter = Letter.Create(sender, recipient, request.SentDate)
            .SetSubject(request.Subject);

        letter = request.Tags.Select(tagName => new Tag(tagName))
            .Aggregate(letter, (current, tag) => current.AddTag(tag));

        await letterRepository.AddAsync(letter, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return new CreateLetterResult(letter.Id);
    }

    private static Correspondent MapToDomain(CorrespondentDto dto)
    {
        Email? email = string.IsNullOrWhiteSpace(dto.Email)
            ? null
            : new Email(dto.Email);

        PhoneNumber? phoneNumber = string.IsNullOrWhiteSpace(dto.Phone)
            ? null
            : new PhoneNumber(dto.Phone);

        return new Correspondent(
            dto.Name,
            new Address(
                dto.Address.Street,
                dto.Address.PostalCode,
                dto.Address.City,
                dto.Address.Country),
            email,
            phoneNumber);
    }
}