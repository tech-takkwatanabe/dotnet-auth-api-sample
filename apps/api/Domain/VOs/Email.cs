using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

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
      try
      {
        // MailAddress クラスを使ってメールアドレスの形式を検証
        _ = new MailAddress(value);
      }
      catch (FormatException)
      {
        throw new ArgumentException("Invalid email format.", nameof(value));
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