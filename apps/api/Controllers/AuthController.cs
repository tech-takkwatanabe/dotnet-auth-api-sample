using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Application.UseCases.GetCurrentUser;
using Api.Application.UseCases.UserLogin;
using Api.Application.UseCases.UserRegistration;
using Api.Domain.DTOs;
using Api.Domain.VOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    RegisterUserCommandHandler registerUserCommandHandler,
    LoginUserCommandHandler loginUserCommandHandler,
    GetCurrentUserCommandHandler getCurrentUserCommandHandler,
    IUserService userService /* IUserService は GetCurrentUserCommandHandler に内包されるため、将来的には削除検討可能 */) : ControllerBase
{
  private readonly RegisterUserCommandHandler _registerUserCommandHandler = registerUserCommandHandler;
  private readonly LoginUserCommandHandler _loginUserCommandHandler = loginUserCommandHandler;
  private readonly GetCurrentUserCommandHandler _getCurrentUserCommandHandler = getCurrentUserCommandHandler;
  private readonly IUserService _userService = userService; // GetCurrentUserCommandHandler が IUserService を使用するため、この直接参照は不要になる可能性があります

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
  public async Task<IActionResult> Login([FromBody] LoginRequest request)
  {
    try
    {
      var command = new LoginUserCommand(request.Email, request.Password);
      var result = await _loginUserCommandHandler.HandleAsync(command);
      if (result.HasValue)
      {
        return Ok(new AuthResponse(result.Value.AccessToken, result.Value.RefreshToken, result.Value.UserUuid, "Bearer"));
      }
      return Unauthorized(new { message = "Invalid email or password." });
    }
    catch (Exception ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpGet("me")]
  [Authorize]
  [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetCurrentUser()
  {
    // HttpContext.User から認証済みユーザーのクレームを取得
    // JwtRegisteredClaimNames.Sub には、トークン生成時に設定したユーザーID (UUID) が格納されている
    var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

    if (string.IsNullOrEmpty(userIdString))
    {
      // 通常、[Authorize]属性により、認証されていない場合はここに到達しない
      return Unauthorized(new { message = "User ID not found in token." });
    }

    if (!Guid.TryParse(userIdString, out var userIdGuid))
    {
      // トークン内のユーザーIDが不正な形式の場合
      return BadRequest(new { message = "Invalid user ID format in token." });
    }

    var userUuid = new Uuid(userIdGuid);
    var command = new GetCurrentUserCommand(userUuid);
    var userResponse = await _getCurrentUserCommandHandler.HandleAsync(command);

    if (userResponse == null)
    {
      return NotFound(new { message = "User not found." });
    }

    return Ok(userResponse);
  }

  [HttpPost("refresh")]
  [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [Consumes("application/json")]
  public Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
  {
    // TODO: 実装
    // AuthResponse に Uuid を渡す必要があります。ここでは仮の Uuid を生成します。
    return Task.FromResult<IActionResult>(Ok(new AuthResponse("access_token", "refresh_token", Uuid.NewUuid())));
  }

  [HttpPost("logout")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public Task<IActionResult> Logout()
  {
    // TODO: 実装
    return Task.FromResult<IActionResult>(Ok(new { message = "Logged out successfully!!" }));
  }
}