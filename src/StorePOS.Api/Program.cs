
using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Extensions;
using StorePOS.Api.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StorePOS.Domain.Models;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TechStorePOS"));

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings?.Issuer,
            ValidAudience = jwtSettings?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings?.Secret ?? throw new InvalidOperationException("JWT Secret not configured"))),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddStorePOSRepositories();
builder.Services.AddStorePOSServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Serve static files from docs directory
var docsPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "docs"));
if (Directory.Exists(docsPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(docsPath),
        RequestPath = "/docs",
        ContentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider(new Dictionary<string, string>
        {
            { ".ps1", "text/plain" },
            { ".cs", "text/plain" },
            { ".py", "text/plain" },
            { ".js", "text/javascript" },
            { ".html", "text/html" },
            { ".css", "text/css" },
            { ".json", "application/json" },
            { ".md", "text/markdown" }
        })
    });
}
else
{
    Console.WriteLine($"Docs directory not found at: {docsPath}");
}

app.UseHttpsRedirection();

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();


// Seed once per process
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var accessories = await db.Categories
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Name == "Accessories");

    if (accessories is null)
    {
        accessories = new StorePOS.Domain.Models.Category { Name = "Accessories" };
        db.Categories.Add(accessories);
        await db.SaveChangesAsync();
    }
    else
    {
        db.Attach(accessories);
    }

    if (!await db.Products.AnyAsync())
    {
        db.Products.AddRange(
            new Product { Sku = "KB-001", Barcode = "622000000001", Name = "Mechanical Keyboard", CategoryId = accessories.Id, Price = 1800m, Cost = 1200m, StockQty = 10, IsActive = true },
            new Product { Sku = "MS-010", Barcode = "622000000010", Name = "Gaming Mouse", CategoryId = accessories.Id, Price = 950m, Cost = 600m, StockQty = 25, IsActive = true }
        );
        await db.SaveChangesAsync();
    }

    // Seed admin user if no users exist
    if (!await db.Users.AnyAsync())
    {
        var adminUser = new StorePOS.Domain.Models.User
        {
            Username = "admin",
            Email = "admin@storepos.com",
            FirstName = "System",
            LastName = "Administrator",
            PasswordHash = StorePOS.Domain.Helpers.PasswordHelper.HashPassword("admin123"),
            PhoneNumber = "+1234567890",
            Role = Enum.Parse<StorePOS.Domain.Enums.UserRole>("Admin"),
            IsActive = true,
            CreatedAt = DateTimeOffset.Now
        };

        db.Users.Add(adminUser);
        await db.SaveChangesAsync();
    }
}


// Register endpoints
app.MapAuthEndpoints();
app.MapProductEndpoints();
app.MapSaleEndpoints();
app.MapUserEndpoints();

// Add a redirect from root docs to index.html
app.MapGet("/docs", () => Results.Redirect("/docs/index.html"));

app.Run();