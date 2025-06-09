using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Domain.VOs;

namespace Api.Domain.VOs.Converters
{
  public class PasswordJsonConverter : JsonConverter<Password>
  {
    public override Password? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType == JsonTokenType.String)
      {
        string? passwordValue = reader.GetString();
        if (passwordValue != null)
        {
          try
          {
            return new Password(passwordValue);
          }
          catch (ArgumentException ex)
          {
            throw new JsonException($"Error converting value to Password: {ex.Message}", ex);
          }
        }
        return null;
      }
      else if (reader.TokenType == JsonTokenType.Null)
      {
        return null;
      }
      throw new JsonException($"Expected string to convert to Password, but got {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, Password value, JsonSerializerOptions options)
    {
      // ここでは ToString() を使用してマスクされた値を書き込みます。
      writer.WriteStringValue(value.ToString());
    }
  }
}
