using Microsoft.AspNetCore.Mvc;
using StorePOS.Domain.DTOs;
using StorePOS.Domain.Services;
using Microsoft.OpenApi.Models;
using StorePOS.Api.Authorization;
using StorePOS.Domain.Enums;

namespace StorePOS.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // GET: query list
        app.MapGet("/api/users", async ([FromServices] IUserService svc, [FromQuery] string? q, CancellationToken ct) =>
        {
            var items = await svc.SearchAsync(q, ct);
            return Results.Ok(items);
        })
        .WithName("GetUsers")
        .WithTags("Users")
        .WithSummary("Get all users or search users")
        .WithDescription("Returns a list of users. Optionally filter by username, email, first name, or last name with the 'q' query string.")
        .Produces<List<UserReadDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager))
        .WithOpenApi(op =>
        {
            op.Responses["200"] = new()
            {
                Description = "A list of users.",
                Content =
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Example = new Microsoft.OpenApi.Any.OpenApiArray
                        {
                            UserEndpointDocs.UserReadDtoExample.Example!,
                            UserEndpointDocs.UserReadDtoExample2.Example!
                        }
                    }
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // GET: by id
        app.MapGet("/api/users/{id:int}", async ([FromServices] IUserService svc, int id, CancellationToken ct) =>
        {
            var dto = await svc.GetByIdAsync(id, ct);
            return dto is null ? Results.NotFound() : Results.Ok(dto);
        })
        .WithName("GetUserById")
        .WithTags("Users")
        .WithSummary("Get user by ID")
        .WithDescription("Returns a user that matches the given ID.")
        .Produces<UserReadDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager))
        .WithOpenApi(op =>
        {
            op.Responses["200"] = new()
            {
                Description = "A user matching the ID.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.UserReadDtoExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "User not found.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.NotFoundExample
                }
            };
            return op;
        });

        // GET: by username
        app.MapGet("/api/users/by-username/{username}", async ([FromServices] IUserService svc, string username, CancellationToken ct) =>
        {
            var dto = await svc.GetByUsernameAsync(username, ct);
            return dto is null ? Results.NotFound() : Results.Ok(dto);
        })
        .WithName("GetUserByUsername")
        .WithTags("Users")
        .WithSummary("Get user by username")
        .WithDescription("Returns a user that matches the given username.")
        .Produces<UserReadDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager))
        .WithOpenApi(op =>
        {
            op.Responses["200"] = new()
            {
                Description = "A user matching the username.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.UserReadDtoExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "User not found.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.NotFoundExample
                }
            };
            return op;
        });

        // POST: create user
        app.MapPost("/api/users", async ([FromServices] IUserService svc, UserCreateDto dto, CancellationToken ct) =>
        {
            var res = await svc.CreateAsync(dto, ct);
            return EndpointHelpers.MapResult(res, d => $"/api/users/{d.Id}");
        })
        .WithName("CreateUser")
        .WithTags("Users")
        .WithSummary("Create a new user")
        .WithDescription("Creates a new user and returns the created entity.")
        .Accepts<UserCreateDto>("application/json")
        .Produces<UserReadDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status409Conflict)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin))
        .WithOpenApi(op =>
        {
            op.RequestBody = new()
            {
                Content =
                {
                    ["application/json"] = UserEndpointDocs.UserCreateDtoExample
                }
            };
            op.Responses["201"] = new()
            {
                Description = "User created successfully.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.UserReadDtoExample
                }
            };
            op.Responses["409"] = new()
            {
                Description = "Username or email already exists.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.ConflictExample
                }
            };
            return op;
        });

        // PUT: update user
        app.MapPut("/api/users/{id:int}", async ([FromServices] IUserService svc, int id, UserUpdateDto dto, CancellationToken ct) =>
        {
            var res = await svc.UpdateAsync(id, dto, ct);
            return EndpointHelpers.MapResult(res);
        })
        .WithName("UpdateUser")
        .WithTags("Users")
        .WithSummary("Update an existing user")
        .WithDescription("Updates an existing user and returns the updated entity.")
        .Accepts<UserUpdateDto>("application/json")
        .Produces<UserReadDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status409Conflict)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin))
        .WithOpenApi(op =>
        {
            op.RequestBody = new()
            {
                Content =
                {
                    ["application/json"] = UserEndpointDocs.UserUpdateDtoExample
                }
            };
            op.Responses["200"] = new()
            {
                Description = "User updated successfully.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.UserReadDtoExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "User not found.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.NotFoundExample
                }
            };
            op.Responses["409"] = new()
            {
                Description = "Username or email already exists.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.ConflictExample
                }
            };
            return op;
        });

        // PUT: change password
        app.MapPut("/api/users/{id:int}/password", async ([FromServices] IUserService svc, int id, UserChangePasswordDto dto, CancellationToken ct) =>
        {
            var res = await svc.ChangePasswordAsync(id, dto, ct);
            return EndpointHelpers.MapResult(res);
        })
        .WithName("ChangeUserPassword")
        .WithTags("Users")
        .WithSummary("Change user password")
        .WithDescription("Changes the password for an existing user.")
        .Accepts<UserChangePasswordDto>("application/json")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin))
        .WithOpenApi(op =>
        {
            op.RequestBody = new()
            {
                Content =
                {
                    ["application/json"] = UserEndpointDocs.UserChangePasswordDtoExample
                }
            };
            op.Responses["204"] = new()
            {
                Description = "Password changed successfully."
            };
            op.Responses["404"] = new()
            {
                Description = "User not found.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.NotFoundExample
                }
            };
            return op;
        });

        // DELETE: delete user
        app.MapDelete("/api/users/{id:int}", async ([FromServices] IUserService svc, int id, CancellationToken ct) =>
        {
            var res = await svc.DeleteAsync(id, ct);
            return EndpointHelpers.MapResult(res);
        })
        .WithName("DeleteUser")
        .WithTags("Users")
        .WithSummary("Delete a user")
        .WithDescription("Deletes an existing user.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin))
        .WithOpenApi(op =>
        {
            op.Responses["204"] = new()
            {
                Description = "User deleted successfully."
            };
            op.Responses["404"] = new()
            {
                Description = "User not found.",
                Content =
                {
                    ["application/json"] = UserEndpointDocs.NotFoundExample
                }
            };
            return op;
        });
    }
}
