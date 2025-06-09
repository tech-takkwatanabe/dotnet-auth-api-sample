using Api.Domain.VOs;

namespace Api.Domain.DTOs;

public record SignUpRequest(
    Name Name,
    Email Email,
    Password Password
);

public record LoginRequest(
    Email Email,
    Password Password
);

public record RefreshTokenRequest(
    string RefreshToken
);

public record SignUpResponse(
    Name Name,
    Email Email
);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType = "Bearer"
);

public record UserResponse(
    Uuid Uuid,
    Email Email,
    Name Name
);

public record LogoutResponse(
    string Message
);