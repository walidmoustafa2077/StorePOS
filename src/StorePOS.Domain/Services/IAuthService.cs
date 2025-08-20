using StorePOS.Domain.DTOs;

namespace StorePOS.Domain.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto, string? ipAddress = null, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);
        Task<bool> LogoutAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);
        Task<bool> LogoutAllAsync(int userId, CancellationToken cancellationToken = default);
    }
}
