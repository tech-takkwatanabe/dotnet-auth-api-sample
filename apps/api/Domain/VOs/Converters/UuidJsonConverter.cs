using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Domain.VOs;

namespace Api.Domain.VOs.Converters
{
  public class UuidJsonConverter : JsonConverter<Uuid>
  {
    public override Uuid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType == JsonTokenType.String)
      {
        string? uuidString = reader.GetString();
        if (uuidString != null && Guid.TryParse(uuidString, out Guid guidValue))
        {
          try
          {
            return new Uuid(guidValue);
          }
          catch (ArgumentException ex) // Uuidコンストラクタのバリデーションエラーをキャッチ
          {
            throw new JsonException($"Error converting value '{uuidString}' to Uuid: {ex.Message}", ex);
          }
        }
      }
      else if (reader.TokenType == JsonTokenType.Null)
      {
        // If the JSON token is null, the converter for Uuid should return null.
        return null;
      }
      throw new JsonException($"Expected string to convert to Uuid, but got {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, Uuid value, JsonSerializerOptions options)
    {
      writer.WriteStringValue(value.Value.ToString());
    }
  }
}