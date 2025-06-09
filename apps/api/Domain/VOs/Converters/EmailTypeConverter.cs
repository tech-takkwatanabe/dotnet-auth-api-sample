using System;
using System.ComponentModel;
using System.Globalization;

namespace Api.Domain.VOs.Converters
{
  public class EmailTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
      if (sourceType == typeof(string))
      {
        return true;
      }
      return base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value is string s)
      {
        // Create a new Email instance from the string input
        return new Email(s);
      }
      // Defer to the base implementation for other types
      return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
      if (destinationType == null)
      {
        return false;
      }
      return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    // Override ConvertTo to convert Email to string.
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      if (destinationType == null)
      {
        throw new ArgumentNullException(nameof(destinationType));
      }
      if (value == null)
      {
        return base.ConvertTo(context, culture, value, destinationType);
      }
      if (destinationType == typeof(string) && value is Email email)
      {
        return email.Value;
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    // Override IsValid to validate Email strings in property grids.
    public override bool IsValid(ITypeDescriptorContext? context, object? value)
    {
      if (value is string s)
      {
        try
        {
          var email = new Email(s);
          return true;
        }
        catch
        {
          return false;
        }
      }
      return base.IsValid(context, value);
    }
  }
}