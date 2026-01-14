using eLetter25.Application.Auth.Contracts;
using eLetter25.Application.Auth.UseCases.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eLetter25.API.Auth.Controllers;

/// <summary>
/// Controller for user authentication
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class LoginController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT access token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await mediator.Send(new LoginUserCommand(request), cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}