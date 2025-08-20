using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.DTOs;
using StorePOS.Domain.Extensions;
using StorePOS.Domain.Helpers;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto, string? ipAddress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                // Find user by username or email
                var user = await _unitOfWork.Users.GetByUsernameOrEmailAsync(loginDto.UsernameOrEmail, cancellationToken);
                
                if (user == null || !user.IsActive)
                {
                    return new AuthResponseDto(false, "Invalid credentials", null);
                }

                // Verify password
                if (!PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return new AuthResponseDto(false, "Invalid credentials", null);
                }

                // Generate tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = CreateRefreshToken(ipAddress);
                
                // Add refresh token to user
                user.RefreshTokens.Add(refreshToken);
                user.LastLoginAt = DateTimeOffset.UtcNow;

                // Remove old refresh tokens
                RemoveOldRefreshTokens(user);

                await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var authToken = new AuthTokenDto(
                    accessToken,
                    refreshToken.Token,
                    refreshToken.ExpiresAt,
                    user.ToReadDto()
                );

                return new AuthResponseDto(true, "Login successful", authToken);
            }
            catch (Exception ex)
            {
                return new AuthResponseDto(false, $"Login failed: {ex.Message}", null);
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await _tokenService.RefreshTokenAsync(refreshToken, ipAddress, cancellationToken);
                
                if (authToken == null)
                {
                    return new AuthResponseDto(false, "Invalid refresh token", null);
                }

                return new AuthResponseDto(true, "Token refreshed successfully", authToken);
            }
            catch (Exception ex)
            {
                return new AuthResponseDto(false, $"Token refresh failed: {ex.Message}", null);
            }
        }

        public async Task<bool> LogoutAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default)
        {
            return await _tokenService.RevokeTokenAsync(refreshToken, ipAddress, cancellationToken);
        }

        public async Task<bool> LogoutAllAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _tokenService.RevokeAllTokensAsync(userId, cancellationToken);
        }

        private RefreshToken CreateRefreshToken(string? ipAddress)
        {
            return new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7), // TODO: Get from settings
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void RemoveOldRefreshTokens(User user)
        {
            // Remove old inactive refresh tokens (older than 7 days)
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.CreatedAt.AddDays(7) <= DateTimeOffset.UtcNow);
        }
    }
}
