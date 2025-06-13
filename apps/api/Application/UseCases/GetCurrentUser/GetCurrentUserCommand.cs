using Api.Domain.VOs;

namespace Api.Application.UseCases.GetCurrentUser;

public record GetCurrentUserCommand(
    Uuid UserId
);