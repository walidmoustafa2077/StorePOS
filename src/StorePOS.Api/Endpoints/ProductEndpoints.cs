using Microsoft.AspNetCore.Mvc;
using StorePOS.Domain.DTOs;
using StorePOS.Domain.Services;
using Microsoft.OpenApi.Models;
using StorePOS.Api.Authorization;
using StorePOS.Domain.Enums;

namespace StorePOS.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        // GET: query list
        app.MapGet("/api/products", async ([FromServices] IProductService svc, [FromQuery] string? q, CancellationToken ct) =>
        {
            var items = await svc.SearchAsync(q, ct);
            return Results.Ok(items);
        })
        .WithName("GetProducts")
        .WithTags("Products")
        .WithSummary("Get all products or search products")
        .WithDescription("Returns a list of products. Optionally filter by name, SKU, or barcode with the 'q' query string.")
        .Produces<List<ProductReadDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager, UserRole.Cashier))
        .WithOpenApi(op =>
        {
            op.Responses["200"] = new()
            {
                Description = "A list of products.",
                Content =
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Example = new Microsoft.OpenApi.Any.OpenApiArray
                        {
                            ProductEndpointDocs.ProductReadDtoExample.Example!,
                            ProductEndpointDocs.ProductReadDtoExample2.Example!
                        }
                    }
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // GET: by barcode
        app.MapGet("/api/products/by-barcode/{barcode}", async ([FromServices] IProductService svc, string barcode, CancellationToken ct) =>
        {
            var dto = await svc.GetByBarcodeAsync(barcode, ct);
            return dto is null ? Results.NotFound() : Results.Ok(dto);
        })
        .WithName("GetProductByBarcode")
        .WithTags("Products")
        .WithSummary("Get product by barcode")
        .WithDescription("Returns a product that matches the given barcode.")
        .Produces<ProductReadDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager, UserRole.Cashier))
        .WithOpenApi(op =>
        {
            op.Responses["200"] = new()
            {
                Description = "A product matching the barcode.",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.ProductReadDtoExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "Product not found.",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.NotFoundExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // POST: create product
        app.MapPost("/api/products", async ([FromServices] IProductService svc, ProductCreateDto dto, CancellationToken ct) =>
        {
            var res = await svc.CreateAsync(dto, ct);
            return EndpointHelpers.MapResult(res, d => $"/api/products/{d.Id}");
        })
        .WithName("CreateProduct")
        .WithTags("Products")
        .WithSummary("Create a new product")
        .WithDescription("Creates a new product and returns the created entity.")
        .Accepts<ProductCreateDto>("application/json")
        .Produces<ProductReadDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status409Conflict, typeof(ConflictErrorExample))
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager))
        .WithOpenApi(op =>
        {
            op.RequestBody = new()
            {
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.ProductCreateDtoExample
                }
            };
            op.Responses["201"] = new()
            {
                Description = "Product created.",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.ProductCreateDtoExample
                }
            };
            op.Responses["409"] = new()
            {
                Description = "Conflict (e.g., SKU or Barcode already exists)",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.ConflictErrorExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // PUT: update product
        app.MapPut("/api/products/{id:int}", async ([FromServices] IProductService svc, int id, ProductUpdateDto dto, CancellationToken ct) =>
        {
            var res = await svc.UpdateAsync(id, dto, ct);
            return EndpointHelpers.MapResult(res);
        })
        .WithName("UpdateProduct")
        .WithTags("Products")
        .WithSummary("Update a product")
        .WithDescription("Updates an existing product by Id.")
        .Produces<ProductReadDto>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager))
        .WithOpenApi(op =>
        {
            op.RequestBody = new()
            {
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.ProductCreateDtoExample
                }
            };
            op.Responses["200"] = new()
            {
                Description = "Product updated.",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.ProductCreateDtoExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "Product not found.",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.NotFoundExample
                }
            };
            op.Responses["409"] = new()
            {
                Description = "Conflict (e.g., SKU or Barcode already exists)",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.ConflictErrorExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // PUT: update stock
        app.MapPut("/api/products/{id:int}/update-stock", async ([FromServices] IProductService svc, int id, ProductStockUpdateDto dto, CancellationToken ct) =>
        {
            var res = await svc.UpdateStockAsync(id, dto, ct);
            return EndpointHelpers.MapResult(res);
        })
        .WithName("UpdateProductStock")
        .WithTags("Products")
        .WithSummary("Update product stock")
        .WithDescription("Updates only the stock quantity of the specified product.")
        .Produces<ProductReadDto>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin, UserRole.Manager))
        .WithOpenApi(op =>
        {
            op.RequestBody = new()
            {
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.ProductStockUpdateDtoExample
                }
            };
            op.Responses["200"] = new()
            {
                Description = "Product stock updated.",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.ProductReadDtoExample
                }
            };
            op.Responses["404"] = new()
            {
                Description = "Product not found.",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.NotFoundExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });

        // DELETE: delete product
        app.MapDelete("/api/products/{id:int}", async ([FromServices] IProductService svc, int id, CancellationToken ct) =>
        {
            var res = await svc.DeleteAsync(id, ct);
            return EndpointHelpers.MapResult(res);
        })
        .WithName("DeleteProduct")
        .WithTags("Products")
        .WithSummary("Delete a product")
        .WithDescription("Deletes a product by its Id. Returns 204 No Content if successful, or 404 if not found.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .WithMetadata(new RequireRoleAttribute(UserRole.Admin))
        .WithOpenApi(op =>
        {
            op.Responses["204"] = new()
            {
                Description = "Product deleted. No content."
            };
            op.Responses["404"] = new()
            {
                Description = "Product not found.",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.NotFoundExample
                }
            };
            op.Responses["401"] = new()
            {
                Description = "Unauthorized",
                Content =
                {
                    ["application/json"] = ProductEndpointDocs.UnauthorizedExample
                }
            };
            return op;
        });
    }
}

public record ConflictErrorExample(string message);
