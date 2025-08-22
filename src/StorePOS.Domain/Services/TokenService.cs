using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.DTOs;
using StorePOS.Domain.Extensions;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Services
{
    /// <summary>
    /// Service responsible for JWT token generation, refresh token management, and token lifecycle operations.
    /// Implements secure token rotation and automatic cleanup of expired tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the TokenService with JWT settings and data access.
        /// </summary>
        /// <param name="jwtSettings">JWT configuration settings including secret key, expiration times, and issuer information</param>
        /// <param name="unitOfWork">Unit of work for database operations and transaction management</param>
        public TokenService(IOptions<JwtSettings> jwtSettings, IUnitOfWork unitOfWork)
        {
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            // Build claims collection with user information for authorization
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("IsActive", user.IsActive.ToString())
            };

            // Configure token descriptor with security settings
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <inheritdoc />
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <inheritdoc />
        public async Task<AuthTokenDto?> RefreshTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default)
        {
            // Find user by refresh token
            var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);
            if (user == null)
                return null;

            // Validate refresh token exists and is still active
            var token = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
            if (token == null || !token.IsActive)
                return null;

            // Replace old refresh token with a new one (token rotation for security)
            var newRefreshToken = RotateRefreshToken(token, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);
            user.LastLoginAt = DateTimeOffset.UtcNow;

            // Clean up old expired refresh tokens to prevent database bloat
            RemoveOldRefreshTokens(user);

            // Persist changes to database
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate new JWT access token with updated user information
            var accessToken = GenerateAccessToken(user);

            return new AuthTokenDto(
                accessToken,
                newRefreshToken.Token,
                newRefreshToken.ExpiresAt,
                user.ToReadDto()
            );
        }

        /// <inheritdoc />
        public async Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);
            if (user == null)
                return false;

            var token = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
            if (token == null || !token.IsActive)
                return false;

            // Revoke the specific token with audit information
            RevokeRefreshToken(token, ipAddress);
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> RevokeAllTokensAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, false, cancellationToken);
            if (user == null)
                return false;

            // Revoke all active refresh tokens for security purposes
            foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
            {
                RevokeRefreshToken(token);
            }

            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        /// <summary>
        /// Creates a new refresh token while revoking the old one (token rotation pattern).
        /// </summary>
        /// <param name="refreshToken">The current refresh token to be replaced</param>
        /// <param name="ipAddress">IP address for audit trail purposes</param>
        /// <returns>A new RefreshToken entity with updated expiration and audit information</returns>
        /// <remarks>
        /// Token rotation is a security best practice that ensures refresh tokens are single-use
        /// and provides audit trail for token usage.
        /// </remarks>
        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string? ipAddress)
        {
            var newRefreshToken = CreateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, newRefreshToken.Token);
            return newRefreshToken;
        }

        /// <summary>
        /// Creates a new refresh token with proper expiration and audit information.
        /// </summary>
        /// <param name="ipAddress">IP address for audit trail purposes</param>
        /// <returns>A new RefreshToken entity ready for database storage</returns>
        private RefreshToken CreateRefreshToken(string? ipAddress)
        {
            return new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        /// <summary>
        /// Marks a refresh token as revoked with audit information.
        /// </summary>
        /// <param name="token">The refresh token to revoke</param>
        /// <param name="ipAddress">IP address performing the revocation</param>
        /// <param name="replacedByToken">Optional token that replaces this one (for rotation)</param>
        private void RevokeRefreshToken(RefreshToken token, string? ipAddress = null, string? replacedByToken = null)
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReplaceByToken = replacedByToken;
        }

        /// <summary>
        /// Removes old inactive refresh tokens to prevent database bloat.
        /// </summary>
        /// <param name="user">The user whose old tokens should be cleaned up</param>
        /// <remarks>
        /// Only removes tokens that are:
        /// 1. Already inactive (revoked or expired)
        /// 2. Older than the configured TTL (Time To Live)
        /// This maintains audit history while preventing unlimited token accumulation.
        /// </remarks>
        private void RemoveOldRefreshTokens(User user)
        {
            // Calculate cleanup threshold based on configured TTL
            var refreshTokenTTL = _jwtSettings.RefreshTokenExpirationDays;
            
            // Remove only inactive tokens older than TTL to maintain audit history
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.CreatedAt.AddDays(refreshTokenTTL) <= DateTimeOffset.UtcNow);
        }
    }
}
