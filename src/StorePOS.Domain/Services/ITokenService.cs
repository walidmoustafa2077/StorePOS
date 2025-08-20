using StorePOS.Domain.DTOs;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task<AuthTokenDto?> RefreshTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);
        Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);
        Task<bool> RevokeAllTokensAsync(int userId, CancellationToken cancellationToken = default);
    }
}
