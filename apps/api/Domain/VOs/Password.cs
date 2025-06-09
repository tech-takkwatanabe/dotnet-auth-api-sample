using System;
using System.ComponentModel;
using System.Text.Json.Serialization; // JsonConverter属性のために追加
using Api.Domain.VOs.Converters; // Converters 名前空間を using

namespace Api.Domain.VOs
{
  [TypeConverter(typeof(PasswordTypeConverter))]
  [JsonConverter(typeof(PasswordJsonConverter))] // JsonConverter属性を追加
  public class Password : IEquatable<Password>
  {
    public string Value { get; }

    // パスワードの最小長を定義
    private const int MinimumLength = 6;

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