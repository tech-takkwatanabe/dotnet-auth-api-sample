using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
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
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
