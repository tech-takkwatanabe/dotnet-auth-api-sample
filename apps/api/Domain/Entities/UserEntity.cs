using System;
using Api.Domain.VOs;

namespace Api.Domain.Entities
{
  public class UserEntity : IEntity<Uuid>
  {
    public Uuid Id { get; private set; }
    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private UserEntity()
    {
      Id = Uuid.NewUuid();
      Name = new Name("Default Name");
      Email = new Email("default@example.com");
    }

    public UserEntity(Uuid id, Name name, Email email)
    {
      Id = id ?? throw new ArgumentNullException(nameof(id));
      Name = name ?? throw new ArgumentNullException(nameof(name));
      Email = email ?? throw new ArgumentNullException(nameof(email));
      CreatedAt = DateTime.UtcNow;
      UpdatedAt = DateTime.UtcNow;
    }
  }
}
