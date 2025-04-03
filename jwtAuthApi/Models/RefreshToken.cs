namespace jwtAuthApi.Models
{
    public class RefreshToken
    {
        public required string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Enabled { get; set; }
        public string Email { get; set; }
    }
}
