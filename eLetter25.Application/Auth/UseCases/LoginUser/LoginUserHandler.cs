using eLetter25.Application.Auth.Ports;
using MediatR;

namespace eLetter25.Application.Auth.UseCases.LoginUser;

public sealed class LoginUserHandler(
    IUserAuthenticationService userAuthenticationService,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var credentials = await userAuthenticationService.ValidateCredentialsAsync(
            request.Email,
            request.Password,
            cancellationToken);
        if (credentials is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var (userId, email, roles) = credentials.Value;
        var token = jwtTokenGenerator.GenerateToken(userId, email, roles);
        return new LoginUserResult(token);
    }
}