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
      return GenerateToken(userId, DateTime.UtcNow.AddSeconds(_jwtSettings.AccessTokenExpirationSeconds));
    }

    public string GenerateRefreshToken(Uuid userId)
    {
      return GenerateToken(userId, DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshTokenExpirationSeconds));
    }

    private string GenerateToken(Uuid userId, DateTime expires)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
          {
            new Claim(JwtRegisteredClaimNames.Sub, userId.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
          }),
        Expires = expires,
        Issuer = _jwtSettings.Issuer,
        Audience = _jwtSettings.Audience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
      };
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }

    public Uuid? ValidateTokenAndGetSub(string? token)
    {
      var principal = GetPrincipalFromToken(token, validateLifetime: true);
      if (principal == null) return null;

      var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
      if (string.IsNullOrEmpty(userIdClaim))
      {
        userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      }
      return Guid.TryParse(userIdClaim, out var userIdGuid) ? new Uuid(userIdGuid) : null;
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