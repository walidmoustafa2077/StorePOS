using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace StorePOS.Api.Endpoints;

public static class UserEndpointDocs
{
    public static readonly OpenApiMediaType UserReadDtoExample = new()
    {
        Example = new OpenApiObject
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
            ["lastLoginAt"] = new OpenApiString("2024-01-20T09:15:00Z")
        }
    };

    public static readonly OpenApiMediaType UserReadDtoExample2 = new()
    {
        Example = new OpenApiObject
        {
            ["id"] = new OpenApiInteger(2),
            ["username"] = new OpenApiString("janesmith"),
            ["email"] = new OpenApiString("jane.smith@example.com"),
            ["firstName"] = new OpenApiString("Jane"),
            ["lastName"] = new OpenApiString("Smith"),
            ["phoneNumber"] = new OpenApiString("+1234567891"),
            ["role"] = new OpenApiString("Manager"),
            ["isActive"] = new OpenApiBoolean(true),
            ["createdAt"] = new OpenApiString("2024-01-10T14:20:00Z"),
            ["lastLoginAt"] = new OpenApiString("2024-01-19T16:45:00Z")
        }
    };

    public static readonly OpenApiMediaType UserCreateDtoExample = new()
    {
        Example = new OpenApiObject
        {
            ["username"] = new OpenApiString("newuser"),
            ["email"] = new OpenApiString("newuser@example.com"),
            ["firstName"] = new OpenApiString("New"),
            ["lastName"] = new OpenApiString("User"),
            ["password"] = new OpenApiString("securepassword123"),
            ["phoneNumber"] = new OpenApiString("+1234567892"),
            ["role"] = new OpenApiString("Cashier"),
            ["isActive"] = new OpenApiBoolean(true)
        }
    };

    public static readonly OpenApiMediaType UserUpdateDtoExample = new()
    {
        Example = new OpenApiObject
        {
            ["username"] = new OpenApiString("updateduser"),
            ["email"] = new OpenApiString("updated@example.com"),
            ["firstName"] = new OpenApiString("Updated"),
            ["lastName"] = new OpenApiString("User"),
            ["phoneNumber"] = new OpenApiString("+1234567893"),
            ["role"] = new OpenApiString("Manager"),
            ["isActive"] = new OpenApiBoolean(true)
        }
    };

    public static readonly OpenApiMediaType UserChangePasswordDtoExample = new()
    {
        Example = new OpenApiObject
        {
            ["currentPassword"] = new OpenApiString("currentpassword123"),
            ["newPassword"] = new OpenApiString("newpassword123")
        }
    };

    public static readonly OpenApiMediaType UserLoginDtoExample = new()
    {
        Example = new OpenApiObject
        {
            ["usernameOrEmail"] = new OpenApiString("johndoe"),
            ["password"] = new OpenApiString("password123")
        }
    };

    public static readonly OpenApiMediaType NotFoundExample = new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("User not found")
        }
    };

    public static readonly OpenApiMediaType ConflictExample = new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Username or email already exists")
        }
    };

    public static readonly OpenApiMediaType BadRequestExample = new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Invalid username/email or password")
        }
    };

    public static readonly OpenApiMediaType SuccessExample = new()
    {
        Example = new OpenApiObject
        {
            ["success"] = new OpenApiBoolean(true)
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
