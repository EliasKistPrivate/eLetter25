using eLetter25.Application.Auth.Contracts;
using MediatR;

namespace eLetter25.Application.Auth.UseCases.LoginUser;

public sealed record LoginUserCommand(LoginUserRequest Request) : IRequest<LoginUserResult>;