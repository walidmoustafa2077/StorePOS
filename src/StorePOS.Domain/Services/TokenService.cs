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
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IOptions<JwtSettings> jwtSettings, IUnitOfWork unitOfWork)
        {
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

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

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<AuthTokenDto?> RefreshTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);
            if (user == null)
                return null;

            var token = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
            if (token == null || !token.IsActive)
                return null;

            // Replace old refresh token with a new one and save
            var newRefreshToken = RotateRefreshToken(token, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);
            user.LastLoginAt = DateTimeOffset.UtcNow;

            // Remove old refresh tokens from user
            RemoveOldRefreshTokens(user);

            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate new jwt
            var accessToken = GenerateAccessToken(user);

            return new AuthTokenDto(
                accessToken,
                newRefreshToken.Token,
                newRefreshToken.ExpiresAt,
                user.ToReadDto()
            );
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);
            if (user == null)
                return false;

            var token = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
            if (token == null || !token.IsActive)
                return false;

            // Revoke token and save
            RevokeRefreshToken(token, ipAddress);
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> RevokeAllTokensAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, false, cancellationToken);
            if (user == null)
                return false;

            // Revoke all active refresh tokens
            foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
            {
                RevokeRefreshToken(token);
            }

            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string? ipAddress)
        {
            var newRefreshToken = CreateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, newRefreshToken.Token);
            return newRefreshToken;
        }

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

        private void RevokeRefreshToken(RefreshToken token, string? ipAddress = null, string? replacedByToken = null)
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReplaceByToken = replacedByToken;
        }

        private void RemoveOldRefreshTokens(User user)
        {
            // Remove old inactive refresh tokens from user based on TTL in app settings
            var refreshTokenTTL = _jwtSettings.RefreshTokenExpirationDays;
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.CreatedAt.AddDays(refreshTokenTTL) <= DateTimeOffset.UtcNow);
        }
    }
}
