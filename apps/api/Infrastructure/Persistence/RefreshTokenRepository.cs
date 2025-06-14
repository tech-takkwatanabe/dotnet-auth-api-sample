using System;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.Repositories;
using Api.Domain.VOs;
using StackExchange.Redis;

namespace Api.Infrastructure.Persistence
{
  public class RefreshTokenRepository(IConnectionMultiplexer redisConnection, TimeSpan? tokenExpiry = null) : IRefreshTokenRepository
  {
    private readonly IDatabase _redis = redisConnection.GetDatabase();
    private readonly TimeSpan _tokenExpiry = tokenExpiry ?? TimeSpan.FromDays(7);

    // JTI (JWT ID) を使用してリフレッシュトークンを検索します。
    public async Task<RefreshTokenEntity?> FindByJtiAsync(Uuid jti)
    {
      var tokenKey = GetRedisKeyForJti(jti);
      var tokenJson = await _redis.StringGetAsync(tokenKey);
      if (tokenJson.IsNullOrEmpty)
      {
        return null;
      }
      // JSONからRefreshTokenEntityにデシリアライズ
      // System.Text.Json を使用する場合
      return JsonSerializer.Deserialize<RefreshTokenEntity>(tokenJson.ToString());
    }

    public async Task SaveAsync(RefreshTokenEntity refreshToken)
    {
      // refreshToken.Id には JTI が設定されていることを前提とします。
      var tokenKey = GetRedisKeyForJti(refreshToken.Id);
      // RefreshTokenEntityをJSONにシリアライズ
      // System.Text.Json を使用する場合
      var tokenJson = JsonSerializer.Serialize(refreshToken);
      // Redisに保存し、有効期限を設定
      // 有効期限は refreshToken.ExpiresAt と Redis のキーの有効期限の短い方を取るなどの考慮も可能
      var actualExpiry = refreshToken.ExpiresAt - DateTime.UtcNow;
      if (actualExpiry <= TimeSpan.Zero) actualExpiry = _tokenExpiry; // 既に期限切れならデフォルト

      var finalExpiry = actualExpiry < _tokenExpiry ? actualExpiry : _tokenExpiry;
      await _redis.StringSetAsync(tokenKey, tokenJson, finalExpiry);
    }

    // JTI (JWT ID) を使用してリフレッシュトークンを削除します。
    public async Task DeleteByJtiAsync(Uuid jti)
    {
      var tokenKey = GetRedisKeyForJti(jti);
      await _redis.KeyDeleteAsync(tokenKey);
    }

    // JTIを元にRedisのキーを生成します。
    private static string GetRedisKeyForJti(Uuid jti)
    {
      return $"refreshtoken:jti:{jti.Value}"; // JTIであることをキーに含めることで、以前の形式と区別
    }
  }
}