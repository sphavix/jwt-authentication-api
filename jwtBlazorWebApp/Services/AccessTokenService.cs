namespace jwtBlazorWebApp.Services
{
    public class AccessTokenService
    {
        private readonly CookiesService _cookiesService;
        private readonly string tokenKey = "access_token";

        public AccessTokenService(CookiesService cookiesService)
        {
            _cookiesService = cookiesService;
        }

        public async Task SetToken(string accessToken)
        {
            await _cookiesService.SetCookie(tokenKey, accessToken, 1);
        }

        public async Task<string> GetToken()
        {
            return await _cookiesService.GetCookie(tokenKey);
        }

        public async Task RemoveToken()
        {
            await _cookiesService.RemoveCookie(tokenKey);
        }
    }
}
