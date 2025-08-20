using StorePOS.Domain.Models;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Get user by username or email
        Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail, CancellationToken ct = default);
        
        // Check if username/email exists
        Task<bool> ExistsAsync(string username, string email, CancellationToken ct = default);
        Task<bool> ExistsAsync(string username, string email, int excludeUserId, CancellationToken ct = default);
        
        // Refresh token methods
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    }
}
