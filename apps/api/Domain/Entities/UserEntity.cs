using System;
using Api.Domain.VOs;

namespace Api.Domain.Entities
{
  public class UserEntity : IEntity<int>
  {
    public int Id { get; private set; }
    public Uuid Uuid { get; private set; }
    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public string? PasswordHash { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private UserEntity() // EF Core用
    {
      Uuid = Uuid.NewUuid();
      Name = null!;
      Email = null!;
      CreatedAt = DateTime.UtcNow;
      UpdatedAt = DateTime.UtcNow;
    }

    public UserEntity(Uuid uuid, Name name, Email email)
    {
      Uuid = uuid ?? throw new ArgumentNullException(nameof(uuid));
      Name = name ?? throw new ArgumentNullException(nameof(name));
      Email = email ?? throw new ArgumentNullException(nameof(email));
      CreatedAt = DateTime.UtcNow;
      UpdatedAt = DateTime.UtcNow;
    }
  }
}
