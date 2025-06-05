using Api.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  [HttpPost("signup")]
  [ProducesResponseType(typeof(SignUpResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [Consumes("application/json")]
  public Task<IActionResult> SignUp([FromBody] SignUpRequest request)
  {
    // TODO: 実装
    return Task.FromResult<IActionResult>(Ok(new SignUpResponse("UserName", "hoge@example.com")));
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
    return Task.FromResult<IActionResult>(Ok(new UserResponse("xxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "user@example.com", "username")));
  }
}