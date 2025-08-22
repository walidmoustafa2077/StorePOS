using StorePOS.Domain.DTOs;

namespace StorePOS.Domain.Services
{
    /// <summary>
    /// Service interface for authentication operations in the Point-of-Sale system.
    /// Provides secure user login, token refresh, and logout functionality with session management.
    /// </summary>
    /// <remarks>
    /// This service coordinates between user validation, token generation, and security auditing.
    /// Key security features:
    /// - Password verification with secure hashing
    /// - JWT access token generation with user claims
    /// - Refresh token rotation for extended security
    /// - IP address tracking for session auditing
    /// - Comprehensive logout capabilities (single session or all sessions)
    /// </remarks>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user with username/email and password, generating access and refresh tokens.
        /// </summary>
        /// <param name="loginDto">Login credentials containing username/email and password</param>
        /// <param name="ipAddress">Client IP address for security auditing and session tracking</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>
        /// Authentication response containing success status, tokens, and user information.
        /// On failure, returns error message without sensitive information.
        /// </returns>
        /// <remarks>
        /// Security considerations:
        /// - Validates user credentials against stored password hash
        /// - Updates last login timestamp for user activity tracking
        /// - Generates new refresh token and revokes old ones beyond security threshold
        /// - Logs authentication attempts for security monitoring
        /// - Returns generic error messages to prevent user enumeration attacks
        /// </remarks>
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto, string? ipAddress = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token, implementing token rotation.
        /// </summary>
        /// <param name="refreshToken">Valid refresh token from previous authentication</param>
        /// <param name="ipAddress">Client IP address for security auditing</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>
        /// New authentication response with fresh access and refresh tokens.
        /// On failure, returns error message and invalidates the provided refresh token.
        /// </returns>
        /// <remarks>
        /// Token rotation security:
        /// - Validates refresh token existence and expiration
        /// - Generates new access token with updated claims
        /// - Issues new refresh token and invalidates the old one
        /// - Tracks token usage for suspicious activity detection
        /// - Maintains session continuity while enhancing security posture
        /// </remarks>
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs out a user by invalidating their specific refresh token session.
        /// </summary>
        /// <param name="refreshToken">The refresh token to invalidate</param>
        /// <param name="ipAddress">Client IP address for security auditing</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>True if logout was successful, false if token was not found</returns>
        /// <remarks>
        /// Single session logout:
        /// - Invalidates only the specific refresh token provided
        /// - Allows user to remain logged in on other devices/sessions
        /// - Updates token revocation timestamp for audit trails
        /// - Suitable for "logout from this device" scenarios
        /// </remarks>
        Task<bool> LogoutAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs out a user from all active sessions by invalidating all their refresh tokens.
        /// </summary>
        /// <param name="userId">The ID of the user to log out from all sessions</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>True if logout was successful, false if user was not found</returns>
        /// <remarks>
        /// Global logout scenarios:
        /// - Security breaches or password changes
        /// - Administrative account suspension
        /// - User-initiated "logout from all devices"
        /// - Forces re-authentication on all devices
        /// - Creates comprehensive audit trail for security events
        /// </remarks>
        Task<bool> LogoutAllAsync(int userId, CancellationToken cancellationToken = default);
    }
}
