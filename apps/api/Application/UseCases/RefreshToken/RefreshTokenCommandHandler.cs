using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Domain.DTOs; // AuthResponse を使用するため

namespace Api.Application.UseCases.RefreshToken;

public class RefreshTokenCommandHandler(IUserService userService)
{
  private readonly IUserService _userService = userService;

  public async Task<AuthResponse?> HandleAsync(RefreshTokenCommand command)
  {
    var result = await _userService.RefreshAccessTokenAsync(command.RefreshTokenValue);

    if (!result.HasValue)
    {
      return null;
    }

    var (accessToken, refreshToken, userUuid) = result.Value;
    return new AuthResponse(accessToken, refreshToken, userUuid, "Bearer");
  }
}