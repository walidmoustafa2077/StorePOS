using StorePOS.Domain.DTOs;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Services
{
    /// <summary>
    /// Defines the contract for JWT token management services in the StorePOS authentication system.
    /// Provides secure token generation, refresh token rotation, and comprehensive token lifecycle management
    /// for Point-of-Sale operations with enterprise-grade security features.
    /// </summary>
    /// <remarks>
    /// This interface defines the core token operations required for secure authentication:
    /// - JWT access token generation with user claims
    /// - Cryptographically secure refresh token creation
    /// - Token rotation pattern for maximum security
    /// - Granular token revocation capabilities
    /// - Audit trail support for compliance and security monitoring
    /// 
    /// The implementation ensures:
    /// - Stateless authentication through JWT tokens
    /// - Secure refresh token rotation to prevent replay attacks
    /// - Comprehensive audit logging for security events
    /// - Scalable token management for multi-device POS environments
    /// </remarks>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT access token containing user claims and authorization information.
        /// </summary>
        /// <param name="user">The user entity containing identity and role information to encode in the token</param>
        /// <returns>A base64-encoded JWT access token string signed with HMAC SHA-256</returns>
        /// <remarks>
        /// Creates a short-lived JWT token (default 15 minutes) containing essential user claims:
        /// - User ID for identity verification
        /// - Username, email, first name, last name for personalization
        /// - User role for authorization decisions
        /// - Account active status for security validation
        /// 
        /// The token is cryptographically signed to prevent tampering and includes
        /// standard JWT claims (issuer, audience, expiration) for proper validation.
        /// 
        /// Security considerations:
        /// - Short lifespan minimizes exposure window
        /// - Stateless design enables horizontal scaling
        /// - HMAC SHA-256 signature prevents tampering
        /// - Minimal claim set reduces information leakage
        /// </remarks>
        string GenerateAccessToken(User user);

        /// <summary>
        /// Generates a cryptographically secure random refresh token for long-term authentication.
        /// </summary>
        /// <returns>A base64-encoded random string (88 characters) suitable for secure refresh token use</returns>
        /// <remarks>
        /// Creates a refresh token using 64 bytes of cryptographically secure random data.
        /// This token is used for:
        /// - Generating new access tokens when they expire
        /// - Maintaining user sessions without frequent re-authentication
        /// - Providing revocation capabilities for security incidents
        /// 
        /// Security features:
        /// - Cryptographically secure random generation (System.Security.Cryptography.RandomNumberGenerator)
        /// - 64-byte entropy provides sufficient security against brute force attacks
        /// - Base64 encoding for safe transport and storage
        /// - Single-use design through token rotation pattern
        /// 
        /// The generated token must be stored securely and associated with the user
        /// in the database for validation during refresh operations.
        /// </remarks>
        string GenerateRefreshToken();

        /// <summary>
        /// Refreshes an access token using a valid refresh token, implementing secure token rotation.
        /// </summary>
        /// <param name="refreshToken">The current refresh token to validate and exchange for new tokens</param>
        /// <param name="ipAddress">Optional IP address for audit trail and security monitoring purposes</param>
        /// <param name="cancellationToken">Cancellation token for async operation control and timeout handling</param>
        /// <returns>
        /// A new <see cref="AuthTokenDto"/> containing fresh access and refresh tokens with user information,
        /// or null if the provided refresh token is invalid, expired, or revoked
        /// </returns>
        /// <remarks>
        /// This method implements the secure token rotation pattern by:
        /// 1. Validating the provided refresh token exists and is active
        /// 2. Revoking the old refresh token immediately (single-use principle)
        /// 3. Generating a new cryptographically secure refresh token
        /// 4. Creating a fresh access token with current user information
        /// 5. Updating the user's last login timestamp
        /// 6. Cleaning up expired refresh tokens to prevent database bloat
        /// 7. Persisting all changes in a single database transaction
        /// 
        /// Security benefits:
        /// - Token rotation prevents replay attacks
        /// - Immediate revocation of old tokens
        /// - Audit trail with IP address tracking
        /// - Automatic cleanup of expired tokens
        /// - Transactional consistency for all operations
        /// 
        /// This method should be called automatically by client applications
        /// when access tokens expire (HTTP 401 responses) to maintain
        /// seamless user experience without manual re-authentication.
        /// </remarks>
        Task<AuthTokenDto?> RefreshTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a specific refresh token, preventing its future use for token refresh operations.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke and mark as inactive</param>
        /// <param name="ipAddress">Optional IP address for audit trail purposes and security monitoring</param>
        /// <param name="cancellationToken">Cancellation token for async operation control and timeout handling</param>
        /// <returns>
        /// True if the token was successfully found and revoked, 
        /// false if the token was not found or was already inactive
        /// </returns>
        /// <remarks>
        /// This method is used for secure logout operations and security incident response.
        /// It immediately invalidates the specified refresh token by:
        /// 1. Locating the token in the user's refresh token collection
        /// 2. Marking it as revoked with timestamp and IP address
        /// 3. Persisting the revocation to the database
        /// 
        /// Use cases:
        /// - User logout from a specific device or session
        /// - Security incident response for compromised tokens
        /// - Administrative token revocation
        /// - Cleanup of suspicious authentication activity
        /// 
        /// The revoked token maintains audit information including:
        /// - Revocation timestamp for forensic analysis
        /// - IP address of revocation request for security monitoring
        /// - Original creation details for complete audit trail
        /// 
        /// Once revoked, the token cannot be used for any authentication
        /// operations and will be cleaned up during regular maintenance.
        /// </remarks>
        Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes all active refresh tokens for a specific user, forcing re-authentication on all devices.
        /// </summary>
        /// <param name="userId">The ID of the user whose tokens should be revoked</param>
        /// <param name="cancellationToken">Cancellation token for async operation control and timeout handling</param>
        /// <returns>
        /// True if the user was found and their tokens were successfully revoked,
        /// false if the user was not found
        /// </returns>
        /// <remarks>
        /// This method provides emergency security capabilities by revoking all active
        /// refresh tokens for a user simultaneously. It is used for:
        /// 
        /// Security scenarios:
        /// - Password changes (invalidate all existing sessions)
        /// - Account compromise detection (immediate session termination)
        /// - Administrative security actions
        /// - Suspicious activity response
        /// - Employee termination or role changes
        /// 
        /// The operation:
        /// 1. Retrieves the user with all their refresh tokens
        /// 2. Marks all active tokens as revoked with current timestamp
        /// 3. Persists all changes in a single database transaction
        /// 4. Maintains audit trail for all revoked tokens
        /// 
        /// Impact:
        /// - User must re-authenticate on ALL devices and applications
        /// - All existing sessions become invalid immediately
        /// - Mobile apps, POS terminals, and web sessions require new login
        /// - Provides immediate security response capability
        /// 
        /// This is a powerful security tool that should be used judiciously
        /// as it will disrupt all user sessions across all devices and require
        /// the user to log in again everywhere.
        /// </remarks>
        Task<bool> RevokeAllTokensAsync(int userId, CancellationToken cancellationToken = default);
    }
}
