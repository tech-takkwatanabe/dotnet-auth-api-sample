namespace Api.Application.UseCases.UserLogout;

public record UserLogoutCommand(
    string RefreshTokenValue
);