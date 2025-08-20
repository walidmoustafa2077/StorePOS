using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext db) : base(db) { }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _set.AsNoTracking()
                .Where(u => u.Username == username.Trim())
                .FirstOrDefaultAsync(ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _set.AsNoTracking()
                .Where(u => u.Email == email.Trim().ToLower())
                .FirstOrDefaultAsync(ct);
        }

        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
                return null;

            var trimmed = usernameOrEmail.Trim();
            
            return await _set
                .Include(u => u.RefreshTokens)
                .Where(u => u.Username == trimmed || u.Email == trimmed.ToLower())
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> ExistsAsync(string username, string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
                return false;

            return await _set.AsNoTracking()
                .AnyAsync(u => u.Username == username.Trim() || u.Email == email.Trim().ToLower(), ct);
        }

        public async Task<bool> ExistsAsync(string username, string email, int excludeUserId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
                return false;

            return await _set.AsNoTracking()
                .Where(u => u.Id != excludeUserId)
                .AnyAsync(u => u.Username == username.Trim() || u.Email == email.Trim().ToLower(), ct);
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            return await _set
                .Include(u => u.RefreshTokens)
                .Where(u => u.RefreshTokens.Any(t => t.Token == refreshToken))
                .FirstOrDefaultAsync(ct);
        }
    }
}
