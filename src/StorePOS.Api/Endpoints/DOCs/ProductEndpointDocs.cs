using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace StorePOS.Api.Endpoints;

public static class ProductEndpointDocs
{
    public static OpenApiMediaType ProductReadDtoExample => new()
    {
        Example = new OpenApiObject
        {
            ["sku"] = new OpenApiString("KB-001"),
            ["barcode"] = new OpenApiString("622000000001"),
            ["name"] = new OpenApiString("Mechanical Keyboard"),
            ["category"] = new OpenApiString("Accessories"),
            ["price"] = new OpenApiDouble(1800),
            ["cost"] = new OpenApiDouble(1200),
            ["stockQty"] = new OpenApiInteger(10),
            ["isActive"] = new OpenApiBoolean(true)
        }
    };

    public static OpenApiMediaType ProductReadDtoExample2 => new()
    {
        Example = new OpenApiObject
        {
            ["sku"] = new OpenApiString("MS-010"),
            ["barcode"] = new OpenApiString("622000000010"),
            ["name"] = new OpenApiString("Gaming Mouse"),
            ["category"] = new OpenApiString("Accessories"),
            ["price"] = new OpenApiDouble(950),
            ["cost"] = new OpenApiDouble(600),
            ["stockQty"] = new OpenApiInteger(25),
            ["isActive"] = new OpenApiBoolean(true)
        }
    };

    public static OpenApiMediaType ProductCreateDtoExample => new()
    {
        Example = new OpenApiObject
        {
            ["sku"] = new OpenApiString("KB-009"),
            ["barcode"] = new OpenApiString("622000000099"),
            ["name"] = new OpenApiString("Wireless Keyboard"),
            ["category"] = new OpenApiString("Accessories"),
            ["price"] = new OpenApiDouble(1200),
            ["cost"] = new OpenApiDouble(800),
            ["stockQty"] = new OpenApiInteger(20),
            ["isActive"] = new OpenApiBoolean(true)
        }
    };

    public static OpenApiMediaType ProductStockUpdateDtoExample => new()
    {
        Example = new OpenApiObject
        {
            ["amount"] = new OpenApiInteger(2),
            ["stockUpdate"] = new OpenApiString("Decrease")
        }
    };

    public static OpenApiMediaType ConflictErrorExample => new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("SKU already exists.")
        }
    };

    public static OpenApiMediaType NotFoundExample => new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Not found")
        }
    };

    public static OpenApiMediaType UnauthorizedExample => new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Unauthorized")
        }
    };
}
