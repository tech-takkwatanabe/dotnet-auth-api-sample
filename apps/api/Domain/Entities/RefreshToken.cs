using System;
using Api.Domain.VOs;

namespace Api.Domain.Entities
{
  public class RefreshToken(Uuid id, string token, Uuid userId, DateTime expiresAt)
  {
    public Uuid Id { get; private set; } = id ?? throw new ArgumentNullException(nameof(id));
    public string Token { get; private set; } = !string.IsNullOrWhiteSpace(token) ? token : throw new ArgumentException("Token cannot be empty.", nameof(token));
    public Uuid UserId { get; private set; } = userId ?? throw new ArgumentNullException(nameof(userId));
    public DateTime ExpiresAt { get; private set; } = expiresAt;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private RefreshToken() : this(null!, null!, null!, DateTime.MinValue) { }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public void Revoke()
    {
      ExpiresAt = DateTime.UtcNow.AddDays(-1);
      UpdatedAt = DateTime.UtcNow;
    }
  }
}