using eLetter25.Application.Auth.Contracts;
using eLetter25.Application.Auth.UseCases.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eLetter25.API.Auth.Controllers;

/// <summary>
/// Controller for user registration
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class RegisterController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success confirmation</returns>
    /// <response code="200">User successfully registered</response>
    /// <response code="400">Validation error or registration failed</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await mediator.Send(new RegisterUserCommand(request), cancellationToken);
            return Ok(new { userId = result.UserId, message = result.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}