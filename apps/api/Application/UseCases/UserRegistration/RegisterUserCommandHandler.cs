using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Domain.VOs;

namespace Api.Application.UseCases.UserRegistration
{
  public class RegisterUserCommandHandler(IUserService userService)
  {
    private readonly IUserService _userService = userService;

    public async Task HandleAsync(RegisterUserCommand command)
    {
      var name = new Name(command.Name);
      var email = new Email(command.Email);
      var passwordVO = new Password(command.Password);

      await _userService.RegisterUserAsync(name, email, passwordVO.Value);
    }
  }
}