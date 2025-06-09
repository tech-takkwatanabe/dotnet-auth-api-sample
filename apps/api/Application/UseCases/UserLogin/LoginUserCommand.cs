using Api.Domain.VOs;

namespace Api.Application.UseCases.UserLogin
{
  public record LoginUserCommand(
      Email Email,
      Password Password
  );
}