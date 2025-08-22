using StorePOS.Domain.DTOs;

namespace StorePOS.Domain.Services
{
    /// <summary>
    /// Service interface for user management operations in the Point-of-Sale system.
    /// Provides comprehensive user lifecycle management including CRUD operations, authentication, and profile management.
    /// </summary>
    /// <remarks>
    /// This service handles all user-related business logic including:
    /// - User registration and profile management
    /// - Search and lookup operations
    /// - Password management and security
    /// - Role-based access control
    /// - User activity tracking
    /// 
    /// All operations return standardized ServiceResult objects for consistent error handling.
    /// </remarks>
    public interface IUserService
    {
        /// <summary>
        /// Searches for users based on a query string, matching username, email, first name, or last name.
        /// </summary>
        /// <param name="q">Search query string to match against user fields</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>List of users matching the search criteria, limited to 100 results</returns>
        /// <remarks>
        /// Search behavior:
        /// - Case-insensitive partial matching using LIKE operations
        /// - Searches across username, email, first name, and last name fields
        /// - Results ordered alphabetically by username
        /// - Limited to 100 results for performance
        /// - Returns empty list if no query provided
        /// </remarks>
        Task<List<UserReadDto>> SearchAsync(string? q, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the user to retrieve</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>User data if found, null otherwise</returns>
        Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a user by their unique username.
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>User data if found, null otherwise</returns>
        /// <remarks>
        /// Username lookup is case-sensitive and requires exact match.
        /// Used for login validation and username availability checks.
        /// </remarks>
        Task<UserReadDto?> GetByUsernameAsync(string username, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>User data if found, null otherwise</returns>
        /// <remarks>
        /// Email lookup is case-insensitive and requires exact match.
        /// Used for login validation and email availability checks.
        /// </remarks>
        Task<UserReadDto?> GetByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// Creates a new user account with the provided information.
        /// </summary>
        /// <param name="dto">User creation data including username, email, password, and profile information</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing the created user data or validation errors</returns>
        /// <remarks>
        /// Creation validation:
        /// - Username and email uniqueness checks
        /// - Password strength requirements
        /// - Required field validation
        /// - Role assignment validation
        /// - Automatic password hashing
        /// - Initial user status configuration
        /// </remarks>
        Task<ServiceResult<UserReadDto>> CreateAsync(UserCreateDto dto, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing user's profile information.
        /// </summary>
        /// <param name="id">The ID of the user to update</param>
        /// <param name="dto">Updated user profile information</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing the updated user data or validation errors</returns>
        /// <remarks>
        /// Update validation:
        /// - User existence verification
        /// - Username and email uniqueness (excluding current user)
        /// - Role change authorization
        /// - Profile field validation
        /// - Preserves sensitive data not included in update
        /// </remarks>
        Task<ServiceResult<UserReadDto>> UpdateAsync(int id, UserUpdateDto dto, CancellationToken ct = default);

        /// <summary>
        /// Changes a user's password with current password verification.
        /// </summary>
        /// <param name="id">The ID of the user changing their password</param>
        /// <param name="dto">Password change data including current and new password</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result indicating success or failure with appropriate errors</returns>
        /// <remarks>
        /// Security measures:
        /// - Current password verification
        /// - New password strength validation
        /// - Password history checking (if implemented)
        /// - Automatic password hashing
        /// - Session invalidation consideration
        /// - Audit trail logging
        /// </remarks>
        Task<ServiceResult<bool>> ChangePasswordAsync(int id, UserChangePasswordDto dto, CancellationToken ct = default);

        /// <summary>
        /// Soft deletes a user account by marking it as inactive.
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result indicating success or failure</returns>
        /// <remarks>
        /// Deletion behavior:
        /// - Soft deletion (marks user as inactive rather than physical deletion)
        /// - Preserves data integrity for historical records
        /// - Prevents immediate re-use of username/email
        /// - Invalidates all active sessions
        /// - May require administrative privileges depending on role
        /// </remarks>
        Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct = default);
        
        /// <summary>
        /// Authenticates user credentials and returns user information for login validation.
        /// </summary>
        /// <param name="dto">Login credentials containing username/email and password</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing user data on successful authentication</returns>
        /// <remarks>
        /// Authentication process:
        /// - Username or email lookup
        /// - Password verification against stored hash
        /// - User status validation (active/inactive)
        /// - Account lockout checking
        /// - Returns generic errors to prevent user enumeration
        /// </remarks>
        Task<ServiceResult<UserReadDto>> LoginAsync(UserLoginDto dto, CancellationToken ct = default);

        /// <summary>
        /// Updates the last login timestamp for a user after successful authentication.
        /// </summary>
        /// <param name="id">The ID of the user who logged in</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result indicating success or failure</returns>
        /// <remarks>
        /// Used for:
        /// - User activity tracking
        /// - Security monitoring
        /// - Account usage analytics
        /// - Login history maintenance
        /// </remarks>
        Task<ServiceResult<bool>> UpdateLastLoginAsync(int id, CancellationToken ct = default);
    }
}
