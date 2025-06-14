using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Domain.DTOs;

namespace Api.Application.UseCases.GetCurrentUser;

public class GetCurrentUserCommandHandler(IUserService userService)
{
  private readonly IUserService _userService = userService;

  public async Task<UserResponse?> HandleAsync(GetCurrentUserCommand command)
  {
    var user = await _userService.GetUserByUuidAsync(command.UserUuid);

    if (user == null)
    {
      return null;
    }

    return new UserResponse(
        user.Uuid,
        user.Email,
        user.Name
    );
  }
}