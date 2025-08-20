using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace StorePOS.Api.Endpoints;

public static class SaleEndpointDocs
{
    public static OpenApiMediaType SaleReadDtoExample => new()
    {
        Example = new OpenApiObject
        {
            ["id"] = new OpenApiInteger(1),
            ["createdAt"] = new OpenApiString("2025-08-20T10:30:00.000Z"),
            ["subtotal"] = new OpenApiDouble(2750.00),
            ["discount"] = new OpenApiDouble(50.00),
            ["tax"] = new OpenApiDouble(270.00),
            ["total"] = new OpenApiDouble(2970.00),
            ["paidAmount"] = new OpenApiDouble(3000.00),
            ["paymentMethod"] = new OpenApiString("Cash"),
            ["notes"] = new OpenApiString("Customer requested receipt"),
            ["status"] = new OpenApiString("Completed"),
            ["carts"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["productId"] = new OpenApiInteger(1),
                    ["productName"] = new OpenApiString("Mechanical Keyboard"),
                    ["productSku"] = new OpenApiString("KB-001"),
                    ["qty"] = new OpenApiInteger(1),
                    ["unitPrice"] = new OpenApiDouble(1800.00),
                    ["lineTotal"] = new OpenApiDouble(1800.00)
                },
                new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(2),
                    ["productId"] = new OpenApiInteger(2),
                    ["productName"] = new OpenApiString("Gaming Mouse"),
                    ["productSku"] = new OpenApiString("MS-010"),
                    ["qty"] = new OpenApiInteger(1),
                    ["unitPrice"] = new OpenApiDouble(950.00),
                    ["lineTotal"] = new OpenApiDouble(950.00)
                }
            }
        }
    };

    public static OpenApiMediaType SaleReadDtoExample2 => new()
    {
        Example = new OpenApiObject
        {
            ["id"] = new OpenApiInteger(2),
            ["createdAt"] = new OpenApiString("2025-08-20T14:15:00.000Z"),
            ["subtotal"] = new OpenApiDouble(1200.00),
            ["discount"] = new OpenApiDouble(0.00),
            ["tax"] = new OpenApiDouble(120.00),
            ["total"] = new OpenApiDouble(1320.00),
            ["paidAmount"] = new OpenApiDouble(0.00),
            ["paymentMethod"] = new OpenApiString("Credit"),
            ["notes"] = new OpenApiString("Pending payment"),
            ["status"] = new OpenApiString("Pending"),
            ["carts"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(3),
                    ["productId"] = new OpenApiInteger(3),
                    ["productName"] = new OpenApiString("Wireless Keyboard"),
                    ["productSku"] = new OpenApiString("KB-009"),
                    ["qty"] = new OpenApiInteger(1),
                    ["unitPrice"] = new OpenApiDouble(1200.00),
                    ["lineTotal"] = new OpenApiDouble(1200.00)
                }
            }
        }
    };

    public static OpenApiMediaType SaleCreateDtoExample => new()
    {
        Example = new OpenApiObject
        {
            ["carts"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["productId"] = new OpenApiInteger(1),
                    ["qty"] = new OpenApiInteger(2),
                    ["unitPrice"] = new OpenApiDouble(1800.00)
                },
                new OpenApiObject
                {
                    ["productId"] = new OpenApiInteger(2),
                    ["qty"] = new OpenApiInteger(1),
                    ["unitPrice"] = new OpenApiDouble(950.00)
                }
            },
            ["discount"] = new OpenApiDouble(100.00),
            ["tax"] = new OpenApiDouble(455.00),
            ["paidAmount"] = new OpenApiDouble(5000.00),
            ["paymentMethod"] = new OpenApiString("Cash"),
            ["notes"] = new OpenApiString("Bulk purchase discount applied")
        }
    };

    public static OpenApiMediaType SaleUpdateDtoExample => new()
    {
        Example = new OpenApiObject
        {
            ["carts"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["productId"] = new OpenApiInteger(1),
                    ["qty"] = new OpenApiInteger(1),
                    ["unitPrice"] = new OpenApiDouble(1800.00)
                }
            },
            ["discount"] = new OpenApiDouble(50.00),
            ["tax"] = new OpenApiDouble(175.00),
            ["paidAmount"] = new OpenApiDouble(2000.00),
            ["paymentMethod"] = new OpenApiString("Credit"),
            ["notes"] = new OpenApiString("Updated payment method"),
            ["status"] = new OpenApiString("Pending")
        }
    };

    public static OpenApiMediaType SaleCartCreateDtoExample => new()
    {
        Example = new OpenApiObject
        {
            ["productId"] = new OpenApiInteger(1),
            ["qty"] = new OpenApiInteger(2),
            ["unitPrice"] = new OpenApiDouble(1800.00)
        }
    };

    public static OpenApiMediaType SaleListExample => new()
    {
        Example = new OpenApiArray
        {
            new OpenApiObject
            {
                ["id"] = new OpenApiInteger(1),
                ["createdAt"] = new OpenApiString("2025-08-20T10:30:00.000Z"),
                ["subtotal"] = new OpenApiDouble(2750.00),
                ["discount"] = new OpenApiDouble(50.00),
                ["tax"] = new OpenApiDouble(270.00),
                ["total"] = new OpenApiDouble(2970.00),
                ["paidAmount"] = new OpenApiDouble(3000.00),
                ["paymentMethod"] = new OpenApiString("Cash"),
                ["notes"] = new OpenApiString("Customer requested receipt"),
                ["status"] = new OpenApiString("Completed"),
                ["carts"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(1),
                        ["productId"] = new OpenApiInteger(1),
                        ["productName"] = new OpenApiString("Mechanical Keyboard"),
                        ["productSku"] = new OpenApiString("KB-001"),
                        ["qty"] = new OpenApiInteger(1),
                        ["unitPrice"] = new OpenApiDouble(1800.00),
                        ["lineTotal"] = new OpenApiDouble(1800.00)
                    }
                }
            },
            new OpenApiObject
            {
                ["id"] = new OpenApiInteger(2),
                ["createdAt"] = new OpenApiString("2025-08-20T14:15:00.000Z"),
                ["subtotal"] = new OpenApiDouble(1200.00),
                ["discount"] = new OpenApiDouble(0.00),
                ["tax"] = new OpenApiDouble(120.00),
                ["total"] = new OpenApiDouble(1320.00),
                ["paidAmount"] = new OpenApiDouble(0.00),
                ["paymentMethod"] = new OpenApiString("Credit"),
                ["notes"] = new OpenApiString("Pending payment"),
                ["status"] = new OpenApiString("Pending"),
                ["carts"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(3),
                        ["productId"] = new OpenApiInteger(3),
                        ["productName"] = new OpenApiString("Wireless Keyboard"),
                        ["productSku"] = new OpenApiString("KB-009"),
                        ["qty"] = new OpenApiInteger(1),
                        ["unitPrice"] = new OpenApiDouble(1200.00),
                        ["lineTotal"] = new OpenApiDouble(1200.00)
                    }
                }
            }
        }
    };

    public static OpenApiMediaType ValidationErrorExample => new()
    {
        Example = new OpenApiObject
        {
            ["carts"] = new OpenApiArray
            {
                new OpenApiString("At least one cart item is required.")
            },
            ["paymentMethod"] = new OpenApiArray
            {
                new OpenApiString("Payment method must be 'Cash', 'Credit', or 'Debit'.")
            },
            ["status"] = new OpenApiArray
            {
                new OpenApiString("Status must be 'Pending', 'Completed', or 'Cancelled'.")
            }
        }
    };

    public static OpenApiMediaType ConflictErrorExample => new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Cannot update a completed sale.")
        }
    };

    public static OpenApiMediaType NotFoundExample => new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Sale not found.")
        }
    };

    public static OpenApiMediaType BadRequestExample => new()
    {
        Example = new OpenApiObject
        {
            ["message"] = new OpenApiString("Cannot complete sale: insufficient stock for product KB-001.")
        }
    };

    public static OpenApiMediaType PaymentMethodsExample => new()
    {
        Example = new OpenApiObject
        {
            ["availablePaymentMethods"] = new OpenApiArray
            {
                new OpenApiString("Cash"),
                new OpenApiString("Credit"),
                new OpenApiString("Debit")
            }
        }
    };

    public static OpenApiMediaType StatusOptionsExample => new()
    {
        Example = new OpenApiObject
        {
            ["availableStatuses"] = new OpenApiArray
            {
                new OpenApiString("Pending"),
                new OpenApiString("Completed"),
                new OpenApiString("Cancelled")
            }
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
