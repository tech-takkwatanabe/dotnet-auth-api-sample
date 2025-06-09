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
            // Console.WriteLine($"DEBUG: PasswordJsonConverter.Read attempting to create Password from '{passwordValue}'"); // デバッグログ
            return new Password(passwordValue);
          }
          catch (ArgumentException ex) // Passwordコンストラクタのバリデーションエラーをキャッチ
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
      // Console.WriteLine($"DEBUG: PasswordJsonConverter.Write writing Password value (masked): '{value.ToString()}'"); // デバッグログ
      // パスワードの平文をレスポンスに含めるべきではありません。
      // ToString()がマスクされた値を返すことを期待するか、
      // もしくはこのValueObjectを直接レスポンスに含めない設計を検討してください。
      // ここでは ToString() を使用してマスクされた値を書き込みます。
      writer.WriteStringValue(value.ToString());
    }
  }
}
