using System.ComponentModel.DataAnnotations;
using StorePOS.Domain.Enums;

namespace StorePOS.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; } = UserRole.Cashier;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? LastLoginAt { get; set; }
        [Timestamp] public byte[]? RowVersion { get; set; }
        
        // Navigation properties
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
