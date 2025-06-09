using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Domain.DTOs;
using Api.Domain.VOs;

namespace Api.Application.UseCases.UserRegistration
{
  public class RegisterUserCommandHandler(IUserService userService)
  {
    private readonly IUserService _userService = userService;

    public async Task HandleAsync(RegisterUserCommand command)
    {
      var signUpRequest = new SignUpRequest(command.Name, command.Email, command.Password);
      await _userService.RegisterUserAsync(signUpRequest);
    }
  }
}