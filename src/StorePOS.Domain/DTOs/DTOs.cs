namespace StorePOS.Domain.DTOs
{
    // DTOs for read operations
    public record ProductReadDto(
        int Id,
        string Sku,
        string Barcode, 
        string Name, 
        string Category, 
        decimal Price, 
        int StockQty);

    // DTOs for write operations
    public record ProductCreateDto(
        string Sku,
        string Barcode,
        string Name,
        string Category,
        decimal Price,
        decimal Cost,
        int StockQty,
        bool IsActive = true
    );

    public record ProductUpdateDto(
        string Sku,
        string Barcode,
        string Name,
        string Category,
        decimal Price,
        decimal Cost,
        int StockQty,
        bool IsActive = true
    );

    public record ProductStockUpdateDto(int amount, string stockUpdate);

    // DTOs for sales
    public record SaleReadDto(
        int Id,
        DateTimeOffset CreatedAt,
        decimal Subtotal,
        decimal Discount,
        decimal Tax,
        decimal Total,
        decimal PaidAmount,
        string PaymentMethod,
        string? Notes,
        string Status,
        List<SaleCartReadDto> Carts);

    public record SaleCartReadDto(
        int Id,
        int ProductId,
        string ProductName,
        string ProductSku,
        int Qty,
        decimal UnitPrice,
        decimal LineTotal);

    public record SaleCreateDto(
        List<SaleCartCreateDto> Carts,
        decimal Discount = 0,
        decimal Tax = 0,
        decimal PaidAmount = 0,
        string PaymentMethod = "Cash",
        string? Notes = null);

    public record SaleCartCreateDto(int ProductId, int Qty, decimal UnitPrice);

    public record SaleUpdateDto(
        List<SaleCartCreateDto> Carts,
        decimal Discount = 0,
        decimal Tax = 0,
        decimal PaidAmount = 0,
        string PaymentMethod = "Cash",
        string? Notes = null,
        string Status = "Pending");

    // Legacy DTOs for backward compatibility
    public record CreateSaleDto(
        List<CreateSaleCartDto> Carts,
        decimal Discount = 0,
        decimal Tax = 0,
        decimal PaidAmount = 0,
        string PaymentMethod = "Cash");

    public record CreateSaleCartDto(int ProductId, int Qty, decimal UnitPrice);

    // DTOs for users
    public record UserReadDto(
        int Id,
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string? PhoneNumber,
        string Role,
        bool IsActive,
        DateTimeOffset CreatedAt,
        DateTimeOffset? LastLoginAt);

    public record UserCreateDto(
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string Password,
        string? PhoneNumber = null,
        string Role = "Cashier",
        bool IsActive = true);

    public record UserUpdateDto(
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string? PhoneNumber = null,
        string Role = "Cashier",
        bool IsActive = true);

    public record UserChangePasswordDto(
        string CurrentPassword,
        string NewPassword);

    public record UserLoginDto(
        string UsernameOrEmail,
        string Password);

    // Authentication DTOs
    public record AuthTokenDto(
        string AccessToken,
        string RefreshToken,
        DateTimeOffset ExpiresAt,
        UserReadDto User);

    public record RefreshTokenDto(
        string RefreshToken);

    public record AuthResponseDto(
        bool Success,
        string? Message,
        AuthTokenDto? Data);
}
