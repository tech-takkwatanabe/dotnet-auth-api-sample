using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Domain.VOs
{
  public class Email : IEquatable<Email>
  {
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Value { get; }

    public Email(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException("Email cannot be empty.", nameof(value));
      }
      Value = value;
    }

    public override bool Equals(object? obj)
    {
      return Equals(obj as Email);
    }

    public bool Equals(Email? other)
    {
      return other != null && Value == other.Value;
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