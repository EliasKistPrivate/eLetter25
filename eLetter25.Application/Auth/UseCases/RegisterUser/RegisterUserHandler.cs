using eLetter25.Application.Auth.Ports;
using MediatR;

namespace eLetter25.Application.Auth.UseCases.RegisterUser;

public sealed class RegisterUserHandler(IUserRegistrationService userRegistrationService)
    : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var userId = await userRegistrationService.RegisterUserAsync(
            request.Email,
            request.Password,
            request.EnableNotifications,
            cancellationToken);
        return new RegisterUserResult(userId, "User successfully registered");
    }
}