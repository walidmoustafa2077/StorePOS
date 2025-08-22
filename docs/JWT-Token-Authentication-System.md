# StorePOS JWT Token Authentication System Documentation

## Overview

The StorePOS application implements a sophisticated **JWT (JSON Web Token) authentication system** with **refresh token rotation** designed for Point-of-Sale operations. This system provides secure, scalable authentication while maintaining excellent user experience for retail environments through a dual-token approach that balances security with usability.

## Authentication Architecture

The authentication system follows these key design patterns:

### 1. Dual-Token Pattern
- **Purpose**: Balances security with user experience through complementary token types
- **Benefits**: 
  - Short-lived access tokens minimize security exposure
  - Long-lived refresh tokens reduce authentication friction
  - Token rotation prevents replay attacks
  - Immediate revocation capabilities for security incidents

### 2. Token Rotation Pattern
- **Purpose**: Ensures refresh tokens are single-use to prevent replay attacks
- **Benefits**:
  - Eliminates token reuse vulnerabilities
  - Provides audit trail for token usage
  - Enables detection of token theft
  - Maintains security without user inconvenience

### 3. Stateless Authentication Pattern
- **Purpose**: Provides scalable authentication without server-side session storage
- **Benefits**:
  - Improves application scalability
  - Reduces server memory usage
  - Enables horizontal scaling
  - Simplifies load balancing

## Core Components

### 1. TokenService
**File**: `TokenService.cs`

The `TokenService` is the heart of the JWT authentication implementation. It serves as:

- **Token Generator**: Creates secure JWT access tokens and cryptographically secure refresh tokens
- **Token Manager**: Handles token rotation, revocation, and lifecycle management
- **Security Controller**: Implements audit trails and automatic cleanup of expired tokens

```csharp
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUnitOfWork _unitOfWork;

    // Core token operations
    public string GenerateAccessToken(User user)
    public string GenerateRefreshToken()
    public Task<AuthTokenDto?> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
    public Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null)
    public Task<bool> RevokeAllTokensAsync(int userId)
}
```

#### Key Configurations:

**JWT Settings**:
- Secret key for HMAC SHA-256 signing
- Issuer and Audience for token validation
- Access token expiration (15 minutes default)
- Refresh token expiration (7 days default)

**Security Features**:
- Cryptographically secure random generation for refresh tokens
- IP address tracking for audit trails
- Automatic token cleanup to prevent database bloat
- Token rotation with replacement tracking

### 2. Access Token (JWT)
**Purpose**: Short-lived authorization token containing user claims

The access token provides stateless authentication with embedded user information:

#### Key Features:

**Claim Structure**:
```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, user.Role.ToString())
};
```
- Contains essential user identity information
- Includes role-based authorization data
- Signed with HMAC SHA-256 for tamper protection
- 15-minute lifespan to minimize exposure window

**Security Characteristics**:
- **Stateless**: No server-side storage required
- **Tamper-proof**: Cryptographically signed
- **Self-contained**: Carries all necessary authorization data
- **Time-limited**: Short expiration reduces attack window

### 3. Refresh Token
**Files**: `RefreshToken.cs`, Database Table

Provides long-term authentication capability with security features:

**Entity Structure**:
```csharp
public class RefreshToken
{
    public string Token { get; set; }              // 64-byte random string
    public DateTimeOffset ExpiresAt { get; set; }  // 7 days from creation
    public DateTimeOffset CreatedAt { get; set; }  // Creation timestamp
    public string? CreatedByIp { get; set; }       // Audit trail
    public DateTimeOffset? RevokedAt { get; set; }  // Revocation tracking
    public string? RevokedByIp { get; set; }       // Revocation audit
    public string? ReplaceByToken { get; set; }    // Rotation tracking
    public bool IsActive => RevokedAt == null && !IsExpired;
}
```

**Security Features**:
- **Cryptographic Security**: 64-byte random generation using cryptographically secure RNG
- **Single Use**: Token rotation ensures each token is used only once
- **Audit Trail**: Complete tracking of creation, usage, and revocation
- **Immediate Revocation**: Database storage enables instant invalidation

### 4. AuthService Integration
**Files**: `AuthService.cs`, `IAuthService.cs`

The AuthService coordinates authentication operations using TokenService:

**Authentication Operations**:
- `LoginAsync`: Validates credentials and generates initial token pair
- `RefreshTokenAsync`: Rotates tokens for continued authentication
- `LogoutAsync`: Revokes refresh token for secure logout
- `RevokeAllTokensAsync`: Emergency revocation for security incidents

**Security Integration**:
- Password verification using secure hashing
- IP address tracking for audit purposes
- Old token cleanup during operations
- User account status validation

## Data Flow Architecture

### 1. Login Process Flow
```csharp
// AuthService coordinates the login process
public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, string? ipAddress = null)
{
    // 1. Validate user credentials
    var user = await _unitOfWork.Users.GetByUsernameOrEmailAsync(loginDto.Username);
    if (!PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
        return new AuthResponseDto(false, "Invalid credentials", null);

    // 2. Generate token pair
    var accessToken = _tokenService.GenerateAccessToken(user);
    var refreshToken = CreateRefreshToken(ipAddress);
    
    // 3. Store refresh token and cleanup old ones
    user.RefreshTokens.Add(refreshToken);
    RemoveOldRefreshTokens(user);
    
    // 4. Update user and save changes
    await _unitOfWork.Users.UpdateAsync(user);
    await _unitOfWork.SaveChangesAsync();
    
    // 5. Return authentication result
    return new AuthResponseDto(true, "Login successful", 
        new AuthTokenDto(accessToken, refreshToken.Token, refreshToken.ExpiresAt, user.ToReadDto()));
}
```

### 2. Token Refresh Flow
```csharp
// TokenService handles secure token rotation
public async Task<AuthTokenDto?> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
{
    // 1. Validate refresh token exists and is active
    var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken);
    var token = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
    if (token == null || !token.IsActive) return null;

    // 2. Generate new tokens (rotation pattern)
    var newRefreshToken = RotateRefreshToken(token, ipAddress);
    user.RefreshTokens.Add(newRefreshToken);
    user.LastLoginAt = DateTimeOffset.UtcNow;

    // 3. Clean up old tokens and save changes
    RemoveOldRefreshTokens(user);
    await _unitOfWork.Users.UpdateAsync(user);
    await _unitOfWork.SaveChangesAsync();

    // 4. Generate new access token and return
    var accessToken = GenerateAccessToken(user);
    return new AuthTokenDto(accessToken, newRefreshToken.Token, newRefreshToken.ExpiresAt, user.ToReadDto());
}
```

### 3. Token Revocation Flow
```csharp
// Secure token revocation with audit trail
public async Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null)
{
    // 1. Find user and token
    var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken);
    var token = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
    if (token == null || !token.IsActive) return false;

    // 2. Revoke token with audit information
    RevokeRefreshToken(token, ipAddress);
    
    // 3. Save changes
    await _unitOfWork.Users.UpdateAsync(user);
    await _unitOfWork.SaveChangesAsync();
    
    return true;
}
```

## Security Implementation Details

### 1. Access Token Generation
The access token generation implements JWT best practices:

**Token Creation Process**:
```csharp
public string GenerateAccessToken(User user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

    // Build claims with user information
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
        new Claim("IsActive", user.IsActive.ToString())
    };

    // Configure token with security settings
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
        Issuer = _jwtSettings.Issuer,
        Audience = _jwtSettings.Audience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
}
```

**Security Features**:
- **Strong Signing**: HMAC SHA-256 with 256-bit minimum key
- **Time Bounds**: Short expiration window (15 minutes)
- **Essential Claims**: Minimal necessary user information
- **Standard Compliance**: RFC 7519 JWT specification

### 2. Refresh Token Security
Refresh tokens implement maximum security through multiple mechanisms:

**Secure Generation**:
```csharp
public string GenerateRefreshToken()
{
    var randomNumber = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
}
```

**Token Rotation Implementation**:
```csharp
private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string? ipAddress)
{
    var newRefreshToken = CreateRefreshToken(ipAddress);
    RevokeRefreshToken(refreshToken, ipAddress, newRefreshToken.Token);
    return newRefreshToken;
}
```

**Security Benefits**:
- **Cryptographic Strength**: 64 bytes of secure random data
- **Single Use**: Each token valid for one refresh operation only
- **Audit Trail**: Complete tracking of token lifecycle
- **Immediate Revocation**: Database storage enables instant invalidation

### 3. Database Security Schema
The refresh token storage implements comprehensive security:

**Entity Relationships**:
```csharp
// User entity configuration in AppDbContext
b.Entity<User>(e =>
{
    e.HasMany(x => x.RefreshTokens)
     .WithOne(x => x.User)
     .HasForeignKey(x => x.UserId)
     .OnDelete(DeleteBehavior.Cascade);
});

// RefreshToken entity configuration
b.Entity<RefreshToken>(e =>
{
    e.Property(x => x.Token).HasMaxLength(255).IsRequired();
    e.Property(x => x.CreatedByIp).HasMaxLength(45);
    e.Property(x => x.RevokedByIp).HasMaxLength(45);
    e.Property(x => x.ReplaceByToken).HasMaxLength(255);
});
```

**Performance Optimizations**:
- Indexes on Token field for fast lookups
- Cascade delete for automatic cleanup
- IP address fields sized for IPv6 compatibility
- Optimal string lengths for Base64 tokens

## Performance Optimizations

### 1. Database Efficiency
The token system implements several performance optimizations:

**Query Optimization**:
```csharp
// Efficient token lookup with user relationship
public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
{
    return await _set
        .Include(u => u.RefreshTokens)  // Single query with join
        .Where(u => u.RefreshTokens.Any(t => t.Token == refreshToken))
        .FirstOrDefaultAsync(ct);
}
```

**Automatic Cleanup**:
```csharp
private void RemoveOldRefreshTokens(User user)
{
    var refreshTokenTTL = _jwtSettings.RefreshTokenExpirationDays;
    
    // Remove only inactive tokens older than TTL
    user.RefreshTokens.RemoveAll(x =>
        !x.IsActive &&
        x.CreatedAt.AddDays(refreshTokenTTL) <= DateTimeOffset.UtcNow);
}
```

### 2. Memory Management
- **Stateless JWTs**: No server-side storage for access tokens
- **Efficient Claims**: Minimal claim set reduces token size
- **Automatic Disposal**: Proper resource cleanup in TokenService

### 3. Network Efficiency
- **Compact Tokens**: Base64 encoding minimizes transfer size
- **Single Requests**: Refresh operations return both new tokens
- **Compression Ready**: JSON structure works well with HTTP compression

## Real-World Usage Scenarios

### 1. Normal POS Operation
```csharp
// Typical daily workflow
// 1. Employee login at start of shift
var loginResult = await _authService.LoginAsync(new LoginDto("cashier01", "password"), clientIp);

// 2. Automatic token refresh every 15 minutes (transparent to user)
var refreshResult = await _authService.RefreshTokenAsync(refreshToken, clientIp);

// 3. Logout at end of shift
var logoutResult = await _authService.LogoutAsync(refreshToken, clientIp);
```

### 2. Security Incident Response
```csharp
// Emergency revocation of all user tokens
await _tokenService.RevokeAllTokensAsync(compromisedUserId);

// User must re-authenticate on all devices
// All existing sessions immediately invalidated
```

### 3. Multi-Device POS Environment
```csharp
// Each device maintains independent token pairs
// Tokens can be revoked per device or all at once
// Central audit trail tracks all authentication events across devices
```

## Configuration and Settings

### 1. JWT Settings Configuration
**File**: `appsettings.json`

```json
{
  "JwtSettings": {
    "Secret": "your-super-secret-256-bit-key-goes-here",
    "Issuer": "StorePOS",
    "Audience": "StorePOS-API",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

**Environment-Specific Settings**:
- **Development**: Longer token lifespans for convenience
- **Production**: Shorter lifespans for maximum security
- **Testing**: Configurable for automated test scenarios

### 2. Dependency Injection Setup
**File**: `ServiceCollectionExtensions.cs`

```csharp
// Token service registration
services.AddScoped<ITokenService, TokenService>();

// JWT settings configuration
services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

// JWT authentication middleware
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret))
        };
    });
```

## Error Handling and Validation

### 1. Token Validation Errors
The system handles various token-related error scenarios:

**Common Error Cases**:
- **Expired Access Token**: Automatic refresh attempt
- **Invalid Refresh Token**: Redirect to login
- **Revoked Token**: Immediate authentication failure
- **Malformed Token**: Security logging and rejection

**Error Response Pattern**:
```csharp
public record AuthResponseDto(
    bool Success,
    string? Message,
    AuthTokenDto? Data);
```

### 2. Security Event Logging
All authentication events are logged for security monitoring:

```csharp
// Login events
_logger.LogInformation("User {UserId} logged in from IP {IpAddress}", user.Id, ipAddress);

// Token refresh events
_logger.LogInformation("Token refreshed for user {UserId}", user.Id);

// Security events
_logger.LogWarning("Failed login attempt for {Username} from IP {IpAddress}", username, ipAddress);
```

## Best Practices Implementation

### 1. Security Best Practices
- **Short Access Token Lifespan**: 15-minute expiration minimizes exposure
- **Strong Refresh Token Generation**: Cryptographically secure random generation
- **Token Rotation**: Single-use refresh tokens prevent replay attacks
- **Comprehensive Audit Trail**: Full tracking of all token operations
- **Immediate Revocation**: Database storage enables instant token invalidation

### 2. Performance Best Practices
- **Stateless Architecture**: JWT tokens require no server-side storage
- **Efficient Database Queries**: Optimized token lookup with proper indexing
- **Automatic Cleanup**: Prevents database bloat from accumulated tokens
- **Minimal Claims**: Only essential user information in access tokens

### 3. Operational Best Practices
- **Environment-Specific Configuration**: Different settings for dev/prod
- **Comprehensive Logging**: Security event tracking and monitoring
- **Graceful Error Handling**: User-friendly error messages
- **Emergency Procedures**: Rapid response capabilities for security incidents

This JWT authentication system provides enterprise-grade security while maintaining excellent user experience for StorePOS operations. The implementation balances security requirements with operational needs, ensuring both protection against attacks and seamless daily operations.
