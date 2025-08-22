using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Repositories
{
    /// <summary>
    /// Repository implementation for User entities with specialized authentication and user management capabilities.
    /// Extends the generic repository to provide user-specific query operations optimized for authentication scenarios.
    /// </summary>
    /// <remarks>
    /// This repository implements user management functionality designed for authentication and authorization:
    /// - Username and email-based user lookup with normalization
    /// - Refresh token relationship management for JWT authentication
    /// - Duplicate user detection for registration validation
    /// - Performance-optimized queries for authentication scenarios
    /// - Proper email normalization (trim + lowercase) for consistency
    /// </remarks>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the UserRepository with the specified database context.
        /// </summary>
        /// <param name="db">The database context for data access operations</param>
        public UserRepository(AppDbContext db) : base(db) { }

        /// <summary>
        /// Retrieves a user by their username with case-sensitive matching.
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>The user with the specified username, or null if not found</returns>
        /// <remarks>
        /// Uses AsNoTracking for performance optimization in authentication scenarios.
        /// Trims whitespace from input for data consistency.
        /// </remarks>
        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _set.AsNoTracking()
                .Where(u => u.Username == username.Trim())
                .FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Retrieves a user by their email address with normalized case-insensitive matching.
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>The user with the specified email, or null if not found</returns>
        /// <remarks>
        /// Performs email normalization (trim + lowercase) to ensure consistent matching
        /// regardless of case variations in user input.
        /// </remarks>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _set.AsNoTracking()
                .Where(u => u.Email == email.Trim().ToLower())
                .FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Retrieves a user by either username or email address, with refresh tokens included.
        /// </summary>
        /// <param name="usernameOrEmail">The username or email to search for</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>The user matching the identifier with refresh tokens loaded, or null if not found</returns>
        /// <remarks>
        /// This method is optimized for authentication scenarios where:
        /// - Users can login with either username or email
        /// - Refresh tokens are needed for JWT token management
        /// - Single query retrieves all necessary authentication data
        /// 
        /// Includes proper email normalization for consistent matching.
        /// </remarks>
        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
                return null;

            var trimmed = usernameOrEmail.Trim();
            
            return await _set
                .Include(u => u.RefreshTokens)  // Include tokens for authentication flow
                .Where(u => u.Username == trimmed || u.Email == trimmed.ToLower())
                .FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Checks if a user with the specified username or email already exists in the system.
        /// </summary>
        /// <param name="username">The username to check for existence</param>
        /// <param name="email">The email to check for existence</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>True if a user with either the username or email exists, false otherwise</returns>
        /// <remarks>
        /// Used for registration validation to prevent duplicate accounts.
        /// Uses efficient existence checking without loading full user data.
        /// </remarks>
        public async Task<bool> ExistsAsync(string username, string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
                return false;

            return await _set.AsNoTracking()
                .AnyAsync(u => u.Username == username.Trim() || u.Email == email.Trim().ToLower(), ct);
        }

        /// <summary>
        /// Checks if a user with the specified username or email exists, excluding a specific user ID.
        /// </summary>
        /// <param name="username">The username to check for existence</param>
        /// <param name="email">The email to check for existence</param>
        /// <param name="excludeUserId">The user ID to exclude from the check</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>True if another user (not the excluded one) has the username or email, false otherwise</returns>
        /// <remarks>
        /// Used for profile update validation to prevent duplicate accounts while allowing
        /// users to keep their existing username/email during updates.
        /// </remarks>
        public async Task<bool> ExistsAsync(string username, string email, int excludeUserId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
                return false;

            return await _set.AsNoTracking()
                .Where(u => u.Id != excludeUserId)  // Exclude the current user
                .AnyAsync(u => u.Username == username.Trim() || u.Email == email.Trim().ToLower(), ct);
        }

        /// <summary>
        /// Retrieves a user by their associated refresh token, with all refresh tokens included.
        /// </summary>
        /// <param name="refreshToken">The refresh token to search for</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>The user associated with the refresh token, with all refresh tokens loaded, or null if not found</returns>
        /// <remarks>
        /// Used in JWT refresh token flows to:
        /// - Validate refresh token ownership
        /// - Load user data for new access token generation
        /// - Access all user's refresh tokens for rotation and cleanup
        /// 
        /// Enables change tracking to support token rotation and user updates.
        /// </remarks>
        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            // Include refresh tokens for token management operations
            // Use tracking context as we may need to modify tokens
            return await _set
                .Include(u => u.RefreshTokens)
                .Where(u => u.RefreshTokens.Any(t => t.Token == refreshToken))
                .FirstOrDefaultAsync(ct);
        }
    }
}
