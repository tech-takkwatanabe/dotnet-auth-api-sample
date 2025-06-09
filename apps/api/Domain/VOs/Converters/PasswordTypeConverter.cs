using System;
using System.ComponentModel;
using System.Globalization;
using Api.Domain.VOs;

namespace Api.Domain.VOs.Converters
{
  public class PasswordTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
      if (sourceType == typeof(string))
      {
        return true;
      }
      // Fallback to base implementation if not a string
      return base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value is string s)
      {
        return new Password(s);
      }
      return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
      if (destinationType == typeof(string))
      {
        return true;
      }
      return base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      if (destinationType == typeof(string) && value is Password password)
      {
        return password.ToString(); // パスワードの平文を返さず、マスクされた文字列を返します。
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
    {
      return false; // Password is treated as a single value, not expandable in property grid
    }

    public override PropertyDescriptorCollection? GetProperties(ITypeDescriptorContext? context, object? value, Attribute[]? attributes)
    {
      return null; // No properties to expose
    }
  }
}