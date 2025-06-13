using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Api.Infrastructure.Middleware;

public static class AuthenticationEvents
{
  public static Task HandleChallengeAsync(JwtBearerChallengeContext context)
  {
    context.HandleResponse();

    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    context.Response.ContentType = "application/json";

    var isTokenExpired = context.Error == "invalid_token" &&
                         context.ErrorDescription != null &&
                         context.ErrorDescription.Contains("expired", StringComparison.OrdinalIgnoreCase);

    object errorResponse;
    if (isTokenExpired)
    {
      errorResponse = new { message = "Access token has expired.", errorCode = "TOKEN_EXPIRED" };
    }
    else
    {
      errorResponse = new { message = "Authentication failed.", errorCode = "AUTHENTICATION_FAILURE" };
    }

    return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
  }
}