using Microsoft.AspNetCore.Mvc;
using StorePOS.Domain.DTOs;
using StorePOS.Domain.Services;
using StorePOS.Api.Authorization;
using StorePOS.Domain.Enums;

namespace StorePOS.Api.Endpoints;

public static class SaleEndpoints
{
    public static void MapSaleEndpoints(this IEndpointRouteBuilder app)
    {
        // GET: query list with optional date filtering
        app.MapGet("/api/sales", async ([FromServices] ISaleService svc, 
            [FromQuery] DateTimeOffset? from, 
            [FromQuery] DateTimeOffset? to, 
            CancellationToken ct) =>
        {
            var items = await svc.SearchAsync(from, to, ct);
            return Results.Ok(items);
        })
        .WithName("GetSales")
        .WithTags("Sales")
        .WithSummary("Get all sales with optional date filtering")
        .WithDescription("Returns a list of sales. Optionally filter by date range using 'from' and 'to' query parameters.")
        .Produces<List<SaleReadDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager, UserRole.Cashier))
        .WithOpenApi(op =>
        {
            op.Responses["200"] = new()
            {
                Description = "A list of sales",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.SaleListExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // GET: by id
        app.MapGet("/api/sales/{id:int}", async ([FromServices] ISaleService svc, int id, CancellationToken ct) =>
        {
            var dto = await svc.GetByIdAsync(id, ct);
            return dto is null ? Results.NotFound() : Results.Ok(dto);
        })
        .WithName("GetSaleById")
        .WithTags("Sales")
        .WithSummary("Get sale by ID")
        .WithDescription("Returns a specific sale by its ID.")
        .Produces<SaleReadDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager, UserRole.Cashier))
        .WithOpenApi(op =>
        {
            op.Responses["200"] = new()
            {
                Description = "Sale found",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.SaleReadDtoExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "Sale not found",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.NotFoundExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // POST: create
        app.MapPost("/api/sales", async ([FromServices] ISaleService svc, [FromBody] SaleCreateDto dto, CancellationToken ct) =>
        {
            var result = await svc.CreateAsync(dto, ct);
            return EndpointHelpers.MapResult(result, data => $"/api/sales/{data.Id}");
        })
        .WithName("CreateSale")
        .WithTags("Sales")
        .WithSummary("Create a new sale")
        .WithDescription("Creates a new sale with the specified items. Sale starts in Pending status.")
        .Accepts<SaleCreateDto>("application/json")
        .Produces<SaleReadDto>(StatusCodes.Status201Created)
        .Produces<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager, UserRole.Cashier))
        .WithOpenApi(op =>
        {
            op.RequestBody = new()
            {
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.SaleCreateDtoExample
                }
            };
            op.Responses["201"] = new()
            {
                Description = "Sale created successfully",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.SaleReadDtoExample
                }
            };
            op.Responses["400"] = new()
            {
                Description = "Validation errors",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.ValidationErrorExample
                }
            };
            op.Responses["409"] = new()
            {
                Description = "Conflict (e.g., insufficient stock)",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.ConflictErrorExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // PUT: update
        app.MapPut("/api/sales/{id:int}", async ([FromServices] ISaleService svc, int id, [FromBody] SaleUpdateDto dto, CancellationToken ct) =>
        {
            var result = await svc.UpdateAsync(id, dto, ct);
            return EndpointHelpers.MapResult(result);
        })
        .WithName("UpdateSale")
        .WithTags("Sales")
        .WithSummary("Update an existing sale")
        .WithDescription("Updates an existing sale. Cannot update completed sales.")
        .Accepts<SaleUpdateDto>("application/json")
        .Produces<SaleReadDto>(StatusCodes.Status200OK)
        .Produces<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager, UserRole.Cashier))
        .WithOpenApi(op =>
        {
            op.RequestBody = new()
            {
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.SaleUpdateDtoExample
                }
            };
            op.Responses["200"] = new()
            {
                Description = "Sale updated successfully",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.SaleReadDtoExample2
                }
            };
            op.Responses["400"] = new()
            {
                Description = "Validation errors or business rule violations",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.ValidationErrorExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "Sale not found",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.NotFoundExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // DELETE: delete
        app.MapDelete("/api/sales/{id:int}", async ([FromServices] ISaleService svc, int id, CancellationToken ct) =>
        {
            var result = await svc.DeleteAsync(id, ct);
            return EndpointHelpers.MapResult(result);
        })
        .WithName("DeleteSale")
        .WithTags("Sales")
        .WithSummary("Delete a sale")
        .WithDescription("Deletes a sale. Cannot delete completed sales.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager))
        .WithOpenApi(op =>
        {
            op.Responses["400"] = new()
            {
                Description = "Cannot delete completed sale",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.BadRequestExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "Sale not found",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.NotFoundExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // POST: complete sale
        app.MapPost("/api/sales/{id:int}/complete", async ([FromServices] ISaleService svc, int id, CancellationToken ct) =>
        {
            var result = await svc.CompleteSaleAsync(id, ct);
            return EndpointHelpers.MapResult(result);
        })
        .WithName("CompleteSale")
        .WithTags("Sales")
        .WithSummary("Complete a sale")
        .WithDescription("Completes a pending sale, reducing product stock quantities and marking as completed.")
        .Produces<SaleReadDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager, UserRole.Cashier))
        .WithOpenApi(op =>
        {
            op.Responses["200"] = new()
            {
                Description = "Sale completed successfully",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.SaleReadDtoExample
                }
            };
            op.Responses["400"] = new()
            {
                Description = "Cannot complete sale (e.g., already completed, insufficient stock)",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.BadRequestExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "Sale not found",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.NotFoundExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // POST: cancel sale
        app.MapPost("/api/sales/{id:int}/cancel", async ([FromServices] ISaleService svc, int id, CancellationToken ct) =>
        {
            var result = await svc.CancelSaleAsync(id, ct);
            return EndpointHelpers.MapResult(result);
        })
        .WithName("CancelSale")
        .WithTags("Sales")
        .WithSummary("Cancel a sale")
        .WithDescription("Marks a sale as cancelled.")
        .Produces<SaleReadDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager))
        .WithOpenApi(op =>
        {
            op.Responses["200"] = new()
            {
                Description = "Sale cancelled successfully",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.SaleReadDtoExample2
                }
            };
            op.Responses["400"] = new()
            {
                Description = "Cannot cancel sale (e.g., already completed)",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.BadRequestExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "Sale not found",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.NotFoundExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = SaleEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });
    }
}
