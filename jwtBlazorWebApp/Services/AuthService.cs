using jwtBlazorWebApp.Models.Dtos;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace jwtBlazorWebApp.Services
{
    public class AuthService(AccessTokenService accessTokenService,
    NavigationManager navManager,
    IHttpClientFactory httpClientFactory)
    {
        private readonly AccessTokenService _accessTokenService = accessTokenService;
        private readonly NavigationManager _navManager = navManager;
        private HttpClient httpClient = httpClientFactory.CreateClient("ApiClient");


        public async Task<bool> Login(string email, string password)
        {
            var response = await httpClient.PostAsJsonAsync("auth", new { email, password });

            if(response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<AuthResponse>(token);

                await _accessTokenService.SetToken(result!.AccessToken);

                return true;
            }
            else
            {
                return false;
            }


        }
    }
}
