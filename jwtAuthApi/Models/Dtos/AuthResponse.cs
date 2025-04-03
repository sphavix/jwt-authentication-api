namespace jwtAuthApi.Models.Dtos
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToke { get; set; } = string.Empty;
    }
}
