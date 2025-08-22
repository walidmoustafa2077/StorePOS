using StorePOS.Domain.Models;
using System.Net;
using System.Text.RegularExpressions;

namespace StorePOS.Domain.Helpers
{
    /// <summary>
    /// Helper class providing security-related utilities for the Point-of-Sale system.
    /// Includes IP address validation, token creation, and security audit support.
    /// </summary>
    public static class SecurityHelper
    {
        private static readonly Regex IpV4Regex = new(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", RegexOptions.Compiled);
        private static readonly Regex IpV6Regex = new(@"^([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$|^::1$|^::$", RegexOptions.Compiled);

        /// <summary>
        /// Validates and normalizes an IP address for security operations.
        /// </summary>
        /// <param name="ipAddress">The IP address to validate</param>
        /// <param name="parameterName">The parameter name for exception reporting</param>
        /// <returns>The validated and normalized IP address</returns>
        /// <exception cref="ArgumentException">Thrown when IP address is null, empty, or invalid</exception>
        /// <remarks>
        /// This method ensures all security operations have valid IP addresses for:
        /// - Audit trail accuracy
        /// - Security monitoring
        /// - Compliance requirements
        /// - Attack pattern detection
        /// </remarks>
        public static string ValidateAndNormalizeIpAddress(string? ipAddress, string parameterName = "ipAddress")
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new ArgumentException("IP address is required for security operations and cannot be null or empty.", parameterName);
            }

            var trimmedIp = ipAddress.Trim();

            // Handle localhost variations
            if (trimmedIp.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                return "127.0.0.1";
            }

            // Validate IPv4
            if (IpV4Regex.IsMatch(trimmedIp))
            {
                return trimmedIp;
            }

            // Validate IPv6
            if (IpV6Regex.IsMatch(trimmedIp) || IPAddress.TryParse(trimmedIp, out var ipv6) && ipv6.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return trimmedIp;
            }

            throw new ArgumentException($"Invalid IP address format: '{trimmedIp}'. Must be a valid IPv4 or IPv6 address.", parameterName);
        }

        /// <summary>
        /// Creates a new refresh token with proper security settings and IP address validation.
        /// </summary>
        /// <param name="tokenValue">The cryptographically secure token value</param>
        /// <param name="ipAddress">The IP address creating the token (required for security audit)</param>
        /// <param name="expirationDays">The number of days until token expires (default: 7)</param>
        /// <returns>A new RefreshToken with validated IP address and security settings</returns>
        /// <exception cref="ArgumentException">Thrown when IP address is invalid</exception>
        /// <remarks>
        /// Centralizes refresh token creation to ensure:
        /// - Consistent expiration policies
        /// - Proper IP address validation
        /// - Security audit trail
        /// - Token lifecycle management
        /// </remarks>
        public static RefreshToken CreateRefreshToken(string tokenValue, string? ipAddress, int expirationDays = 7)
        {
            var validatedIp = ValidateAndNormalizeIpAddress(ipAddress, nameof(ipAddress));

            return new RefreshToken
            {
                Token = tokenValue,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(expirationDays),
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedByIp = validatedIp
            };
        }

        /// <summary>
        /// Validates an IP address without throwing exceptions.
        /// </summary>
        /// <param name="ipAddress">The IP address to validate</param>
        /// <returns>True if the IP address is valid, false otherwise</returns>
        /// <remarks>
        /// Use this method when you need to check IP validity without exception handling.
        /// Useful for input validation in controllers or APIs.
        /// </remarks>
        public static bool IsValidIpAddress(string? ipAddress)
        {
            try
            {
                ValidateAndNormalizeIpAddress(ipAddress);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a default IP address for local operations when none is provided.
        /// </summary>
        /// <returns>The localhost IP address (127.0.0.1)</returns>
        /// <remarks>
        /// Use sparingly and only for internal system operations.
        /// Real user operations should always provide actual client IP addresses.
        /// </remarks>
        public static string GetLocalhostIpAddress() => "127.0.0.1";

        /// <summary>
        /// Checks if an IP address represents a localhost connection.
        /// </summary>
        /// <param name="ipAddress">The IP address to check</param>
        /// <returns>True if the IP represents localhost, false otherwise</returns>
        public static bool IsLocalhostAddress(string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            var trimmed = ipAddress.Trim();
            return trimmed.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                   trimmed.Equals("::1", StringComparison.OrdinalIgnoreCase) ||
                   trimmed.Equals("localhost", StringComparison.OrdinalIgnoreCase);
        }
    }
}