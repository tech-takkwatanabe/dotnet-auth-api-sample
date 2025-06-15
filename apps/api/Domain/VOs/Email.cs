using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json.Serialization;
using Api.Configuration;
using Api.Domain.VOs.Converters;

namespace Api.Domain.VOs
{
  [TypeConverter(typeof(EmailTypeConverter))]
  [JsonConverter(typeof(EmailJsonConverter))]
  public class Email : IEquatable<Email>
  {
    [Required]
    [EmailAddress]
    [MinLength(Const.EmailMinLength)]
    [MaxLength(Const.EmailMaxLength)]
    public string Value { get; }

    public Email(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException("Email cannot be empty.", nameof(value));
      }
      if (value.Length < Const.EmailMinLength || value.Length > Const.EmailMaxLength)
      {
        throw new ArgumentOutOfRangeException(nameof(value), $"Email must be between {Const.EmailMinLength} and {Const.EmailMaxLength} characters long.");
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
