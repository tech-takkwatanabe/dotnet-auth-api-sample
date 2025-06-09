using System;
using System.ComponentModel;
using System.Globalization;
using Api.Domain.VOs;


namespace Api.Domain.VOs.Converters
{
  public class UuidTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
      // 文字列から Guid への変換をサポート
      return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }
      if (value is string s)
      {
        if (string.IsNullOrWhiteSpace(s))
        {
          throw new ArgumentException("Input string cannot be null or whitespace.", nameof(value));
        }
        // Guid.TryParse を使用して安全に変換
        if (Guid.TryParse(s, out Guid result))
        {
          // Ensure Uuid is defined elsewhere as a value object wrapping Guid
          return new Uuid(result);
        }
        throw new FormatException("The provided string is not a valid GUID format.");
      }
      return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
      return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      if (destinationType == typeof(string) && value is Uuid uuid)
      {
        return uuid.Value.ToString();
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}