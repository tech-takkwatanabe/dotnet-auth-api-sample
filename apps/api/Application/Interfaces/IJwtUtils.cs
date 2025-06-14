using System.Security.Claims;
using Api.Domain.VOs;

namespace Api.Application.Interfaces
{
  public interface IJwtUtils
  {
    string GenerateAccessToken(Uuid userUuid);
    // RefreshToken生成時にJTIも返すように変更
    (string Token, Uuid Jti) GenerateRefreshToken(Uuid userUuid);

    // トークン検証時にSubとJTIの両方を返すように変更
    // メソッド名を変更して意図を明確化
    (Uuid? Sub, Uuid? Jti) ValidateTokenAndGetSubAndJti(string? token);

    // 有効期限切れトークンからPrincipalを取得するメソッド (これは変更なし)
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
  }
}