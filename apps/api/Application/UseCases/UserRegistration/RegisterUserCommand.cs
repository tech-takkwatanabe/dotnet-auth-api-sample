namespace Api.Application.UseCases.UserRegistration
{
  public record RegisterUserCommand(
      string Name,
      string Email,
      string Password
  );
}