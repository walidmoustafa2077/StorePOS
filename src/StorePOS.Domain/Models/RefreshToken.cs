namespace StorePOS.Domain.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = default!;
        public DateTimeOffset ExpiresAt { get; set; }
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedByIp { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplaceByToken { get; set; }
        public bool IsActive => RevokedAt == null && !IsExpired;
        
        // Foreign key
        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
