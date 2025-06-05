using System.Security.Claims;
using Api.Domain.VOs;

namespace Api.Application.Interfaces
{
  public interface IJwtUtils
  {
    string GenerateAccessToken(Uuid userId);
    string GenerateRefreshToken(Uuid userId);
    Uuid? ValidateTokenAndGetUserId(string? token);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
  }
}