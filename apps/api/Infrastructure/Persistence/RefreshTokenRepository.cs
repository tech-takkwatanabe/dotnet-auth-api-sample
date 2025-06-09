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

    public async Task<RefreshTokenEntity?> FindByUuidAsync(Uuid uuid)
    {
      var tokenKey = GetRedisKey(uuid);
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
      var tokenKey = GetRedisKey(refreshToken.Id);
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

    public async Task DeleteByUuidAsync(Uuid uuid)
    {
      var tokenKey = GetRedisKey(uuid);
      await _redis.KeyDeleteAsync(tokenKey);
    }

    private static string GetRedisKey(Uuid tokenId)
    {
      return $"refreshtoken:{tokenId.Value}";
    }
  }
}