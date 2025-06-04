namespace Api.Domain.DTOs;

public record SignUpRequest(
    string UserName,
    string Email,
    string Password
);

public record LoginRequest(
    string Email,
    string Password
);

public record RefreshTokenRequest(
    string RefreshToken
);

public record SignUpResponse(
    string UserName,  
    string Email
);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType = "Bearer"
);

public record UserResponse(
    string Uuid,
    string Email,
    string UserName
); 

public record LogoutResponse(
    string Message
); 