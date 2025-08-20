using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace StorePOS.Api.Endpoints;

public static class AuthEndpointDocs
{
    public static readonly OpenApiMediaType UserLoginDtoExample = new()
    {
        Example = new OpenApiObject
        {
            ["usernameOrEmail"] = new OpenApiString("johndoe"),
            ["password"] = new OpenApiString("password123")
        }
    };

    public static readonly OpenApiMediaType AuthResponseDtoExample = new()
    {
        Example = new OpenApiObject
        {
            ["success"] = new OpenApiBoolean(true),
            ["message"] = new OpenApiString("Login successful"),
            ["data"] = new OpenApiObject
            {
                ["accessToken"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."),
                ["refreshToken"] = new OpenApiString("def50200e8a2f1a1b2c3d4e5f6789abcdef..."),
                ["expiresAt"] = new OpenApiString("2025-08-20T18:30:00.000Z"),
                ["user"] = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["username"] = new OpenApiString("johndoe"),
                    ["email"] = new OpenApiString("john.doe@example.com"),
                    ["firstName"] = new OpenApiString("John"),
                    ["lastName"] = new OpenApiString("Doe"),
                    ["phoneNumber"] = new OpenApiString("+1234567890"),
                    ["role"] = new OpenApiString("Cashier"),
                    ["isActive"] = new OpenApiBoolean(true),
                    ["createdAt"] = new OpenApiString("2024-01-15T10:30:00Z"),
                    ["lastLoginAt"] = new OpenApiString("2025-08-20T12:30:00Z")
                }
            }
        }
    };

    public static readonly OpenApiMediaType AuthResponseErrorExample = new()
    {
        Example = new OpenApiObject
        {
            ["success"] = new OpenApiBoolean(false),
            ["message"] = new OpenApiString("Invalid username/email or password"),
            ["data"] = new OpenApiNull()
        }
    };

    public static readonly OpenApiMediaType RefreshTokenDtoExample = new()
    {
        Example = new OpenApiObject
        {
            ["refreshToken"] = new OpenApiString("def50200e8a2f1a1b2c3d4e5f6789abcdef...")
        }
    };

    public static readonly OpenApiMediaType LogoutSuccessExample = new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Logged out successfully")
        }
    };

    public static readonly OpenApiMediaType LogoutErrorExample = new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Logout failed")
        }
    };

    public static readonly OpenApiMediaType LogoutAllSuccessExample = new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Logged out from all devices successfully")
        }
    };

    public static readonly OpenApiMediaType LogoutAllErrorExample = new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Logout all failed")
        }
    };

    public static readonly OpenApiMediaType CurrentUserExample = new()
    {
        Example = new OpenApiObject
        {
            ["id"] = new OpenApiInteger(1),
            ["username"] = new OpenApiString("johndoe"),
            ["email"] = new OpenApiString("john.doe@example.com"),
            ["role"] = new OpenApiString("Cashier"),
            ["isActive"] = new OpenApiBoolean(true)
        }
    };

    public static readonly OpenApiMediaType UnauthorizedExample = new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Unauthorized")
        }
    };
}
