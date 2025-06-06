using System;
using Api.Domain.VOs;

namespace Api.Domain.Entities
{
  public class RefreshTokenEntity(Uuid id, string token, Uuid userId, DateTime expiresAt) // Constructor parameters updated
  {
    public Uuid Id { get; private set; } = id ?? throw new ArgumentNullException(nameof(id)); // Renamed Uuid to Id
    public string Token { get; private set; } = !string.IsNullOrWhiteSpace(token) ? token : throw new ArgumentException("Token cannot be empty.", nameof(token));
    public Uuid UserId { get; private set; } = userId ?? throw new ArgumentNullException(nameof(userId)); // Changed UserId type to Uuid
    public DateTime ExpiresAt { get; private set; } = expiresAt;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private RefreshTokenEntity() : this(Uuid.NewUuid(), string.Empty, Uuid.NewUuid(), DateTime.MinValue) { } // Updated EF Core constructor

    public void UpdateToken(string newToken, DateTime newExpiresAt)
    {
      if (string.IsNullOrWhiteSpace(newToken))
      {
        throw new ArgumentException("New token cannot be empty.", nameof(newToken));
      }
      if (newExpiresAt <= DateTime.UtcNow)
      {
        throw new ArgumentException("New expiration date must be in the future.", nameof(newExpiresAt));
      }

      Token = newToken;
      ExpiresAt = newExpiresAt;
      UpdatedAt = DateTime.UtcNow;
    }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public void Revoke()
    {
      ExpiresAt = DateTime.UtcNow.AddDays(-1);
      UpdatedAt = DateTime.UtcNow;
    }
  }
}