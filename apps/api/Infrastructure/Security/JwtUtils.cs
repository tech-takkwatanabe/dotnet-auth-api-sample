using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Application.Interfaces;
using Api.Domain.VOs;
using Api.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Infrastructure.Security
{
  public class JwtUtils : IJwtUtils
  {
    private readonly JwtSettings _jwtSettings;

    public JwtUtils(IOptions<JwtSettings> jwtSettings)
    {
      if (jwtSettings == null || jwtSettings.Value == null)
      {
        throw new ArgumentNullException(nameof(jwtSettings), "JWT settings must be provided.");
      }
      _jwtSettings = jwtSettings.Value;
      if (string.IsNullOrWhiteSpace(_jwtSettings.Key) || _jwtSettings.Key.Length < 32)
      {
        throw new ArgumentException("The JWT secret key must be set and contain at least 32 characters.");
      }
    }

    public string GenerateAccessToken(Uuid userId)
    {
      // GenerateTokenがタプルを返すようになったため、トークン文字列のみを取得
      var (token, _) = GenerateToken(userId, DateTime.UtcNow.AddSeconds(_jwtSettings.AccessTokenExpirationSeconds));
      return token;
    }

    public (string Token, Uuid Jti) GenerateRefreshToken(Uuid userId)
    {
      return GenerateToken(userId, DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshTokenExpirationSeconds), true);
    }

    // JTIも生成して返すように変更
    private (string Token, Uuid Jti) GenerateToken(Uuid userId, DateTime expires, bool isRefreshToken = false)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
      var jti = Guid.NewGuid(); // JTIを生成
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
          {
            new Claim(JwtRegisteredClaimNames.Sub, userId.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()) // メソッド内で生成したjtiを使用
          }),
        // Subject = new ClaimsIdentity(claims),
        Expires = expires,
        Issuer = _jwtSettings.Issuer,
        Audience = _jwtSettings.Audience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
      };
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
      return (tokenHandler.WriteToken(token), new Uuid(jti));
    }

    public (Uuid? Sub, Uuid? Jti) ValidateTokenAndGetSubAndJti(string? token)
    {
      var principal = GetPrincipalFromToken(token, validateLifetime: true);
      if (principal == null) return (null, null);

      var subClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
      if (string.IsNullOrEmpty(subClaim))
      {
        // ASP.NET Core Identity のデフォルトの NameIdentifier もフォールバックとして確認 (通常は不要)
        subClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      }

      var jtiClaim = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

      Uuid? sub = Guid.TryParse(subClaim, out var subGuid) ? new Uuid(subGuid) : null;
      Uuid? jti = Guid.TryParse(jtiClaim, out var jtiGuid) ? new Uuid(jtiGuid) : null;

      return (sub, jti);
    }


    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
      return GetPrincipalFromToken(token, validateLifetime: false);
    }

    private ClaimsPrincipal? GetPrincipalFromToken(string? token, bool validateLifetime)
    {
      if (string.IsNullOrEmpty(token))
        return null;

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
      try
      {
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = true,
          ValidIssuer = _jwtSettings.Issuer,
          ValidateAudience = true,
          ValidAudience = _jwtSettings.Audience,
          ValidateLifetime = validateLifetime,
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        return principal;
      }
      catch (SecurityTokenException)
      {
        return null;
      }
    }
  }
}