using Api.Application.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt; // Alias to avoid naming conflicts if any

namespace Api.Infrastructure.Security
{
  public class PasswordHasher : IPasswordHasher
  {
    public string Hash(string password)
    {
      return BCryptNet.HashPassword(password);
    }

    public bool Verify(string password, string hashedPassword)
    {
      return BCryptNet.Verify(password, hashedPassword);
    }
  }
}