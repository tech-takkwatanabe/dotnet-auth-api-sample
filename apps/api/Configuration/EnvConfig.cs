using System;

namespace Api.Infrastructure.Configuration
{
  /// <summary>
  /// Helper class to retrieve environment variables.
  /// Ensure DotNetEnv.Env.Load() is called before using this class.
  /// </summary>
  public static class EnvConfig
  {
    public static string GetString(string variableName, string defaultValue = "")
    {
      return Environment.GetEnvironmentVariable(variableName) ?? defaultValue;
    }

    public static int GetInt(string variableName, int defaultValue = 0)
    {
      string? value = Environment.GetEnvironmentVariable(variableName);
      return int.TryParse(value, out int result) ? result : defaultValue;
    }
  }
}
