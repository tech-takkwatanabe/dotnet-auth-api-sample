using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Domain.VOs;

namespace Api.Domain.VOs.Converters
{
  public class EmailJsonConverter : JsonConverter<Email>
  {
    public override Email? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType == JsonTokenType.String)
      {
        string? emailValue = reader.GetString();
        if (emailValue != null)
        {
          try
          {
            return new Email(emailValue);
          }
          catch (ArgumentException ex)
          {
            throw new JsonException($"Error converting value '{emailValue}' to Email: {ex.Message}", ex);
          }
        }
      }
      throw new JsonException($"Expected string to convert to Email, but got {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, Email value, JsonSerializerOptions options)
    {
      writer.WriteStringValue(value.Value);
    }
  }
}