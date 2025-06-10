using System.Security.Claims;
using Api.Domain.VOs;

namespace Api.Application.Interfaces
{
  public interface IJwtUtils
  {
    string GenerateAccessToken(Uuid userUuid);
    string GenerateRefreshToken(Uuid userUuid);
    Uuid? ValidateTokenAndGetSub(string? token);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
  }
}