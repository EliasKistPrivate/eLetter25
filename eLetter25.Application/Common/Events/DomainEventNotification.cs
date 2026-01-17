using eLetter25.Domain.Common;
using MediatR;

namespace eLetter25.Application.Common.Events;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
