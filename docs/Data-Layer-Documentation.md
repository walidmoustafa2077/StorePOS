# StorePOS Data Layer Documentation

## Overview

The Data layer in StorePOS implements a comprehensive data access pattern using **Entity Framework Core** with the **Repository Pattern** and **Unit of Work Pattern**. This architecture provides a clean separation between the business logic and data access concerns, offering maintainable, testable, and scalable data operations.

## Architecture Pattern

The data layer follows these key design patterns:

### 1. Repository Pattern
- **Purpose**: Encapsulates data access logic and provides a more object-oriented view of the persistence layer
- **Benefits**: 
  - Centralizes data access logic
  - Provides a substitution point for unit testing
  - Minimizes duplicate query logic
  - Decouples infrastructure from domain model

### 2. Unit of Work Pattern
- **Purpose**: Maintains a list of objects affected by a business transaction and coordinates writing out changes
- **Benefits**:
  - Ensures consistency across multiple repository operations
  - Manages database transactions
  - Reduces database round trips
  - Provides a single point of commit/rollback

### 3. Generic Repository Pattern
- **Purpose**: Provides common CRUD operations for all entities through a base implementation
- **Benefits**:
  - Reduces code duplication
  - Ensures consistent behavior across all repositories
  - Provides type safety through generics

## Core Components

### 1. AppDbContext
**File**: `AppDbContext.cs`

The `AppDbContext` is the heart of the Entity Framework Core implementation. It serves as:

- **Database Session**: Represents a session with the database and can be used to query and save instances of entities
- **Entity Configuration**: Defines entity relationships, constraints, and mappings
- **Change Tracking**: Tracks changes to entities for efficient database updates

```csharp
public class AppDbContext : DbContext
{
    // DbSets represent tables in the database
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleCart> SaleLines => Set<SaleCart>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}
```

#### Key Configurations:

**Product Entity**:
- Unique index on SKU (Stock Keeping Unit)
- Index on Barcode for fast lookups
- Decimal precision for Price and Cost (18,2)
- Required Name field with 200 character limit

**Sale Entity**:
- Decimal precision for all monetary fields (Subtotal, Discount, Tax, Total, PaidAmount)
- One-to-many relationship with SaleCart (Cascade delete)
- 500 character limit for Notes

**User Entity**:
- Unique indexes on Username and Email
- One-to-many relationship with RefreshTokens (Cascade delete)
- Various field length constraints for data integrity

**SaleCart Entity**:
- Decimal precision for UnitPrice and LineTotal
- Foreign key relationship to Product (Restrict delete to maintain data integrity)

### 2. Generic Repository
**Files**: `IGenericRepository.cs`, `GenericRepository.cs`

The generic repository provides a base implementation for common CRUD operations that all entities share.

#### Key Features:

**Query Flexibility**:
```csharp
IQueryable<T> Query(bool asNoTracking = true)
```
- Exposes IQueryable for complex queries
- Optional change tracking control
- Enables API layers to compose includes/projections efficiently

**Optimized Data Retrieval**:
```csharp
Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, 
                            bool asNoTracking = true, 
                            CancellationToken ct = default, 
                            params Expression<Func<T, object>>[] includes)
```
- Expression-based filtering
- Configurable entity includes for related data
- Change tracking control for performance optimization

**Flexible Listing**:
```csharp
Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null,
                       Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                       bool asNoTracking = true,
                       CancellationToken ct = default,
                       params Expression<Func<T, object>>[] includes)
```
- Optional filtering with predicates
- Custom ordering logic
- Related data inclusion
- Performance optimization through no-tracking queries

**Performance Operations**:
- `ExistsAsync`: Efficient existence checking without loading entities
- `CountAsync`: Optimized counting with optional filtering

**CRUD Operations**:
- Async and sync variants for Add, Update, Remove operations
- Bulk operations support (AddRange, RemoveRange)

### 3. Specialized Repositories

#### ProductRepository
**Files**: `IProductRepository.cs`, `ProductRepository.cs`

Extends the generic repository with product-specific functionality:

**Smart Product Search**:
```csharp
Task<Product?> GetAsync(string query, CancellationToken ct = default)
```

This method implements intelligent product lookup with prioritized matching:

1. **Exact Match Priority** (in order):
   - SKU (highest priority)
   - Barcode (medium priority)
   - Name (lower priority)

2. **Partial Match Fallback**:
   - Contains matching across SKU, Barcode, and Name
   - Ordered by Name for consistent results

**Use Cases**:
- Barcode scanning at POS
- Manual product lookup by SKU
- Product search by name
- Quick product identification during sales

#### UserRepository
**Files**: `IUserRepository.cs`, `UserRepository.cs`

Provides user-specific data access patterns:

**Authentication Support**:
- `GetByUsernameAsync`: Fast username-based lookup
- `GetByEmailAsync`: Email-based user retrieval
- `GetByUsernameOrEmailAsync`: Flexible login support with RefreshTokens included

**Security Features**:
- Automatic email normalization (trim + lowercase)
- Username trimming for consistency
- Related RefreshToken loading for authentication flows

#### SaleRepository
**Files**: `ISaleRepository.cs`, `SaleRepository.cs`

Handles complex sale data retrieval:

**Sale with Line Items**:
```csharp
Task<Sale?> GetWithLinesAsync(object id, bool includeProducts = false, CancellationToken ct = default)
```
- Loads sale with all cart items
- Optional product details inclusion
- Optimized for receipt generation and sale details

**Performance Considerations**:
- Uses EF.Property for dynamic ID comparison
- Selective product loading to avoid over-fetching
- AsNoTracking for read-only scenarios

#### Other Repositories
- **CategoryRepository**: Basic category management
- **SaleCartRepository**: Individual cart line management

### 4. Unit of Work
**Files**: `IUnitOfWork.cs`, `UnitOfWork.cs`

The Unit of Work coordinates operations across multiple repositories:

**Repository Access**:
```csharp
public interface IUnitOfWork : IAsyncDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    ISaleRepository Sales { get; }
    ISaleCartRepository SaleCarts { get; }
    IUserRepository Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

**Transaction Management**:
- Single `SaveChangesAsync` method for all changes
- Automatic transaction handling by Entity Framework
- Proper disposal pattern implementation

## Data Flow Architecture

### 1. Service Layer → Unit of Work
```csharp
// Example from a service
public class ProductService
{
    private readonly IUnitOfWork _uow;
    
    public async Task<Product?> GetProductAsync(string query)
    {
        return await _uow.Products.GetAsync(query);
    }
    
    public async Task<bool> CreateProductAsync(Product product)
    {
        await _uow.Products.AddAsync(product);
        var result = await _uow.SaveChangesAsync();
        return result > 0;
    }
}
```

### 2. Repository → Entity Framework
```csharp
// Repository handles EF-specific logic
public async Task<Product?> GetAsync(string query, CancellationToken ct = default)
{
    // Exact match first
    var exact = await _set.AsNoTracking()
                          .Where(p => p.Sku == query || p.Barcode == query || p.Name == query)
                          .OrderByDescending(p => p.Sku == query)
                          .FirstOrDefaultAsync(ct);
    
    if (exact != null) return exact;
    
    // Fallback to partial match
    return await _set.AsNoTracking()
                     .Where(p => p.Sku.Contains(query) || p.Barcode.Contains(query))
                     .FirstOrDefaultAsync(ct);
}
```

### 3. Entity Framework → Database
Entity Framework translates LINQ expressions into optimized SQL queries and manages:
- Connection pooling
- Query caching
- Change tracking
- Lazy loading
- Transaction management

## Performance Optimizations

### 1. AsNoTracking Queries
```csharp
// Default to no-tracking for read operations
IQueryable<T> Query(bool asNoTracking = true)
```
- Reduces memory usage
- Improves query performance
- Prevents accidental entity modifications

### 2. Selective Entity Loading
```csharp
// Load only what's needed
Task<Sale?> GetWithLinesAsync(object id, bool includeProducts = false, CancellationToken ct = default)
```
- Avoids over-fetching data
- Reduces network traffic
- Improves response times

### 3. Expression-Based Filtering
```csharp
// Efficient server-side filtering
Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, ...)
```
- Translates to SQL WHERE clauses
- Reduces data transfer
- Enables complex filtering logic

### 4. Index Optimization
```csharp
// Strategic indexing in AppDbContext
e.HasIndex(x => x.Sku).IsUnique();
e.HasIndex(x => x.Barcode);
e.HasIndex(x => x.Username).IsUnique();
e.HasIndex(x => x.Email).IsUnique();
```
- Fast lookups for common queries
- Ensures data uniqueness
- Optimizes join operations

## Error Handling and Resilience

### 1. Null Safety
```csharp
public async Task<Product?> GetAsync(string query, CancellationToken ct = default)
{
    if (string.IsNullOrWhiteSpace(query))
        return null;  // Early return for invalid input
}
```

### 2. Cancellation Token Support
All async operations support cancellation tokens for:
- Request timeout handling
- Graceful service shutdown
- Resource cleanup

### 3. Connection Management
```csharp
public ValueTask DisposeAsync() => _db.DisposeAsync();
```
- Proper resource disposal
- Connection pool management
- Memory leak prevention

## Testing Considerations

### 1. Interface-Based Design
All repositories implement interfaces, enabling:
- Easy mocking for unit tests
- Dependency injection
- Test double creation

### 2. Separation of Concerns
- Repository logic is isolated from business logic
- Database concerns are abstracted
- Testable without actual database

### 3. Query Composability
```csharp
IQueryable<T> Query(bool asNoTracking = true)
```
Enables testing of complex query logic without database hits.

## Usage Examples

### 1. Simple CRUD Operations
```csharp
// Create
var product = new Product { Name = "Coffee", Price = 5.99m, Sku = "COF001" };
await _uow.Products.AddAsync(product);
await _uow.SaveChangesAsync();

// Read
var foundProduct = await _uow.Products.GetAsync("COF001");

// Update
foundProduct.Price = 6.99m;
_uow.Products.Update(foundProduct);
await _uow.SaveChangesAsync();

// Delete
_uow.Products.Remove(foundProduct);
await _uow.SaveChangesAsync();
```

### 2. Complex Queries
```csharp
// Get active products with category information
var activeProducts = await _uow.Products.ListAsync(
    predicate: p => p.IsActive,
    orderBy: query => query.OrderBy(p => p.Name),
    includes: p => p.Category
);

// Check if product exists without loading it
var exists = await _uow.Products.ExistsAsync(p => p.Sku == "COF001");

// Count products in category
var count = await _uow.Products.CountAsync(p => p.CategoryId == categoryId);
```

### 3. Transaction Management
```csharp
// Multiple operations in single transaction
var sale = new Sale { Total = 25.99m };
await _uow.Sales.AddAsync(sale);

var cartItem = new SaleCart { SaleId = sale.Id, ProductId = productId, Quantity = 2 };
await _uow.SaleCarts.AddAsync(cartItem);

// Both operations succeed or fail together
var result = await _uow.SaveChangesAsync();
```

## Best Practices

### 1. Use AsNoTracking for Read-Only Operations
- Improves performance
- Reduces memory usage
- Prevents accidental modifications

### 2. Leverage Includes Sparingly
- Only include necessary related data
- Consider projection for large datasets
- Use Select() for specific fields

### 3. Implement Proper Disposal
- Always dispose of Unit of Work
- Use using statements or dependency injection
- Implement IAsyncDisposable correctly

### 4. Handle Null Cases
- Check for null inputs
- Return null when appropriate
- Use nullable reference types

### 5. Use CancellationTokens
- Enable request cancellation
- Support graceful shutdowns
- Improve responsiveness

## Migration and Database Schema

The data layer uses Entity Framework Core Code-First approach:

1. **Model Definition**: Entities are defined as C# classes
2. **Configuration**: Relationships and constraints in `OnModelCreating`
3. **Migrations**: Schema changes are tracked and versioned
4. **Database Creation**: Automatic schema generation

This approach ensures:
- Version-controlled database schema
- Consistent deployments across environments
- Easy rollback capabilities
- Automated database updates

## Conclusion

The StorePOS data layer provides a robust, scalable, and maintainable foundation for data access. Through the combination of Repository and Unit of Work patterns with Entity Framework Core, it delivers:

- **Performance**: Optimized queries and efficient data loading
- **Maintainability**: Clean separation of concerns and testable code
- **Flexibility**: Composable queries and extensible repository pattern
- **Reliability**: Transaction management and error handling
- **Scalability**: Connection pooling and resource management

This architecture supports the complex requirements of a Point of Sale system while maintaining code quality and development velocity.
