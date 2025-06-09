using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Domain.VOs;

namespace Api.Domain.VOs.Converters
{
  public class NameJsonConverter : JsonConverter<Name>
  {
    public override Name? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType == JsonTokenType.String)
      {
        string? nameValue = reader.GetString();
        if (nameValue != null)
        {
          try
          {
            return new Name(nameValue);
          }
          catch (ArgumentException ex)
          {
            throw new JsonException($"Error converting value '{nameValue}' to Name: {ex.Message}");
          }
        }
      }
      throw new JsonException($"Expected string to convert to Name, but got {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, Name value, JsonSerializerOptions options)
    {
      writer.WriteStringValue(value.Value);
    }
  }
}