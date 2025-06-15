using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Api.Domain.VOs.Converters;

namespace Api.Domain.VOs
{
  [TypeConverter(typeof(NameTypeConverter))]
  [JsonConverter(typeof(NameJsonConverter))]
  public class Name : IEquatable<Name>
  {
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string Value { get; }

    public Name(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException("Name cannot be empty.", nameof(value));
      }
      if (value.Length < 1 || value.Length > 100)
      {
        throw new ArgumentOutOfRangeException(nameof(value), "Name must be between 1 and 100 characters long.");
      }
      Value = value;
    }

    public override bool Equals(object? obj)
    {
      if (ReferenceEquals(this, obj)) return true;
      if (obj is null || obj.GetType() != GetType()) return false;
      return Equals(obj as Name);
    }

    public bool Equals(Name? other)
    {
      if (other is null) return false;
      return string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Value);
    }

    public override string ToString()
    {
      return Value;
    }
  }
}