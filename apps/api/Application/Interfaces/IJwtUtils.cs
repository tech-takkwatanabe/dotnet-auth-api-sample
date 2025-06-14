using System.Security.Claims;
using Api.Domain.VOs;

namespace Api.Application.Interfaces
{
  public interface IJwtUtils
  {
    string GenerateAccessToken(Uuid userUuid);
    // RefreshToken生成時にJTIも返す
    (string Token, Uuid Jti) GenerateRefreshToken(Uuid userUuid);

    // トークン検証時にSubとJTIの両方を返す
    (Uuid? Sub, Uuid? Jti) ValidateTokenAndGetSubAndJti(string? token);

    // 有効期限切れトークンからPrincipalを取得するメソッド
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
  }
}