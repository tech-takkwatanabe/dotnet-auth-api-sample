using System;
using System.ComponentModel;
using System.Globalization;
using Api.Domain.VOs;

namespace Api.Domain.VOs.Converters
{
  public class NameTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
      return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }
      if (value is string strValue)
      {
        return new Name(strValue);
      }
      return base.ConvertFrom(context, culture, value!);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
      if (destinationType == null)
      {
        throw new ArgumentNullException(nameof(destinationType));
      }
      return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      if (destinationType == typeof(string) && value is Name nameInstance)
      {
        return nameInstance.Value;
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}