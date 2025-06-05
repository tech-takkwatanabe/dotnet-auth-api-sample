using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Application.Interfaces;
using Api.Infrastructure.Configuration;
using Api.Infrastructure.Security;
using Api.Infrastructure.Settings;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Kestrelの設定
builder.WebHost.ConfigureKestrel(options =>
{
  // HTTP設定
  options.ListenAnyIP(8080, listenOptions =>
  {
    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
  });

  // HTTPS設定
  options.ListenAnyIP(8443, listenOptions =>
  {
    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    var certPath = Environment.GetEnvironmentVariable("SSL_CERT_PATH") ?? "/https/localhost-cert.pem";
    var keyPath = Environment.GetEnvironmentVariable("SSL_KEY_PATH") ?? "/https/localhost-key.pem";
    var cert = X509Certificate2.CreateFromPemFile(certPath, keyPath);
    listenOptions.UseHttps(cert, httpsOptions =>
      {
        httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        httpsOptions.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.NoCertificate;
        httpsOptions.AllowAnyClientCertificate();
        httpsOptions.CheckCertificateRevocation = false;
      });
  });
});

// .env ファイルから環境変数を読み込む (アプリケーションの早い段階で)
Env.Load();

// 1. JWT設定を環境変数から読み込むように変更
builder.Services.AddSingleton(new JwtSettings
{
  Key = EnvConfig.GetString("JWT_KEY"),
  Issuer = EnvConfig.GetString("JWT_ISSUER"),
  Audience = EnvConfig.GetString("JWT_AUDIENCE"),
  AccessTokenExpirationSeconds = EnvConfig.GetInt("ACCESS_TOKEN_EXPIRE_SECONDS", 900),
  RefreshTokenExpirationSeconds = EnvConfig.GetInt("REFRESH_TOKEN_EXPIRE_SECONDS", 3600)
});


// 2. JWT UtilsをDIコンテナに登録
builder.Services.AddSingleton<IJwtUtils, JwtUtils>();

// 3. JWT認証ミドルウェアの設定 (コントローラーで [Authorize] を使う場合)
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  var serviceProvider = builder.Services.BuildServiceProvider();
  var jwtSettings = serviceProvider.GetRequiredService<JwtSettings>();

  if (string.IsNullOrEmpty(jwtSettings.Key) || jwtSettings.Key.Length < 32)
  {
    throw new InvalidOperationException("JWT Key must be configured via environment variables (JWT_KEY) and be at least 32 characters long for JWT authentication.");
  }
  options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidIssuer = jwtSettings.Issuer,
    ValidateAudience = true,
    ValidAudience = jwtSettings.Audience,
    ValidateLifetime = true,
    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
    ValidateIssuerSigningKey = true,
    ClockSkew = TimeSpan.Zero
  };
  // Add services to the container.
  // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
  builder.Services.AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      });
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(c =>
  {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
  });

  builder.Services.AddHealthChecks();
  builder.Services.AddAuthorization();

  var app = builder.Build();

  // Configure the HTTP request pipeline.
  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
  }

  app.UseRouting();
  app.UseHttpsRedirection();
  app.UseAuthentication();
  app.UseAuthorization();
  app.MapControllers();

  app.MapHealthChecks("/health");

  app.Run();
});
