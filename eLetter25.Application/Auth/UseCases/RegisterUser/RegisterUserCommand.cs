using eLetter25.Application.Auth.Contracts;
using MediatR;

namespace eLetter25.Application.Auth.UseCases.RegisterUser;

public sealed record RegisterUserCommand(RegisterUserRequest Request) : IRequest<RegisterUserResult>;