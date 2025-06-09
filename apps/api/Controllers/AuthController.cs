using System;
using System.Threading.Tasks;
using Api.Application.UseCases.UserRegistration;
using Api.Domain.DTOs;
using Api.Domain.VOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(RegisterUserCommandHandler registerUserCommandHandler) : ControllerBase
{
  private readonly RegisterUserCommandHandler _registerUserCommandHandler = registerUserCommandHandler;

  [HttpPost("signup")]
  [ProducesResponseType(typeof(SignUpResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  [Consumes("application/json")]
  public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
  {
    try
    {
      var command = new RegisterUserCommand(request.Name, request.Email, request.Password);
      await _registerUserCommandHandler.HandleAsync(command);
      return Ok(new SignUpResponse(request.Name, request.Email));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Email address is already in use"))
    {
      return Conflict(new { message = ex.Message });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpPost("login")]
  [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [Consumes("application/json")]
  public Task<IActionResult> Login([FromBody] LoginRequest request)
  {
    // TODO: 実装
    return Task.FromResult<IActionResult>(Ok(new AuthResponse("access_token", "refresh_token")));
  }

  [HttpPost("refresh")]
  [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [Consumes("application/json")]
  public Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
  {
    // TODO: 実装
    return Task.FromResult<IActionResult>(Ok(new AuthResponse("access_token", "refresh_token")));
  }

  [HttpPost("logout")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public Task<IActionResult> Logout()
  {
    // TODO: 実装
    return Task.FromResult<IActionResult>(Ok(new { message = "Logged out successfully!!" }));
  }

  [HttpGet("me")]
  [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public Task<IActionResult> GetCurrentUser()
  {
    // TODO: 実装
    // 仮のデータをValueObjectで作成
    var uuid = new Uuid(Guid.NewGuid()); // 仮のUUIDを生成
    var email = new Email("user@example.com"); // 仮のEmail
    var name = new Name("username"); // 仮のName
    return Task.FromResult<IActionResult>(Ok(new UserResponse(uuid, email, name)));
  }
}