using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Api.Domain.VOs.Converters;

namespace Api.Domain.VOs
{
  [TypeConverter(typeof(PasswordTypeConverter))]
  [JsonConverter(typeof(PasswordJsonConverter))]
  public class Password : IEquatable<Password>
  {
    private const int MinimumLength = 6;

    [Required]
    [MinLength(MinimumLength)]
    [MaxLength(100)]
    public string Value { get; }

    public Password(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException("Password cannot be empty.", nameof(Password));
      }
      if (value.Length < MinimumLength)
      {
        throw new ArgumentException($"Password must be at least {MinimumLength} characters long.", nameof(Password));
      }
      Value = value;
    }

    public override bool Equals(object? obj)
    {
      return Equals(obj as Password);
    }

    public bool Equals(Password? other)
    {
      return other != null && Value == other.Value;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Value);
    }

    public override string ToString()
    {
      return "****"; // セキュリティのため、実際のパスワード値は返さない
    }
  }
}