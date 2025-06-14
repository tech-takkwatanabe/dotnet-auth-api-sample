using System;
using System.Threading.Tasks;
using Api.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Api.Application.UseCases.UserLogout;

public class UserLogoutCommandHandler(IUserService userService, ILogger<UserLogoutCommandHandler> logger)
{
  private readonly IUserService _userService = userService;
  private readonly ILogger<UserLogoutCommandHandler> _logger = logger;

  public async Task HandleAsync(UserLogoutCommand command)
  {
    try
    {
      await _userService.LogoutAsync(command.RefreshTokenValue);
    }
    catch (ArgumentException ex)
    {
      _logger.LogWarning(ex, "Logout attempt with invalid or non-existent refresh token. Token value (first 10 chars): {RefreshTokenStart}", command.RefreshTokenValue.Length > 10 ? command.RefreshTokenValue.Substring(0, 10) : command.RefreshTokenValue);
      throw;
    }
  }
}