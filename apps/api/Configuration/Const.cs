namespace Api.Configuration
{
  public static class Const
  {
    // Email
    public const int EmailMinLength = 5;
    public const int EmailMaxLength = 320;

    // Name
    public const int NameMinLength = 1;
    public const int NameMaxLength = 100;

    // Password (Plain text)
    public const int PasswordMinLength = 6;
    public const int PasswordMaxLength = 100;

    // PasswordHash (Stored in DB)
    public const int PasswordHashMaxLength = 255;
  }
}