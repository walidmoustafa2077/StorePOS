using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StorePOS.Api.Authorization;
using StorePOS.Domain.DTOs;
using StorePOS.Domain.Services;

namespace StorePOS.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            // POST: Login
            app.MapPost("/api/auth/login", async (
                [FromServices] IAuthService authService,
                [FromBody] UserLoginDto loginDto,
                HttpContext context,
                CancellationToken ct) =>
            {
                var ipAddress = context.GetIpAddress();
                var result = await authService.LoginAsync(loginDto, ipAddress, ct);
                
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                
                return Results.BadRequest(result);
            })
            .WithName("Login")
            .WithTags("Authentication")
            .WithSummary("User login")
            .WithDescription("Authenticate user and return access token and refresh token")
            .Produces<AuthResponseDto>(StatusCodes.Status200OK)
            .Produces<AuthResponseDto>(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            .WithOpenApi(op =>
            {
                op.RequestBody = new()
                {
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.UserLoginDtoExample
                    }
                };
                op.Responses["200"] = new()
                {
                    Description = "Login successful",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.AuthResponseDtoExample
                    }
                };
                op.Responses["400"] = new()
                {
                    Description = "Invalid credentials",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.AuthResponseErrorExample
                    }
                };
                return op;
            });

            // POST: Refresh Token
            app.MapPost("/api/auth/refresh", async (
                [FromServices] IAuthService authService,
                [FromBody] RefreshTokenDto refreshTokenDto,
                HttpContext context,
                CancellationToken ct) =>
            {
                var ipAddress = context.GetIpAddress();
                var result = await authService.RefreshTokenAsync(refreshTokenDto.RefreshToken, ipAddress, ct);
                
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                
                return Results.BadRequest(result);
            })
            .WithName("RefreshToken")
            .WithTags("Authentication")
            .WithSummary("Refresh access token")
            .WithDescription("Use refresh token to get a new access token")
            .Produces<AuthResponseDto>(StatusCodes.Status200OK)
            .Produces<AuthResponseDto>(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            .WithOpenApi(op =>
            {
                op.RequestBody = new()
                {
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.RefreshTokenDtoExample
                    }
                };
                op.Responses["200"] = new()
                {
                    Description = "Token refresh successful",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.AuthResponseDtoExample
                    }
                };
                op.Responses["400"] = new()
                {
                    Description = "Invalid refresh token",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.AuthResponseErrorExample
                    }
                };
                return op;
            });

            // POST: Logout
            app.MapPost("/api/auth/logout", async (
                [FromServices] IAuthService authService,
                [FromBody] RefreshTokenDto refreshTokenDto,
                HttpContext context,
                CancellationToken ct) =>
            {
                var ipAddress = context.GetIpAddress();
                var success = await authService.LogoutAsync(refreshTokenDto.RefreshToken, ipAddress, ct);
                
                if (success)
                {
                    return Results.Ok(new { message = "Logged out successfully" });
                }
                
                return Results.BadRequest(new { message = "Logout failed" });
            })
            .WithName("Logout")
            .WithTags("Authentication")
            .WithSummary("User logout")
            .WithDescription("Revoke refresh token and logout user")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization()
            .WithOpenApi(op =>
            {
                op.RequestBody = new()
                {
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.RefreshTokenDtoExample
                    }
                };
                op.Responses["200"] = new()
                {
                    Description = "Logout successful",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.LogoutSuccessExample
                    }
                };
                op.Responses["400"] = new()
                {
                    Description = "Logout failed",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.LogoutErrorExample
                    }
                };
                return op;
            });

            // POST: Logout All Devices
            app.MapPost("/api/auth/logout-all", async (
                [FromServices] IAuthService authService,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var userId = user.GetUserId();
                var success = await authService.LogoutAllAsync(userId, ct);
                
                if (success)
                {
                    return Results.Ok(new { message = "Logged out from all devices successfully" });
                }
                
                return Results.BadRequest(new { message = "Logout all failed" });
            })
            .WithName("LogoutAll")
            .WithTags("Authentication")
            .WithSummary("Logout from all devices")
            .WithDescription("Revoke all refresh tokens for the current user")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization()
            .WithOpenApi(op =>
            {
                op.Responses["200"] = new()
                {
                    Description = "Logout from all devices successful",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.LogoutAllSuccessExample
                    }
                };
                op.Responses["400"] = new()
                {
                    Description = "Logout all failed",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.LogoutAllErrorExample
                    }
                };
                return op;
            });

            // GET: Current User Info
            app.MapGet("/api/auth/me", (ClaimsPrincipal user) =>
            {
                if (!user.Identity?.IsAuthenticated ?? false)
                {
                    return Results.Unauthorized();
                }

                var userInfo = new
                {
                    Id = user.GetUserId(),
                    Username = user.GetUsername(),
                    Email = user.GetEmail(),
                    Role = user.GetRole().ToString(),
                    IsActive = user.IsActive()
                };

                return Results.Ok(userInfo);
            })
            .WithName("GetCurrentUser")
            .WithTags("Authentication")
            .WithSummary("Get current user information")
            .WithDescription("Returns information about the currently authenticated user")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization()
            .WithOpenApi(op =>
            {
                op.Responses["200"] = new()
                {
                    Description = "Current user information",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.CurrentUserExample
                    }
                };
                op.Responses["401"] = new()
                {
                    Description = "Unauthorized",
                    Content =
                    {
                        ["application/json"] = AuthEndpointDocs.UnauthorizedExample
                    }
                };
                return op;
            });
        }
    }
}
