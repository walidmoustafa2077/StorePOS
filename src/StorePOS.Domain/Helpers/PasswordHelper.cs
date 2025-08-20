using System.Security.Cryptography;
using System.Text;

namespace StorePOS.Domain.Helpers
{
    public static class PasswordHelper
    {
        private const string Salt = "StorePOS_Salt_2024"; // Consider using a more secure approach in production

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            using var sha256 = SHA256.Create();
            var saltedPassword = password + Salt;
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashedBytes);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
    }
}
