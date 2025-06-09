using Api.Domain.VOs;

namespace Api.Application.UseCases.UserRegistration
{
  public record RegisterUserCommand(
      Name Name,
      Email Email,
      Password Password
  );
}