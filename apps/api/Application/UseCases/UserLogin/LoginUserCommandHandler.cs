using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Domain.VOs;

namespace Api.Application.UseCases.UserLogin
{
  public class LoginUserCommandHandler(IUserService userService)
  {
    private readonly IUserService _userService = userService;

    public async Task<(string AccessToken, string RefreshToken, Uuid UserUuid)?> HandleAsync(LoginUserCommand command)
    {
      var loginRequest = new Domain.DTOs.LoginRequest(command.Email, command.Password);
      var result = await _userService.LoginUserAsync(loginRequest);

      if (result.HasValue)
      {
        return result.Value;
      }
      return null;
    }
  }
}