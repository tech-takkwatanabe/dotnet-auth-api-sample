using System;

namespace Api.Domain.VOs
{
  public class Uuid : IEquatable<Uuid>
  {
    public Guid Value { get; }

    public Uuid(Guid value)
    {
      if (value == Guid.Empty)
      {
        throw new ArgumentException("Uuid cannot be empty.", nameof(value));
      }
      Value = value;
    }

    public static Uuid NewUuid() => new(Guid.NewGuid());

    public override bool Equals(object? obj)
    {
      return Equals(obj as Uuid);
    }

    public bool Equals(Uuid? other)
    {
      return other != null && Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Value);
    }

    public override string ToString()
    {
      return Value.ToString();
    }
  }
}