namespace jwtAuthApi.Models
{
    public class UserToken
    {
        public string AccessToken { get; set; } = string.Empty;
        public RefreshToken RefershToken { get; set; }
    }
}
