using System;
using System.ComponentModel;
using System.Text.Json.Serialization; // JsonConverter属性のために追加
using Api.Domain.VOs.Converters; // Converters 名前空間を using

namespace Api.Domain.VOs
{
  [TypeConverter(typeof(UuidTypeConverter))]
  [JsonConverter(typeof(UuidJsonConverter))] // JsonConverter属性を追加
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

    public static bool operator ==(Uuid? left, Uuid? right)
    {
      if (ReferenceEquals(left, right))
        return true;
      if (left is null || right is null)
        return false;
      return left.Equals(right);
    }

    public static bool operator !=(Uuid? left, Uuid? right)
    {
      return !(left == right);
    }
  }
}